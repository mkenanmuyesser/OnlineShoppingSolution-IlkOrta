using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.BusinessContracts
{
    public interface IMarkaService : IGenericRepository<Marka>
    {
        #region Cache
        List<Marka> GetAllActiveBrandsFromCache();
        #endregion

        #region NonCache
        List<Marka> GetAllActiveBrands();        
        #endregion
    }
}
