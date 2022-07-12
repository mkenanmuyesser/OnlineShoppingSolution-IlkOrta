using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.BusinessContracts
{
    public interface IAdresIlceService : IGenericRepository<AdresIlce>
    {
        #region Cache
        List<AdresIlce> GetAllActiveTownsFromCache();

        List<AdresIlce> GetAllActiveTownsByCityIdFromCache(int adresIlId);
        #endregion

        #region NonCache
        List<AdresIlce> GetAllActiveCities();

        List<AdresIlce> GetAllActiveTownsByCityId(int adresIlId);       
        #endregion
    }
}
