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
    public class GonderimService : GenericRepository<Gonderim>, IGonderimService
    {
        #region Cache
        public List<Gonderim> GetAllActiveShippingsFromCache()
        {
            var aktifGonderimler = this.FindByFromCache(x => x.AktifMi, CacheDataObj.AktifGonderimler).OrderBy(x => x.Sira).ToList();

            return aktifGonderimler;
        }
        #endregion

        #region NonCache       
        public List<Gonderim> GetAllActiveShippings()
        {
            var aktifGonderimler = this.FindBy(x => x.AktifMi).OrderBy(x => x.Sira).ToList();

            return aktifGonderimler;
        }
        #endregion
    }
}
