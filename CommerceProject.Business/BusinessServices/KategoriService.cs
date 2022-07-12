using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using CommerceProject.Business.Custom;

namespace CommerceProject.Business.BusinessServices
{
    public class KategoriService : GenericRepository<Kategori>, IKategoriService
    {
        private IUrunService UrunService = new UrunService();

        #region Cache
        public Kategori GetCategoryFromCache(int kategoriId)
        {
            var kategori = this.GetSingleFromCache(CacheDataObj.AktifKategoriler,
                                                   x => x.KategoriId == kategoriId,
                                                   new string[] { "UrunKategori", "UrunKategori.Urun", "UrunKategori.Urun.UrunResim", "KategoriOzellik" });
            kategori.SeciliKategori = true;
            kategori.AltKategoriler = this.GetAllActiveCategoriesWithNestedFromCache(kategoriId);

            return kategori;
        }

        public List<Kategori> GetAllActiveCategoriesWithNestedFromCache(int? kategoriId = null)
        {
            var kategoriler = this.FindByFromCache(x => x.AktifMi &&
                                                        (kategoriId == null &&
                                                            (x.UstKategoriId == 0 || x.UstKategoriId == null)) ||
                                                            (kategoriId != null && (x.UstKategoriId == kategoriId)
                                                        ),
                                                        CacheDataObj.AktifKategoriler,
                                                        new string[] { "UrunKategori", "UrunKategori.Urun", "UrunKategori.Urun.UrunResim", "KategoriOzellik" }).
                                                        OrderBy(x => x.Sira).
                                                        Select(x =>
                                                        {
                                                            x.SeciliKategori = false;
                                                            x.AltKategoriler = this.GetAllActiveCategoriesWithNestedFromCache(x.KategoriId);
                                                            return x;
                                                        }).ToList();
            return kategoriler;
        }

        public List<Kategori> GetMenuCategoriesFromCache(int ozellikId, int? ustKategoriId = null)
        {
            int menuSira = 0;
            if (ustKategoriId == null)
                ustKategoriId = 0;

            var kategoriler = this.FindByFromCache(x => x.AktifMi &&
                                                        x.KategoriOzellik.Any(p => p.AktifMi && p.OzellikId == ozellikId) &&
                                                        (
                                                            (ustKategoriId == 0 && (x.UstKategoriId == null)) ||
                                                            (ustKategoriId != 0 && (x.UstKategoriId == ustKategoriId))
                                                        ),
                                                        CacheDataObj.AktifKategoriler,
                                                        new string[] { "UrunKategori", "UrunKategori.Urun", "UrunKategori.Urun.UrunResim", "KategoriOzellik" }).
                                                        Where(x => x.AktifMi).
                                                        OrderBy(x => x.Sira).
                                                        Select(x =>
                                                        {
                                                            x.SeciliKategori = false;
                                                            x.MenuSira = menuSira++;
                                                            x.AltKategoriler = this.GetMenuCategoriesFromCache(ozellikId, x.KategoriId);
                                                            return x;
                                                        }).ToList();
            return kategoriler;
        }

        public List<Kategori> GetCategoriesByPropertyFromCache(int ozellikId)
        {
            int menuSira = 0;
            var kategoriler = this.FindByFromCache(x => x.AktifMi &&
                                                        x.KategoriOzellik.Any(p => p.AktifMi && p.OzellikId == ozellikId),
                                                        CacheDataObj.AktifKategoriler,
                                                        new string[] { "KategoriOzellik" }).
                                                        OrderBy(x => x.Sira).
                                                        Select(x =>
                                                        {
                                                            x.SeciliKategori = false;
                                                            x.MenuSira = menuSira++;
                                                            x.AltKategoriler = this.GetMenuCategoriesFromCache(x.KategoriId);
                                                            return x;
                                                        }).ToList();
            return kategoriler;
        }
        #endregion

        #region NonCache
        public Kategori GetCategory(int kategoriId)
        {
            var kategori = this.GetSingle(x => x.KategoriId == kategoriId,
                                               true,
                                               new string[] { "UrunKategori" });

            kategori.SeciliKategori = true;
            kategori.AltKategoriler = this.GetAllActiveCategoriesWithNested(kategoriId);

            return kategori;
        }

        public List<Kategori> GetAllActiveSimpleCategoriesWithNested(int kategoriId)
        {
            var kategoriler = this.FindBy(x => x.AktifMi &&
                                               x.UstKategoriId == kategoriId,
                                               true,
                                               new string[] { "UrunKategori" }).
                                               OrderBy(x => x.Sira).
                                               ToList().
                                               Select(x =>
                                               {
                                                   x.SeciliKategori = false;
                                                   x.AltKategoriler = this.GetAllActiveSimpleCategoriesWithNested(x.KategoriId);
                                                   return x;
                                               }).ToList();
            return kategoriler;
        }

        public List<Kategori> GetAllActiveCategoriesWithNested(int? kategoriId = null)
        {
            var kategoriler = this.FindBy(x => x.AktifMi &&
                                               (kategoriId == null &&
                                                   (x.UstKategoriId == 0 || x.UstKategoriId == null)) ||
                                                   (kategoriId != null && (x.UstKategoriId == kategoriId)
                                               ),
                                               true,
                                               new string[] { "UrunKategori", "UrunKategori.Urun", "UrunKategori.Urun.UrunResim", "KategoriOzellik" }).
                                               OrderBy(x => x.Sira).
                                               ToList().
                                               Select(x =>
                                               {
                                                   x.SeciliKategori = false;
                                                   x.AltKategoriler = this.GetAllActiveCategoriesWithNested(x.KategoriId);
                                                   return x;
                                               }).ToList();
            return kategoriler;
        }

        public List<Kategori> GetMenuCategories(int ozellikId, int? ustKategoriId = null)
        {
            int menuSira = 0;
            if (ustKategoriId == null)
                ustKategoriId = 0;

            var kategoriler = this.FindBy(x => x.AktifMi &&
                                               x.KategoriOzellik.Any(p => p.AktifMi && p.OzellikId == ozellikId) &&
                                               (
                                                    (ustKategoriId == 0 && (x.UstKategoriId == null)) ||
                                                    (ustKategoriId != 0 && (x.UstKategoriId == ustKategoriId))
                                               ),
                                               true,
                                               new string[] { "KategoriOzellik" }).
                                               OrderBy(x => x.Sira).
                                               ToList().
                                               Select(x =>
                                               {
                                                   x.SeciliKategori = false;
                                                   x.MenuSira = menuSira++;
                                                   x.AltKategoriler = this.GetMenuCategories(ozellikId, x.KategoriId);
                                                   return x;
                                               }).ToList();
            return kategoriler;
        }

        public List<Kategori> GetCategoriesByProperty(int ozellikId)
        {
            int menuSira = 0;
            var kategoriler = this.FindBy(x => x.AktifMi &&
                                               x.KategoriOzellik.Any(p => p.AktifMi && p.OzellikId == ozellikId),
                                               true,
                                               new string[] { "UrunKategori", "UrunKategori.Urun", "UrunKategori.Urun.UrunResim", "KategoriOzellik" }).
                                               OrderBy(x => x.Sira).
                                               ToList().
                                               Select(x =>
                                               {
                                                   x.SeciliKategori = false;
                                                   x.MenuSira = menuSira++;
                                                   x.AltKategoriler = this.GetMenuCategories(x.KategoriId);
                                                   return x;
                                               }).ToList();
            return kategoriler;
        }
        #endregion
    }
}
