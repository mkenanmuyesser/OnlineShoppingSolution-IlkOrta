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
    public class MarkaService : GenericRepository<Marka>, IMarkaService
    {
        #region Cache
        public List<Marka> GetAllActiveBrandsFromCache()
        {
            var markalar = this.FindByFromCache(x => (x.AktifMi == true &&
                                                        x.AnaSayfadaGosterilsinMi == true), CacheDataObj.AktifMarkalar).OrderBy(x => x.Sira).ToList();
            return markalar;
        }
        #endregion

        #region NonCache
        public List<Marka> GetAllActiveBrands()
        {
            var markalar = this.FindBy(x => (x.AktifMi == true &&
                                            x.AnaSayfadaGosterilsinMi == true), false).OrderBy(x => x.Sira).ToList();
            return markalar;
        }
        #endregion
    }
}
