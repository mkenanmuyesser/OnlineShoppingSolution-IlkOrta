using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.BusinessContracts
{
    public interface ISiparisOdemeTipService : IGenericRepository<SiparisOdemeTip>
    {
        #region Cache
        List<SiparisOdemeTip> GetAllActiveOrderPaymentTypesFromCache();
        #endregion

        #region NonCache       
        List<SiparisOdemeTip> GetAllActiveOrderPaymentTypes();        
        #endregion
    }
}
