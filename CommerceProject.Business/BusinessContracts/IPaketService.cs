using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.BusinessContracts
{
    public interface IPaketService : IGenericRepository<Paket>
    {
        #region Cache
        List<Paket> GetAllActivePackagesFromCache(List<int> seciliKategoriler = null);

        List<Paket> GetAllActiveRelatedPackagesFromCache(List<int> seciliKategoriler = null);

        List<Paket> SearchPackagesFromCache(string searchParameter);

        Paket GetSinglePackageFromCache(int paketId, List<int> urunler = null);
        #endregion

        #region NonCache
        List<Paket> GetAllActivePackages(List<int> seciliKategoriler = null);

        List<Paket> GetAllActiveRelatedPackages(List<int> seciliKategoriler = null);

        List<Paket> SearchPackages(string searchParameter);

        Paket GetSinglePackage(int paketId, List<int> urunler = null);
        #endregion
    }
}
