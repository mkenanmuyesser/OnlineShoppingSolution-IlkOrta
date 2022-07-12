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
    public interface IUrunService : IGenericRepository<Urun>
    {
        #region Cache
        List<Urun> GetAllActiveProductsFromCache(List<int> seciliKategoriler = null);

        List<Urun> GetAllActiveRelatedProductsFromCache(int urunId);

        List<Urun> GetAllActiveProductsByPropertyFromCache(int ozellikId);

        List<Urun> SearchProductsFromCache(string searchParameter);

        Urun GetSingleProductFromCache(int urunId);
        #endregion

        #region NonCache
        List<Urun> GetAllActiveProducts(List<int> seciliKategoriler = null);

        List<Urun> GetAllActiveRelatedProducts(int urunId);

        List<Urun> GetAllActiveProductsByProperty(int ozellikId);

        List<Urun> SearchProducts(string searchParameter);

        Urun GetSingleProduct(int urunId);

        int GetProductsTotalSales(int urunId);
        #endregion
    }
}
