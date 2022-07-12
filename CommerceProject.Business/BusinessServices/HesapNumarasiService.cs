using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using CommerceProject.Business.Custom;

namespace CommerceProject.Business.BusinessServices
{
    public class HesapNumarasiService : GenericRepository<HesapNumarasi>, IHesapNumarasiService
    {
        #region Cache
        public List<HesapNumarasi> GetAllActiveBankAccountNumbersFromCache()
        {
            var aktifHesapNumaralari = this.FindByFromCache(x => (x.AktifMi == true), CacheDataObj.AktifHesapNumaralari, new string[] { "Banka" });
            return aktifHesapNumaralari;
        }
        #endregion

        #region NonCache
        public List<HesapNumarasi> GetAllActiveBankAccountNumbers()
        {
            var aktifHesapNumaralari = this.FindBy(x => (x.AktifMi == true), false, new string[] { "Banka" }).ToList();
            return aktifHesapNumaralari;
        }
        #endregion
    }
}
