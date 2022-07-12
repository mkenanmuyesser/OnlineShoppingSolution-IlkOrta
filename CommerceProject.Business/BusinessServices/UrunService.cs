using CommerceProject.Business.BusinessContracts;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using CommerceProject.Business.Custom;
using System;
using CommerceProject.Business.Helper.Cookie;
using CommerceProject.Business.Helper.PriceCalculation;

namespace CommerceProject.Business.BusinessServices
{
    public class UrunService : GenericRepository<Urun>, IUrunService
    {
        private readonly ISiparisDetayService siparisDetayService = new SiparisDetayService();

        #region Cache
        public List<Urun> GetAllActiveProductsFromCache(List<int> seciliKategoriler = null)
        {
            if (seciliKategoriler == null)
                seciliKategoriler = new List<int>();

            var aktifUrunler = this.FindByFromCache(x => x.AktifMi &&
                                                         x.UrunKategori.Any(p => seciliKategoriler.Contains(p.KategoriId)),
                                                        CacheDataObj.AktifUrunler,
                                                        new string[] { "UrunResim", "UrunKategori", "UrunYorum", "UrunNitelik", "UrunNitelik.Nitelik", "UrunOzellik", "Vergi", "Marka", "SiparisDetay" }).
                                                        AsEnumerable().
                                                        Select(x =>
                                                        {
                                                            var anaResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 1);
                                                            var thumbResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 2);
                                                            x.AnaResim = anaResim == null ? thumbResim : anaResim;
                                                            x.ThumbResim = thumbResim;
                                                            x.YorumSayisi = x.UrunYorum.Count(p => p.AktifMi);
                                                            x.YorumOrtalama = (x.UrunYorum.Sum(p => p.Puan) / (decimal)(x.UrunYorum.Any(p => p.AktifMi) ? x.UrunYorum.Count(p => p.AktifMi) : 1));

                                                            if (x.OzelFiyatAktifMi &&
                                                          (x.OzelFiyatBaslangicTarihi.HasValue && x.OzelFiyatBaslangicTarihi.Value <= DateTime.Now) &&
                                                          (x.OzelFiyatBitisTarihi.HasValue && x.OzelFiyatBitisTarihi >= DateTime.Now))
                                                                x.IndirimMiktari = x.OzelFiyat == null ? 0 : (100 - Convert.ToInt32((x.OzelFiyat.Value / x.EskiFiyat) * 100));
                                                            else
                                                                x.IndirimMiktari = (100 - Convert.ToInt32((x.Fiyat / x.EskiFiyat) * 100));

                                                            //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
                                                            //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
                                                            var priceDetail = PriceCalculationHelper.PriceDetail(x.Fiyat, x.Vergi.VergiOrani);
                                                            x.KdvDahilTutar = priceDetail.KdvDahilTutar;
                                                            x.KdvHaricTutar = priceDetail.KdvHaricTutar;
                                                            x.KdvTutar = priceDetail.KdvTutar;

                                                            return x;
                                                        }).OrderBy(x => x.Sira).ToList();
            return aktifUrunler;
        }

        public List<Urun> GetAllActiveRelatedProductsFromCache(int urunId)
        {
            var aktifUrunler = this.FindByFromCache(x => x.AktifMi &&
                                                         x.UrunId != urunId &&
                                                         x.IlgiliUrun.Any(p => p.UrunId1 == urunId || p.UrunId2 == urunId),
                                                         CacheDataObj.AktifUrunler,
                                                         new string[] { "UrunResim", "IlgiliUrun" }).
                                                         AsEnumerable().
                                                         Select(x =>
                                                         {
                                                             var anaResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 1);
                                                             var thumbResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 2);
                                                             x.AnaResim = anaResim == null ? thumbResim : anaResim;
                                                             x.ThumbResim = thumbResim;                                                           

                                                             if (x.OzelFiyatAktifMi &&
                                                           (x.OzelFiyatBaslangicTarihi.HasValue && x.OzelFiyatBaslangicTarihi.Value <= DateTime.Now) &&
                                                           (x.OzelFiyatBitisTarihi.HasValue && x.OzelFiyatBitisTarihi >= DateTime.Now))
                                                                 x.IndirimMiktari = x.OzelFiyat == null ? 0 : (100 - Convert.ToInt32((x.OzelFiyat.Value / x.EskiFiyat) * 100));
                                                             else
                                                                 x.IndirimMiktari = (100 - Convert.ToInt32((x.Fiyat / x.EskiFiyat) * 100));

                                                             //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
                                                             //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
                                                             var priceDetail = PriceCalculationHelper.PriceDetail(x.Fiyat, x.Vergi.VergiOrani);
                                                             x.KdvDahilTutar = priceDetail.KdvDahilTutar;
                                                             x.KdvHaricTutar = priceDetail.KdvHaricTutar;
                                                             x.KdvTutar = priceDetail.KdvTutar;

                                                             return x;
                                                         }).OrderBy(x => x.Sira).ToList();
            return aktifUrunler;
        }

        public List<Urun> GetAllActiveProductsByPropertyFromCache(int ozellikId)
        {
            var aktifUrunler = this.FindByFromCache(x => x.AktifMi &&
                                                         x.UrunOzellik.Any(p => p.AktifMi && p.OzellikId == ozellikId),
                                                        CacheDataObj.AktifUrunler,
                                                        new string[] { "UrunResim", "UrunKategori", "UrunYorum", "UrunNitelik", "UrunNitelik.Nitelik", "UrunOzellik", "Vergi", "Marka" }).
                                                        AsEnumerable().
                                                        Select(x =>
                                                        {
                                                            var anaResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 1);
                                                            var thumbResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 2);
                                                            x.AnaResim = anaResim == null ? thumbResim : anaResim;
                                                            x.ThumbResim = thumbResim;
                                                            x.YorumSayisi = x.UrunYorum.Count(p => p.AktifMi);
                                                            x.YorumOrtalama = (x.UrunYorum.Sum(p => p.Puan) / (decimal)(x.UrunYorum.Any(p => p.AktifMi) ? x.UrunYorum.Count(p => p.AktifMi) : 1));

                                                            if (x.OzelFiyatAktifMi &&
                                                          (x.OzelFiyatBaslangicTarihi.HasValue && x.OzelFiyatBaslangicTarihi.Value <= DateTime.Now) &&
                                                          (x.OzelFiyatBitisTarihi.HasValue && x.OzelFiyatBitisTarihi >= DateTime.Now))
                                                                x.IndirimMiktari = x.OzelFiyat == null ? 0 : (100 - Convert.ToInt32((x.OzelFiyat.Value / x.EskiFiyat) * 100));
                                                            else
                                                                x.IndirimMiktari = (100 - Convert.ToInt32((x.Fiyat / x.EskiFiyat) * 100));

                                                            //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
                                                            //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
                                                            var priceDetail = PriceCalculationHelper.PriceDetail(x.Fiyat, x.Vergi.VergiOrani);
                                                            x.KdvDahilTutar = priceDetail.KdvDahilTutar;
                                                            x.KdvHaricTutar = priceDetail.KdvHaricTutar;
                                                            x.KdvTutar = priceDetail.KdvTutar;

                                                            return x;
                                                        }).OrderBy(x => x.Sira).ToList();
            return aktifUrunler;
        }

        public List<Urun> SearchProductsFromCache(string searchParameter)
        {
            var aktifUrunler = this.FindByFromCache(x => x.AktifMi &&
                                                         x.Adi.Contains(searchParameter),
                                                         CacheDataObj.AktifUrunler,
                                                         new string[] { "UrunResim", "UrunKategori", "UrunYorum", "UrunNitelik", "UrunNitelik.Nitelik", "UrunOzellik", "Vergi", "Marka" }).
                                                         AsEnumerable().
                                                         Select(x =>
                                                         {
                                                             var anaResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 1);
                                                             var thumbResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 2);
                                                             x.AnaResim = anaResim == null ? thumbResim : anaResim;
                                                             x.ThumbResim = thumbResim;
                                                             x.YorumSayisi = x.UrunYorum.Count(p => p.AktifMi);
                                                             x.YorumOrtalama = (x.UrunYorum.Sum(p => p.Puan) / (decimal)(x.UrunYorum.Any(p => p.AktifMi) ? x.UrunYorum.Count(p => p.AktifMi) : 1));

                                                             if (x.OzelFiyatAktifMi &&
                                                            (x.OzelFiyatBaslangicTarihi.HasValue && x.OzelFiyatBaslangicTarihi.Value <= DateTime.Now) &&
                                                            (x.OzelFiyatBitisTarihi.HasValue && x.OzelFiyatBitisTarihi >= DateTime.Now))
                                                                 x.IndirimMiktari = x.OzelFiyat == null ? 0 : (100 - Convert.ToInt32((x.OzelFiyat.Value / x.EskiFiyat) * 100));
                                                             else
                                                                 x.IndirimMiktari = (100 - Convert.ToInt32((x.Fiyat / x.EskiFiyat) * 100));

                                                             //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
                                                             //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
                                                             var priceDetail = PriceCalculationHelper.PriceDetail(x.Fiyat, x.Vergi.VergiOrani);
                                                             x.KdvDahilTutar = priceDetail.KdvDahilTutar;
                                                             x.KdvHaricTutar = priceDetail.KdvHaricTutar;
                                                             x.KdvTutar = priceDetail.KdvTutar;

                                                             return x;
                                                         }).OrderBy(x => x.Sira).ToList();
            return aktifUrunler;
        }

        public Urun GetSingleProductFromCache(int urunId)
        {
            var urun = this.GetSingleFromCache(CacheDataObj.AktifUrunler, (x => x.UrunId == urunId), new string[] { "UrunResim", "UrunKategori", "UrunYorum", "UrunNitelik", "UrunNitelik.Nitelik", "UrunOzellik", "Vergi", "Marka" });

            var anaResim = urun.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 1);
            var thumbResim = urun.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 2);
            urun.AnaResim = anaResim == null ? thumbResim : anaResim;
            urun.ThumbResim = thumbResim;
            urun.YorumSayisi = urun.UrunYorum.Count(p => p.AktifMi);
            urun.YorumOrtalama = (urun.UrunYorum.Sum(p => p.Puan) / (decimal)(urun.UrunYorum.Any(p => p.AktifMi) ? urun.UrunYorum.Count(p => p.AktifMi) : 1));

            if (urun.OzelFiyatAktifMi &&
            (urun.OzelFiyatBaslangicTarihi.HasValue && urun.OzelFiyatBaslangicTarihi.Value <= DateTime.Now) &&
            (urun.OzelFiyatBitisTarihi.HasValue && urun.OzelFiyatBitisTarihi >= DateTime.Now))
                urun.IndirimMiktari = urun.OzelFiyat == null ? 0 : (100 - Convert.ToInt32((urun.OzelFiyat.Value / urun.EskiFiyat) * 100));
            else
                urun.IndirimMiktari = (100 - Convert.ToInt32((urun.Fiyat / urun.EskiFiyat) * 100));

            //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
            //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
            var priceDetail = PriceCalculationHelper.PriceDetail(urun.Fiyat, urun.Vergi.VergiOrani);
            urun.KdvDahilTutar = priceDetail.KdvDahilTutar;
            urun.KdvHaricTutar = priceDetail.KdvHaricTutar;
            urun.KdvTutar = priceDetail.KdvTutar;

            return urun;
        }
        #endregion

        #region NonCache
        public List<Urun> GetAllActiveProducts(List<int> seciliKategoriler = null)
        {
            if (seciliKategoriler == null)
                seciliKategoriler = new List<int>();

            var aktifUrunler = this.FindBy(x => x.AktifMi &&
                                                x.UrunKategori.Any(p => seciliKategoriler.Contains(p.KategoriId)),
                                                true,
                                                new string[] { "UrunResim", "UrunKategori", "UrunYorum", "UrunNitelik", "UrunNitelik.Nitelik", "UrunOzellik", "Vergi", "Marka", "SiparisDetay" }).
                                                ToList().
                                                Select(x =>
                                                {
                                                    var anaResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 1);
                                                    var thumbResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 2);
                                                    x.AnaResim = anaResim == null ? thumbResim : anaResim;
                                                    x.ThumbResim = thumbResim;
                                                    x.YorumSayisi = x.UrunYorum.Count(p => p.AktifMi);
                                                    x.YorumOrtalama = (x.UrunYorum.Sum(p => p.Puan) / (decimal)(x.UrunYorum.Any(p => p.AktifMi) ? x.UrunYorum.Count(p => p.AktifMi) : 1));

                                                    decimal hesaplananFiyat = 0;
                                                    if (x.OzelFiyatAktifMi &&
                                                        (x.OzelFiyatBaslangicTarihi.HasValue && x.OzelFiyatBaslangicTarihi.Value <= DateTime.Now) &&
                                                        (x.OzelFiyatBitisTarihi.HasValue && x.OzelFiyatBitisTarihi >= DateTime.Now))
                                                    {
                                                        hesaplananFiyat = x.OzelFiyat.HasValue ? 0 : x.OzelFiyat.Value;
                                                        x.IndirimMiktari = x.OzelFiyat == null ? 0 : (100 - Convert.ToInt32((x.OzelFiyat.Value / x.EskiFiyat) * 100));
                                                    }
                                                    else
                                                    {
                                                        hesaplananFiyat = x.Fiyat;
                                                        x.IndirimMiktari = (100 - Convert.ToInt32((x.Fiyat / x.EskiFiyat) * 100));
                                                    }

                                                    //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
                                                    //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
                                                    var priceDetail = PriceCalculationHelper.PriceDetail(hesaplananFiyat, x.Vergi.VergiOrani);
                                                    x.KdvDahilTutar = priceDetail.KdvDahilTutar;
                                                    x.KdvHaricTutar = priceDetail.KdvHaricTutar;
                                                    x.KdvTutar = priceDetail.KdvTutar;

                                                    return x;
                                                }).OrderBy(x => x.Sira).ToList();
            return aktifUrunler;
        }

        public List<Urun> GetAllActiveRelatedProducts(int urunId)
        {
            var aktifUrunler = this.FindBy(x => x.AktifMi &&
                                                x.UrunId != urunId &&
                                                x.IlgiliUrun.Any(p => p.UrunId1 == urunId || p.UrunId2 == urunId),
                                                true,
                                                new string[] { "UrunResim",  "IlgiliUrun" }).
                                                AsEnumerable().
                                                Select(x =>
                                                {
                                                    var anaResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 1);
                                                    var thumbResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 2);
                                                    x.AnaResim = anaResim == null ? thumbResim : anaResim;
                                                    x.ThumbResim = thumbResim;                                                  

                                                    if (x.OzelFiyatAktifMi &&
                                                (x.OzelFiyatBaslangicTarihi.HasValue && x.OzelFiyatBaslangicTarihi.Value <= DateTime.Now) &&
                                                (x.OzelFiyatBitisTarihi.HasValue && x.OzelFiyatBitisTarihi >= DateTime.Now))
                                                        x.IndirimMiktari = x.OzelFiyat == null ? 0 : (100 - Convert.ToInt32((x.OzelFiyat.Value / x.EskiFiyat) * 100));
                                                    else
                                                        x.IndirimMiktari = (100 - Convert.ToInt32((x.Fiyat / x.EskiFiyat) * 100));

                                                    //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
                                                    //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
                                                    var priceDetail = PriceCalculationHelper.PriceDetail(x.Fiyat, x.Vergi.VergiOrani);
                                                    x.KdvDahilTutar = priceDetail.KdvDahilTutar;
                                                    x.KdvHaricTutar = priceDetail.KdvHaricTutar;
                                                    x.KdvTutar = priceDetail.KdvTutar;

                                                    return x;
                                                }).OrderBy(x => x.Sira).ToList();
            return aktifUrunler;
        }

        public List<Urun> GetAllActiveProductsByProperty(int ozellikId)
        {
            var aktifUrunler = this.FindBy(x => x.AktifMi &&
                                                x.UrunOzellik.Any(p => p.AktifMi && p.OzellikId == ozellikId),
                                                true,
                                                new string[] { "UrunResim", "UrunKategori", "UrunYorum", "UrunNitelik", "UrunNitelik.Nitelik", "UrunOzellik", "Vergi", "Marka" }).
                                                ToList().
                                                Select(x =>
                                                {
                                                    var anaResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 1);
                                                    var thumbResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 2);
                                                    x.AnaResim = anaResim == null ? thumbResim : anaResim;
                                                    x.ThumbResim = thumbResim;
                                                    x.YorumSayisi = x.UrunYorum.Count(p => p.AktifMi);
                                                    x.YorumOrtalama = (x.UrunYorum.Sum(p => p.Puan) / (decimal)(x.UrunYorum.Any(p => p.AktifMi) ? x.UrunYorum.Count(p => p.AktifMi) : 1));

                                                    if (x.OzelFiyatAktifMi &&
                                                                                                            (x.OzelFiyatBaslangicTarihi.HasValue && x.OzelFiyatBaslangicTarihi.Value <= DateTime.Now) &&
                                                                                                            (x.OzelFiyatBitisTarihi.HasValue && x.OzelFiyatBitisTarihi >= DateTime.Now))
                                                        x.IndirimMiktari = x.OzelFiyat == null ? 0 : (100 - Convert.ToInt32((x.OzelFiyat.Value / x.EskiFiyat) * 100));
                                                    else
                                                        x.IndirimMiktari = (100 - Convert.ToInt32((x.Fiyat / x.EskiFiyat) * 100));

                                                    //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
                                                    //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
                                                    var priceDetail = PriceCalculationHelper.PriceDetail(x.Fiyat, x.Vergi.VergiOrani);
                                                    x.KdvDahilTutar = priceDetail.KdvDahilTutar;
                                                    x.KdvHaricTutar = priceDetail.KdvHaricTutar;
                                                    x.KdvTutar = priceDetail.KdvTutar;

                                                    return x;
                                                }).OrderBy(x => x.Sira).ToList();
            return aktifUrunler;
        }

        public List<Urun> SearchProducts(string searchParameter)
        {
            var aktifUrunler = this.FindBy(x => x.AktifMi &&
                                                x.Adi.Contains(searchParameter),
                                                true,
                                                new string[] { "UrunResim", "UrunKategori", "UrunYorum", "UrunNitelik", "UrunNitelik.Nitelik", "UrunOzellik", "Vergi", "Marka" }).
                                                AsEnumerable().
                                                Select(x =>
                                                {
                                                    var anaResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 1);
                                                    var thumbResim = x.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 2);
                                                    x.AnaResim = anaResim == null ? thumbResim : anaResim;
                                                    x.ThumbResim = thumbResim;
                                                    x.YorumSayisi = x.UrunYorum.Count(p => p.AktifMi);
                                                    x.YorumOrtalama = (x.UrunYorum.Sum(p => p.Puan) / (decimal)(x.UrunYorum.Any(p => p.AktifMi) ? x.UrunYorum.Count(p => p.AktifMi) : 1));

                                                    if (x.OzelFiyatAktifMi &&
                                                                                                        (x.OzelFiyatBaslangicTarihi.HasValue && x.OzelFiyatBaslangicTarihi.Value <= DateTime.Now) &&
                                                                                                        (x.OzelFiyatBitisTarihi.HasValue && x.OzelFiyatBitisTarihi >= DateTime.Now))
                                                        x.IndirimMiktari = x.OzelFiyat == null ? 0 : (100 - Convert.ToInt32((x.OzelFiyat.Value / x.EskiFiyat) * 100));
                                                    else
                                                        x.IndirimMiktari = (100 - Convert.ToInt32((x.Fiyat / x.EskiFiyat) * 100));

                                                    //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
                                                    //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
                                                    var priceDetail = PriceCalculationHelper.PriceDetail(x.Fiyat, x.Vergi.VergiOrani);
                                                    x.KdvDahilTutar = priceDetail.KdvDahilTutar;
                                                    x.KdvHaricTutar = priceDetail.KdvHaricTutar;
                                                    x.KdvTutar = priceDetail.KdvTutar;

                                                    return x;
                                                }).OrderBy(x => x.Sira).ToList();
            return aktifUrunler;
        }

        public Urun GetSingleProduct(int urunId)
        {
            var urun = this.GetSingle(x => x.UrunId == urunId,
                                        true,
                                        new string[] { "UrunResim", "UrunKategori", "UrunYorum", "UrunNitelik", "UrunNitelik.Nitelik", "UrunOzellik", "Vergi", "Marka" });

            var anaResim = urun.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 1);
            var thumbResim = urun.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 2);
            urun.AnaResim = anaResim == null ? thumbResim : anaResim;
            urun.ThumbResim = thumbResim;
            urun.YorumSayisi = urun.UrunYorum.Count(p => p.AktifMi);
            urun.YorumOrtalama = (urun.UrunYorum.Sum(p => p.Puan) / (decimal)(urun.UrunYorum.Any(p => p.AktifMi) ? urun.UrunYorum.Count(p => p.AktifMi) : 1));

            if (urun.OzelFiyatAktifMi &&
               (urun.OzelFiyatBaslangicTarihi.HasValue && urun.OzelFiyatBaslangicTarihi.Value <= DateTime.Now) &&
               (urun.OzelFiyatBitisTarihi.HasValue && urun.OzelFiyatBitisTarihi >= DateTime.Now))
                urun.IndirimMiktari = urun.OzelFiyat == null ? 0 : (100 - Convert.ToInt32((urun.OzelFiyat.Value / urun.EskiFiyat) * 100));
            else
                urun.IndirimMiktari = (100 - Convert.ToInt32((urun.Fiyat / urun.EskiFiyat) * 100));

            //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
            //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
            var priceDetail = PriceCalculationHelper.PriceDetail(urun.Fiyat, urun.Vergi.VergiOrani);
            urun.KdvDahilTutar = priceDetail.KdvDahilTutar;
            urun.KdvHaricTutar = priceDetail.KdvHaricTutar;
            urun.KdvTutar = priceDetail.KdvTutar;

            return urun;
        }

        public int GetProductsTotalSales(int urunId)
        {
            var siparisler = siparisDetayService.FindBy(x => x.AktifMi && x.UrunId == urunId, true);

            if (siparisler.Any())
                return siparisler.Sum(x => x.Adet);
            else
                return 0;
        }
        #endregion
    }
}
