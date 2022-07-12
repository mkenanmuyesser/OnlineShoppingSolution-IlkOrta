using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using System.Web.Security;
using CommerceProject.Business.Helper.Cryptography;
using System.Web;
using System.Security.Principal;

namespace CommerceProject.Business.BusinessServices
{
    public class KullaniciService : GenericRepository<Kullanici>, IKullaniciService
    {
        public bool IsAuthenticated()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public bool LoginUser(string email, string password, bool rememberMe = false)
        {
            bool isValidUser = false;
            Kullanici kullanici = this.IsValidUser(email, password);
            if (kullanici != null)
            {
                var authTicket = new FormsAuthenticationTicket(
            1,
            email,
            DateTime.Now,
            DateTime.Now.AddMinutes(20),
            rememberMe,
            GetUserRoles()
            );

                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                HttpContext.Current.Response.Cookies.Add(authCookie);

                HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(email, "CommerceAuthTypeName"), authTicket.UserData.Split(';'));

                isValidUser = true;
            }

            return isValidUser;
        }

        public bool LoginAdminUser(string email, string password, bool rememberMe = false)
        {
            bool isValidUser = false;
            Kullanici kullanici = this.IsValidUser(email, password);
            if (kullanici != null)
            {
                var authTicket = new FormsAuthenticationTicket(1,
                                                                email,
                                                                DateTime.Now,
                                                                DateTime.Now.AddMinutes(20),
                                                                rememberMe,
                                                                GetUserRoles()
                                                                );

                Guid superAdminGuid = Guid.Parse("fe3e6535-b81c-483e-ae2f-a116a1128c8a");
                // kullanıcı admin yetkisine sahip veya süper admin ise giriş yap
                if (kullanici.KullaniciRol.Any(x => x.AktifMi && x.Rol.AktifMi && x.RolId == 1) || kullanici.KullaniciId == superAdminGuid)
                {
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    HttpContext.Current.Response.Cookies.Add(authCookie);

                    HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(email, "CommerceAuthTypeName"), authTicket.UserData.Split(';'));

                    isValidUser = true;
                }
            }

            return isValidUser;
        }

        public void LogoutUser()
        {
            FormsAuthentication.SignOut();
        }

        public Kullanici GetAuthenticatedUser(bool asNoTracking = false)
        {
            Kullanici kullanici = null;

            if (HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var authTicket = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value);

                if (!authTicket.Expired)
                {
                    var eposta = authTicket.Name;
                    kullanici = this.GetSingle(x => x.Eposta == eposta, asNoTracking, new string[] {
                                                                                      "KullaniciRol" ,
                                                                                      "KullaniciRol.Rol",
                                                                                      "KullaniciYetki",
                                                                                      "KullaniciYetki.Yetki",
                                                                                      "KullaniciDetay" ,
                                                                                      "KullaniciAdres",
                                                                                      "KullaniciAdres.Adres"});
                }
            }

            return kullanici;
        }

        public bool CreateUser(string ad, string soyad, string sifre, string eposta, int? sirketId = null)
        {
            //ayni eposta adresi ile kullanıcı olusturulmus mu?
            if (this.GetSingle(x => x.Eposta == eposta) == null)
            {
                try
                {
                    Kullanici kullanici = new Kullanici
                    {
                        Eposta = eposta,
                        Sifre = CryptoHelper.Encode(sifre),
                        KullaniciDetay = new KullaniciDetay
                        {
                            Ad = ad,
                            Soyad = soyad,
                            SepetteUrunVarMi = false,
                            SirketId = sirketId
                        },
                        KullaniciId = Guid.NewGuid(),
                        UyelikTarihi = DateTime.Now,
                        SonGirisTarihi = DateTime.Now,
                        BasarisizGirisSayisi = 0,
                        HesapKilitliMi = false,
                        EpostaOnayliMi = true,
                        AktifMi = true
                    };

                    this.Add(kullanici);

                    return this.Save();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
                return false;
        }

        public bool UpdateUserPassword(string eskiSifre, string yeniSifre)
        {
            var kullanici = this.GetAuthenticatedUser(true);

            if (kullanici != null)
            {
                //kullanıcı şifresi eşleşiyor mu?
                string sifre = CryptoHelper.Encode(eskiSifre);
                if (kullanici.Sifre == sifre)
                {
                    try
                    {
                        kullanici.Sifre = CryptoHelper.Encode(yeniSifre);
                        this.Edit(kullanici);
                        this.Save();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
            else
                return false;
        }

        public string GenerateUserPassword(string eposta)
        {
            string yeniSifre = "";
            Kullanici kullanici = this.IsValidEmail(eposta);

            if (kullanici != null)
            {
                yeniSifre = Membership.GeneratePassword(8, 2);

                try
                {
                    kullanici.Sifre = CryptoHelper.Encode(yeniSifre);
                    this.Edit(kullanici);

                    var guncelle = this.Save();
                    if (!guncelle)
                        yeniSifre = "";
                }
                catch (Exception ex)
                {
                    yeniSifre = "";
                }
            }

            return yeniSifre;
        }

        private Kullanici IsValidUser(string email, string password)
        {
            string sifre = CryptoHelper.Encode(password);

            var kullanici = this.GetSingle(x => (x.AktifMi == true &&
                                                 x.EpostaOnayliMi == true &&
                                                 x.HesapKilitliMi == false &&
                                                 x.Eposta == email &&
                                                 x.Sifre == sifre),
                                                 false,
                                                 new string[] {
                                                     "KullaniciRol" ,
                                                     "KullaniciRol.Rol"
                                                 });

            return kullanici;
        }

        private Kullanici IsValidEmail(string email)
        {
            var kullanici = this.GetSingle(x => (x.AktifMi == true &&
                                                 x.Eposta == email),
                                                 false,
                                                 new string[] {
                                                     "KullaniciRol" ,
                                                     "KullaniciRol.Rol"
                                                 });

            return kullanici;
        }

        private string GetUserRoles()
        {
            string roller = "";

            var kullanici = this.GetAuthenticatedUser();

            if (kullanici != null)
            {
                if (kullanici.KullaniciRol.Any(x => x.AktifMi))
                {
                    foreach (var rol in kullanici.KullaniciRol)
                    {
                        if (!roller.Contains(rol.Rol.Adi))
                        {
                            roller += rol.Rol.Adi + ";";
                        }
                    }

                    if (roller.EndsWith(";"))
                        roller = roller.Remove(roller.Length - 1, 1);
                }
            }

            return roller;
        }
    }
}
