using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;

namespace CommerceProject.Business.BusinessServices
{
    public class KullaniciDetayService : GenericRepository<KullaniciDetay>, IKullaniciDetayService
    {
        public bool UpdateUserDetail(Kullanici kullanici, string ad, string soyad)
        {           
            //ayni eposta adresi ile kullanıcı olusturulmus mu?
            if (kullanici != null)
            {
                var kullaniciDetay = this.GetSingle(x => x.KullaniciDetayId == kullanici.KullaniciId, true);

                if (kullaniciDetay != null)
                {
                    try
                    {
                        kullaniciDetay.Ad = ad;
                        kullaniciDetay.Soyad = soyad;

                        this.Edit(kullaniciDetay);
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
    }
}
