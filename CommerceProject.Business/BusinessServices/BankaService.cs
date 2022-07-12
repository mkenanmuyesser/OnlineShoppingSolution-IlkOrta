using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using CommerceProject.Business.Custom;
using CommerceProject.Business.Helper.Caching;

namespace CommerceProject.Business.BusinessServices
{
    public class BankaService : GenericRepository<Banka>, IBankaService
    {
        #region Cache
        public List<Banka> GetInstallmentCalculationDataByPriceFromCache(decimal fiyat)
        {
            var bankalar = this.FindByFromCache(x => (x.AktifMi == true &&
                                                      x.Taksit.Any(p => p.AktifMi == true)),
                                                      CacheDataObj.AktifTaksitler,
                                                      new string[] { "Taksit" }).
                                                      OrderBy(x => x.Sira).ToList();
            var bankalarData = new List<Banka>();

            foreach (var banka in bankalar)
            {
                var taksitBilgileri = new List<Taksit>();

                foreach (var taksit in banka.Taksit)
                {
                    if (taksit.TaksitSayisi > 0)
                    {
                        decimal aylikTaksit = fiyat / (decimal)taksit.TaksitSayisi;
                        taksit.AylikOdeme = aylikTaksit + (aylikTaksit * taksit.FaizOrani);
                        taksit.ToplamTutar = taksit.AylikOdeme * taksit.TaksitSayisi;
                    }

                    taksitBilgileri.Add(taksit);
                }

                banka.TaksitBilgileri = taksitBilgileri;

                bankalarData.Add(banka);
            }

            return bankalarData;
        }
        #endregion

        #region NonCache
        public List<Banka> GetInstallmentCalculationDataByPrice(decimal fiyat)
        {
            var bankalar = this.FindBy(x => (x.AktifMi == true &&
                                             x.Taksit.Any(p => p.AktifMi == true)),
                                             false,
                                             new string[] { "Taksit" }).
                                             OrderBy(x => x.Sira).ToList();
            var bankalarData = new List<Banka>();

            foreach (var banka in bankalar)
            {
                var taksitBilgileri = new List<Taksit>();

                foreach (var taksit in banka.Taksit)
                {
                    if (taksit.TaksitSayisi > 0 && taksit.AktifMi)
                    {
                        decimal aylikTaksit = fiyat / (decimal)taksit.TaksitSayisi;
                        taksit.AylikOdeme = aylikTaksit + (aylikTaksit * taksit.FaizOrani);
                        taksit.ToplamTutar = taksit.AylikOdeme * taksit.TaksitSayisi;

                        taksitBilgileri.Add(taksit);
                    }                                  
                }

                banka.TaksitBilgileri = taksitBilgileri;

                bankalarData.Add(banka);
            }

            return bankalarData;
        }
        #endregion
    }
}
