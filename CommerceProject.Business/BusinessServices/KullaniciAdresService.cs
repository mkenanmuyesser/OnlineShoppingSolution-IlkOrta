using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;

namespace CommerceProject.Business.BusinessServices
{
    public class KullaniciAdresService : GenericRepository<KullaniciAdres>, IKullaniciAdresService
    {
        public List<KullaniciAdres> GetUserAdresses(Guid userId)
        {
            var kullaniciAdresler = this.FindBy(x => x.AktifMi && 
                                                     x.Adres.AktifMi&&
                                                     x.KullaniciId == userId,
                                            false,
                                            new string[] { "Adres", "Adres.FaturaTip", "Adres.AdresIl", "Adres.AdresIlce" }).ToList();
            return kullaniciAdresler;
        }

        public KullaniciAdres GetUserAdress(Guid userId, int adresId)
        {
            var kullaniciAdres = this.GetSingle(x => x.AktifMi == true &&
                                                     x.Adres.AktifMi &&
                                                     x.KullaniciId == userId &&
                                                     x.AdresId == adresId,
                                            false,
                                            new string[] { "Adres", "Adres.FaturaTip", "Adres.AdresIl", "Adres.AdresIlce" });
            return kullaniciAdres;
        }
    }
}
