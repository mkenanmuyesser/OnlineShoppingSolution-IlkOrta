using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;

namespace CommerceProject.Business.BusinessServices
{
    public class IstekListesiService : GenericRepository<IstekListesi>, IIstekListesiService
    {
        public List<IstekListesi> GetUserWishList(Guid userId)
        {
            var istekListesi = this.FindBy(x => x.AktifMi == true &&
                                                x.KullaniciId == userId,
                                                false,
                                                new string[] { "Urun", "Urun.StokHareket" }).
                                                ToList();                                               

            //istekListesi.ForEach(x =>
            //{
            //    //x.StoktaVarMi = x.Urun.StokHareket.Any(p => p.AktifMi == true) ?
            //    //       (x.Urun.StokHareket.Where(p => p.AktifMi == true && p.StokHareketTipId == 1).Sum(p => p.Miktar) -
            //    //       (x.Urun.StokHareket.Where(p => p.AktifMi == true && p.StokHareketTipId == 2).Sum(p => p.Miktar))) > 0 : false;
            //});

            return istekListesi;
        }
    }
}
