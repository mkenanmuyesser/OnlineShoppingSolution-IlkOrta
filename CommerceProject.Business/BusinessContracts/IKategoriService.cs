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
    public interface IKategoriService : IGenericRepository<Kategori>
    {
        #region Cache
        Kategori GetCategoryFromCache(int kategoriId);

        List<Kategori> GetAllActiveCategoriesWithNestedFromCache(int? kategoriId = null);

        List<Kategori> GetMenuCategoriesFromCache(int ozellikId, int? ustKategoriId = null);

        List<Kategori> GetCategoriesByPropertyFromCache(int ozellikId);
        #endregion

        #region NonCache
        Kategori GetCategory(int kategoriId);

        List<Kategori> GetAllActiveCategoriesWithNested(int? kategoriId = null);

        List<Kategori> GetMenuCategories(int ozellikId, int? ustKategoriId = null);

        List<Kategori> GetCategoriesByProperty(int ozellikId);
        #endregion
    }
}
