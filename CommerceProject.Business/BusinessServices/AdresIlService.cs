using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using CommerceProject.Business.Custom;

namespace CommerceProject.Business.BusinessServices
{
    public class AdresIlService : GenericRepository<AdresIl>, IAdresIlService
    {
        #region Cache

        public List<AdresIl> GetAllActiveCitiesFromCache()
        {
            var aktifIller = this.FindByFromCache(x => x.AktifMi, CacheDataObj.AktifAdresIller);
            return aktifIller;
        }

        #endregion

        #region NonCache

        public List<AdresIl> GetAllActiveCities()
        {
            var aktifIller = this.FindBy(x => x.AktifMi, false).ToList();
            return aktifIller;
        }

        #endregion
    }
}
