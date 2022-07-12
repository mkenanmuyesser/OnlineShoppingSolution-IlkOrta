using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.BusinessContracts
{
    public interface IAdresIlService : IGenericRepository<AdresIl>
    {
        #region Cache
        List<AdresIl> GetAllActiveCitiesFromCache();
        #endregion

        #region NonCache
        List<AdresIl> GetAllActiveCities();       
        #endregion
    }
}
