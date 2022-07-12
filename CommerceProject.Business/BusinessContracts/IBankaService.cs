using CommerceProject.Business.Custom;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.BusinessContracts
{
    public interface IBankaService : IGenericRepository<Banka>
    {
        #region Cache
        List<Banka> GetInstallmentCalculationDataByPriceFromCache(decimal fiyat);
        #endregion

        #region NonCache
        List<Banka> GetInstallmentCalculationDataByPrice(decimal fiyat);
        #endregion
    }
}
