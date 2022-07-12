using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using CommerceProject.Business.Helper.Cookie;
using System.Collections.Specialized;
using System.Web;

namespace CommerceProject.Business.BusinessServices
{
    public class SepetService : GenericRepository<Sepet>, ISepetService
    {
        private readonly IUrunService urunService = new UrunService();
        private readonly IIcerikAyarService icerikAyarService = new IcerikAyarService();

        public List<Sepet> GetUserProductsFromBasket(Guid kullaniciId)
        {
            var sepetList = new List<Sepet>();

            //Kullanıcı giriş yapmadıysa cookie okunur. Eğer giriş yapıldıysa veritabanından sepet okunur.
            if (kullaniciId == Guid.Empty)
            {
                var cookie = CookieHelper.GetUserCookie();

                if (cookie != null && cookie.Values.Count > 0)
                {
                    string cookieValues = cookie.Values.ToString();
                    foreach (var keyValue in cookieValues.Split('&'))
                    {
                        if (!string.IsNullOrEmpty(keyValue))
                        {
                            string[] keyValueArray = keyValue.Split('=');
                            int urunId = Convert.ToInt32(keyValueArray[0]);
                            int adet = Convert.ToInt32(keyValueArray[1]);

                            Urun urun = null;
                            if (icerikAyarService.GetFirst().CacheAktifMi)
                                urun = urunService.GetSingleProductFromCache(urunId);
                            else
                                urun = urunService.GetSingleProduct(urunId);

                            var sepet = new Sepet
                            {
                                UrunId = Convert.ToInt32(urunId),
                                Urun = urun,
                                Adet = adet,
                                ToplamFiyat = urun.Fiyat * adet,
                                KullaniciId = Guid.Empty,
                                AktifMi = true,
                            };
                            sepetList.Add(sepet);
                        }
                    }
                }
            }
            else
            {
                sepetList = this.FindBy(x => x.AktifMi &&
                                             x.SepetTipId == 1 &&
                                             x.KullaniciId == kullaniciId, true, new string[] { }).
                                             AsEnumerable().
                                             Select(x =>
                                             {
                                                 x.Urun = icerikAyarService.GetFirst().CacheAktifMi ? urunService.GetSingleProductFromCache(x.UrunId) : urunService.GetSingleProduct(x.UrunId);
                                                 x.ToplamFiyat = x.Urun.Fiyat * x.Adet;
                                                 return x;
                                             }).ToList();
            }

            return sepetList;
        }

