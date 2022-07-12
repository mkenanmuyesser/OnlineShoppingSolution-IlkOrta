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
    public class SSSService : GenericRepository<SSS>, ISSSService
    {
        #region Cache
        public List<SSS> GetAllActiveSSSFromCache()
        {
            var aktifSSS = this.FindByFromCache(x => (x.AktifMi == true), CacheDataObj.AktifSSS);
            return aktifSSS;
        }
        #endregion

        #region Cache
        public List<SSS> GetAllActiveSSS()
        {
            var aktifSSS = this.FindBy(x => (x.AktifMi == true), false).ToList();
            return aktifSSS;
        }
        #endregion
    }
}
