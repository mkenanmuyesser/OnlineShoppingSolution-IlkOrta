using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using CommerceProject.Business.Custom;

namespace CommerceProject.Business.BusinessServices
{
    public class AdresIlceService : GenericRepository<AdresIlce>, IAdresIlceService
    {
        #region Cache

        public List<AdresIlce> GetAllActiveTownsFromCache()
        {
            var aktifIlceler = this.FindByFromCache(x => x.AktifMi, CacheDataObj.AktifAdresIlceler);
            return aktifIlceler;
        }

        public List<AdresIlce> GetAllActiveTownsByCityIdFromCache(int adresIlId)
        {
            var aktifIlceler = GetAllActiveTownsFromCache().Where(x => x.AdresIlId == adresIlId).ToList();
            return aktifIlceler;
        }

        #endregion

        #region NonCache

        public List<AdresIlce> GetAllActiveCities()
        {
            var aktifIlceler = this.FindBy(x => x.AktifMi, false).ToList();
            return aktifIlceler;
        }

        public List<AdresIlce> GetAllActiveTownsByCityId(int adresIlId)
        {
            var aktifIlceler = this.FindBy(x => x.AktifMi && x.AdresIlId == adresIlId, false).ToList();
            return aktifIlceler;
        }

        #endregion
    }
}