        public bool SetUserProductsFromBasket(Guid kullaniciId, int urunId, int adet)
        {
            //Kullanıcı giriş yapmadıysa cookie okunur. Eğer giriş yapıldıysa veritabanından sepet okunur.
            if (adet > 0)
            {
                if (kullaniciId == Guid.Empty)
                {
                    var cookie = CookieHelper.GetUserCookie();

                    if (cookie.HasKeys)
                    {
                        //Aynı üründen varsa adeti arttırılır. Yoksa yeni value oluşturulur.
                        var urunAdet = cookie.Values[urunId.ToString()];
                        if (string.IsNullOrEmpty(urunAdet))
                        {
                            cookie.Values.Add(urunId.ToString(), adet.ToString());
                        }
                        else
                        {
                            cookie.Values[urunId.ToString()] = (Convert.ToInt32(urunAdet) + adet).ToString();
                        }
                    }
                    else
                    {
                        cookie.Values.Add(urunId.ToString(), adet.ToString());
                    }

                    CookieHelper.SetUserCookie(cookie);
                }
                else
                {
                    //Aynı üründen varsa adeti arttırılır. Yoksa yeni value oluşturulur.
                    Sepet sepet = this.GetFirst(x => x.AktifMi &&
                                                 x.SepetTipId == 1 &&
                                                 x.KullaniciId == kullaniciId &&
                                                 x.UrunId == urunId, true, new string[] { "Urun" });

                    if (sepet != null)
                    {
                        sepet.Adet += adet;
                        sepet.GuncellemeTarihi = DateTime.Now;

                        this.Edit(sepet);
                    }
                    else
                    {
                        sepet = new Sepet
                        {
                            SepetTipId = 1,
                            KullaniciId = kullaniciId,
                            UrunId = urunId,
                            Adet = adet,
                            OlusturmaTarihi = DateTime.Now,
                            GuncellemeTarihi = DateTime.Now,
                            AktifMi = true
                        };

                        this.Add(sepet);
                    }

                    this.Save();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetUserPackagesFromBasket(Guid kullaniciId, int adet, Paket paket)
        {
            //Kullanıcı giriş yapmadıysa cookie okunur. Eğer giriş yapıldıysa veritabanından sepet okunur.
            if (adet > 0)
            {
                if (kullaniciId == Guid.Empty)
                {
                    var cookie = CookieHelper.GetUserCookie();

                    if (cookie.HasKeys)
                    {
                        foreach (var paketUrun in paket.PaketUrun)
                        {
                            int urunId = paketUrun.UrunId;
                            int paketAdet = paketUrun.Adet;
                            //Aynı üründen varsa adeti arttırılır. Yoksa yeni value oluşturulur.
                            var urunAdet = cookie.Values[urunId.ToString()];
                            if (string.IsNullOrEmpty(urunAdet))
                            {
                                cookie.Values.Add(urunId.ToString(), (paketAdet * adet).ToString());
                            }
                            else
                            {
                                cookie.Values[urunId.ToString()] = (Convert.ToInt32(urunAdet) + (paketAdet * adet)).ToString();
                            }
                        }
                    }
                    else
                    {
                        foreach (var paketUrun in paket.PaketUrun)
                        {
                            int urunId = paketUrun.UrunId;
                            int paketAdet = paketUrun.Adet;
                            cookie.Values.Add(urunId.ToString(), (paketAdet * adet).ToString());
                        }
                    }

                    CookieHelper.SetUserCookie(cookie);
                }
                else
                {
                    foreach (var paketUrun in paket.PaketUrun)
                    {
                        int urunId = paketUrun.UrunId;
                        int paketAdet = paketUrun.Adet;
                        //Aynı üründen varsa adeti arttırılır. Yoksa yeni value oluşturulur.
                        Sepet sepet = this.GetFirst(x => x.AktifMi &&
                                                 x.SepetTipId == 1 &&
                                                 x.KullaniciId == kullaniciId &&
                                                 x.UrunId == urunId, true, new string[] { "Urun" });

                        if (sepet != null)
                        {
                            sepet.Adet += (paketAdet * adet);
                            sepet.GuncellemeTarihi = DateTime.Now;

                            this.Edit(sepet);
                        }
                        else
                        {
                            sepet = new Sepet
                            {
                                SepetTipId = 1,
                                KullaniciId = kullaniciId,
                                UrunId = urunId,
                                Adet = (paketAdet * adet),
                                OlusturmaTarihi = DateTime.Now,
                                GuncellemeTarihi = DateTime.Now,
                                AktifMi = true
                            };

                            this.Add(sepet);
                        }
                    }

                    this.Save();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveUserProductsFromBasket(Guid kullaniciId, int urunId, int adet)
        {
            //Kullanıcı giriş yapmadıysa cookie okunur. Eğer giriş yapıldıysa veritabanından sepet okunur.
            if (adet > 0)
            {
                if (kullaniciId == Guid.Empty)
                {
                    var cookie = CookieHelper.GetUserCookie();

                    if (cookie != null && cookie.Values.Count > 0)
                    {
                        int urunAdet = 0;
                        int.TryParse(cookie.Values[urunId.ToString()], out urunAdet);

                        if (urunAdet > 0)
                        {

                            urunAdet -= adet;

                            if (urunAdet <= 0)
                                cookie.Values.Remove(urunId.ToString());
                            else
                                cookie.Values[urunId.ToString()] = urunAdet.ToString();
                        }

                        CookieHelper.SetUserCookie(cookie);
                    }
                }
                else
                {
                    //Aynı üründen varsa silme işlemi veritabanında yapılır. Yoksa false değer döndürülür.
                    Sepet sepet = this.GetFirst(x => x.AktifMi &&
                                                  x.SepetTipId == 1 &&
                                                  x.KullaniciId == kullaniciId &&
                                                  x.UrunId == urunId, true, new string[] { "Urun" });

                    if (sepet != null)
                    {
                        if (sepet.Adet > 0)
                        {
                            sepet.Adet -= adet;

                            if (sepet.Adet <= 0)
                                sepet.AktifMi = false;
                        }
                        else
                        {
                            sepet.AktifMi = false;
                        }

                        sepet.GuncellemeTarihi = DateTime.Now;

                        this.Edit(sepet);
                    }
                    else
                    {
                        return false;
                    }

                    return this.Save();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ClearUserProductsFromBasket(Guid kullaniciId)
        {
            //Kullanıcı giriş yapmadıysa cookie okunur. Eğer giriş yapıldıysa veritabanından sepet okunur.
            if (kullaniciId == Guid.Empty)
            {
                var cookie = CookieHelper.GetUserCookie();

                if (cookie != null && cookie.Values.Count > 0)
                {
                    return CookieHelper.ClearCookie(cookie);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //sepetteki bütün aktif ürünler false olacak
                var sepettekiUrunler = this.FindBy(x => x.AktifMi &&
                                              x.SepetTipId == 1 &&
                                              x.KullaniciId == kullaniciId, true, new string[] { "Urun" });

                if (sepettekiUrunler.Any())
                {
                    foreach (var sepettekiUrun in sepettekiUrunler)
                    {
                        sepettekiUrun.AktifMi = false;
                        sepettekiUrun.GuncellemeTarihi = DateTime.Now;
                        this.Edit(sepettekiUrun);
                    }

                    return this.Save();
                }
                else
                {
                    return false;
                }
            }
        }

        public bool SetProcessUserProductsFromBasket(Guid kullaniciId)
        {
            //Kullanıcı giriş yapmadıysa cookie okunur. Eğer giriş yapıldıysa veritabanından sepet okunur.
            if (kullaniciId == Guid.Empty)
            {
                return false;
            }
            else
            {
                //sepetteki bütün aktif ürünler false olacak
                var sepettekiUrunler = this.FindBy(x => x.AktifMi &&
                                              x.SepetTipId == 1 &&
                                              x.KullaniciId == kullaniciId, true, new string[] { "Urun" });

                if (sepettekiUrunler.Any())
                {
                    foreach (var sepettekiUrun in sepettekiUrunler)
                    {
                        sepettekiUrun.AktifMi = false;
                        sepettekiUrun.SepetTipId = 2;
                        sepettekiUrun.GuncellemeTarihi = DateTime.Now;
                        this.Edit(sepettekiUrun);
                    }

                    return this.Save();
                }
                else
                {
                    return false;
                }
            }
        }

        //bu silinecek
        public bool RemoveProductsFromBasket(Guid kullaniciId, int urunId)
        {
            //Kullanıcı giriş yapmadıysa cookie okunur. Eğer giriş yapıldıysa veritabanından sepet okunur.
            if (kullaniciId == Guid.Empty)
            {
                var cookie = CookieHelper.GetUserCookie();

                if (cookie != null && cookie.Values.Count > 0)
                {
                    cookie.Values.Remove(urunId.ToString());

                    CookieHelper.SetUserCookie(cookie);
                }
            }
            else
            {
                //Aynı üründen varsa silme işlemi veritabanında yapılır. Yoksa false değer döndürülür.
                Sepet sepet = this.GetFirst(x => x.AktifMi &&
                                              x.SepetTipId == 1 &&
                                              x.KullaniciId == kullaniciId &&
                                              x.UrunId == urunId, true, new string[] { "Urun" });

                if (sepet != null)
                {
                    sepet.AktifMi = false;
                    sepet.GuncellemeTarihi = DateTime.Now;

                    this.Edit(sepet);
                }
                else
                {
                    return false;
                }

                return this.Save();
            }

            return true;
        }
    }
}
