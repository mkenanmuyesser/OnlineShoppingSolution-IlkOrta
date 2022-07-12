using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using CommerceProject.Business.Custom;

namespace CommerceProject.Business.BusinessServices
{
    public class SiparisOdemeTipService : GenericRepository<SiparisOdemeTip>, ISiparisOdemeTipService
    {
        #region Cache
        public List<SiparisOdemeTip> GetAllActiveOrderPaymentTypesFromCache()
        {
            var aktifSiparisOdemeTipler = this.FindByFromCache(x => x.AktifMi, CacheDataObj.AktifSiparisOdemeTipleri).OrderBy(x => x.Sira).ToList();

            return aktifSiparisOdemeTipler;
        }
        #endregion

        #region NonCache       
        public List<SiparisOdemeTip> GetAllActiveOrderPaymentTypes()
        {
            var aktifSiparisOdemeTipler = this.FindBy(x => x.AktifMi).OrderBy(x => x.Sira).ToList();

            return aktifSiparisOdemeTipler;
        }
        #endregion
    }
}
