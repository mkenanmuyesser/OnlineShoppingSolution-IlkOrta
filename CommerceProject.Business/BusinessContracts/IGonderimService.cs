using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.BusinessContracts
{
    public interface IGonderimService : IGenericRepository<Gonderim>
    {
        #region Cache
        List<Gonderim> GetAllActiveShippingsFromCache();
        #endregion

        #region NonCache       
        List<Gonderim> GetAllActiveShippings();        
        #endregion
    }
}
