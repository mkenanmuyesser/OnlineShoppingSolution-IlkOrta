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
    public interface ISSSService : IGenericRepository<SSS>
    {
        #region Cache
        List<SSS> GetAllActiveSSSFromCache();
        #endregion

        #region Cache
        List<SSS> GetAllActiveSSS();
        #endregion
    }
}
