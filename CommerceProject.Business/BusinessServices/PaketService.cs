using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using CommerceProject.Business.Custom;
using CommerceProject.Business.Helper.PriceCalculation;

namespace CommerceProject.Business.BusinessServices
{
    public class PaketService : GenericRepository<Paket>, IPaketService
    {
        #region Cache
        public List<Paket> GetAllActivePackagesFromCache(List<int> seciliKategoriler = null)
        {
            var aktifPaketler = this.FindByFromCache(x => x.AktifMi &&
                                                        (seciliKategoriler == null ||
                                                        x.PaketKategori.Any(p => seciliKategoriler.Contains(p.KategoriId))),
                                                        CacheDataObj.AktifPaketler,
                                                        new string[] { "PaketResim", "PaketKategori", "PaketUrun", "PaketUrun.Urun", "PaketNitelik", "PaketNitelik.Nitelik", "Vergi" }).
                                                        AsEnumerable().
                                                        Select(x =>
                                                        {
                                                            //resim koşulları değişecek
                                                            x.AnaResim = x.PaketResim.FirstOrDefault();//(u => u.UrunResimTipId == 1);
                                                            x.ThumbResim = x.PaketResim.FirstOrDefault();// (u => u.UrunResimTipId == 2);      
                                                            x.EskiFiyat = x.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true).Sum(p => p.Urun.EskiFiyat);
                                                            x.Fiyat = x.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true).Sum(p => p.Urun.Fiyat);
                                                            x.IndirimMiktari = x.EskiFiyat == 0 ? 0 : (100 - Convert.ToInt32((x.Fiyat / x.EskiFiyat) * 100));
                                                            //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
                                                            //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
                                                            var priceDetail = PriceCalculationHelper.PriceDetail(x.Fiyat, x.Vergi.VergiOrani);
                                                            x.KdvDahilTutar = priceDetail.KdvDahilTutar;
                                                            x.KdvHaricTutar = priceDetail.KdvHaricTutar;
                                                            x.KdvTutar = priceDetail.KdvTutar;

                                                            return x;
                                                        }).OrderBy(x => x.Sira).ToList();
            return aktifPaketler;
        }

        //yapılacak, bitmedi
        public List<Paket> GetAllActiveRelatedPackagesFromCache(List<int> seciliKategoriler = null)
        {
            var aktifPaketler = GetAllActivePackagesFromCache(seciliKategoriler);

            return aktifPaketler;
        }

        public List<Paket> SearchPackagesFromCache(string searchParameter)
        {
            var packages = this.FindByFromCache(x => x.AktifMi &&
                                            x.Adi.Contains(searchParameter), CacheDataObj.AktifPaketler, new string[] { "PaketResim", "PaketKategori", "PaketUrun", "PaketUrun.Urun", "PaketNitelik", "PaketNitelik.Nitelik", "Vergi" }).
                                            AsEnumerable().
                                            Select(x =>
                                            {
                                                x.EskiFiyat = x.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true).Sum(p => p.Urun.EskiFiyat);
                                                x.Fiyat = x.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true).Sum(p => p.Urun.Fiyat);
                                                x.IndirimMiktari = x.EskiFiyat == 0 ? 0 : (100 - Convert.ToInt32((x.Fiyat / x.EskiFiyat) * 100));
                                                return x;
                                            }).
                                            OrderBy(x => x.Sira).ToList();
            return packages;
        }

        public Paket GetSinglePackageFromCache(int paketId, List<int> urunler = null)
        {
            var paket = this.GetSingleFromCache(CacheDataObj.AktifPaketler, (x => x.PaketId == paketId), new string[] { "PaketResim", "PaketKategori", "PaketUrun", "PaketUrun.Urun", "PaketNitelik", "PaketNitelik.Nitelik", "Vergi" });

            paket.PaketUrun = paket.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true && (urunler == null || (urunler != null && urunler.Contains(p.UrunId)))).ToList();
            paket.AnaResim = paket.PaketResim.FirstOrDefault();
            paket.ThumbResim = paket.PaketResim.LastOrDefault();
            paket.EskiFiyat = paket.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true).Sum(p => p.Urun.EskiFiyat);
            paket.Fiyat = paket.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true).Sum(p => p.Urun.Fiyat);
            paket.IndirimMiktari = paket.EskiFiyat == 0 ? 0 : (100 - Convert.ToInt32((paket.Fiyat / paket.EskiFiyat) * 100));

            //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
            //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
            var priceDetail = PriceCalculationHelper.PriceDetail(paket.Fiyat, paket.Vergi.VergiOrani);
            paket.KdvDahilTutar = priceDetail.KdvDahilTutar;
            paket.KdvHaricTutar = priceDetail.KdvHaricTutar;
            paket.KdvTutar = priceDetail.KdvTutar;

            return paket;
        }
        #endregion

        #region NonCache
        public List<Paket> GetAllActivePackages(List<int> seciliKategoriler = null)
        {
            if (seciliKategoriler == null)
                seciliKategoriler = new List<int>();

            var aktifPaketler = this.FindBy(x => x.AktifMi&&
                                                 x.PaketKategori.Any(p => seciliKategoriler.Contains(p.KategoriId)),
                                                        true,
                                                        new string[] { "PaketResim", "PaketKategori", "PaketUrun", "PaketUrun.Urun", "PaketNitelik", "PaketNitelik.Nitelik", "Vergi" }).
                                                        ToList().
                                                        Select(x =>
                                                        {
                                                            //resim koşulları değişecek
                                                            x.AnaResim = x.PaketResim.FirstOrDefault();//(u => u.UrunResimTipId == 1);
                                                            x.ThumbResim = x.PaketResim.FirstOrDefault();//(u => u.UrunResimTipId == 2);    
                                                            x.EskiFiyat = x.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true).Sum(p => p.Urun.EskiFiyat);
                                                            x.Fiyat = x.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true).Sum(p => p.Urun.Fiyat);
                                                            x.IndirimMiktari = x.EskiFiyat == 0 ? 0 : (100 - Convert.ToInt32((x.Fiyat / x.EskiFiyat) * 100));
                                                            //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
                                                            //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
                                                            var priceDetail = PriceCalculationHelper.PriceDetail(x.Fiyat, x.Vergi.VergiOrani);
                                                            x.KdvDahilTutar = priceDetail.KdvDahilTutar;
                                                            x.KdvHaricTutar = priceDetail.KdvHaricTutar;
                                                            x.KdvTutar = priceDetail.KdvTutar;

                                                            return x;
                                                        }).OrderBy(x => x.Sira).ToList();
            return aktifPaketler;
        }

        //yapılacak, bitmedi
        public List<Paket> GetAllActiveRelatedPackages(List<int> seciliKategoriler = null)
        {
            var aktifPaketler = GetAllActivePackages(seciliKategoriler);

            return aktifPaketler;
        }

        public List<Paket> SearchPackages(string searchParameter)
        {
            var packages = this.FindBy(x => x.AktifMi &&
                                            x.Adi.Contains(searchParameter), true, new string[] { "PaketResim", "PaketKategori", "PaketUrun", "PaketUrun.Urun", "PaketNitelik", "PaketNitelik.Nitelik", "Vergi" }).
                                            AsEnumerable().
                                            Select(x =>
                                            {
                                                x.EskiFiyat = x.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true).Sum(p => p.Urun.EskiFiyat);
                                                x.Fiyat = x.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true).Sum(p => p.Urun.Fiyat);
                                                x.IndirimMiktari = x.EskiFiyat == 0 ? 0 : (100 - Convert.ToInt32((x.Fiyat / x.EskiFiyat) * 100));
                                                return x;
                                            }).
                                            OrderBy(x => x.Sira).ToList();
            return packages;
        }

        public Paket GetSinglePackage(int paketId, List<int> urunler = null)
        {
            var paket = this.GetSingle((x => x.PaketId == paketId), true, new string[] { "PaketResim", "PaketKategori", "PaketUrun", "PaketUrun.Urun", "PaketUrun.Urun.UrunResim", "PaketNitelik", "PaketNitelik.Nitelik", "Vergi" });

            paket.PaketUrun = paket.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true && (urunler == null || (urunler != null && urunler.Contains(p.UrunId)))).
                
                Select(x=> 
                {
                    var anaResim = x.Urun.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 1);
                    var thumbResim = x.Urun.UrunResim.FirstOrDefault(u => u.AktifMi && u.UrunResimTipId == 2);
                    x.Urun.AnaResim = anaResim == null ? thumbResim : anaResim;
                    x.Urun.ThumbResim = thumbResim;

                    return x;
                }).ToList();
            paket.AnaResim = paket.PaketResim.FirstOrDefault();
            paket.ThumbResim = paket.PaketResim.LastOrDefault();
            paket.EskiFiyat = paket.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true).Sum(p => p.Urun.EskiFiyat);
            paket.Fiyat = paket.PaketUrun.Where(p => p.AktifMi == true && p.Urun.AktifMi == true).Sum(p => p.Urun.Fiyat);
            paket.IndirimMiktari = paket.EskiFiyat == 0 ? 0 : (100 - Convert.ToInt32((paket.Fiyat / paket.EskiFiyat) * 100));

            //burada kdv dahil hariç hesaplanabilmesi için iş kuralı var.
            //program içerik ayarlarında kdv dahilmi hariç mi hesaplanacağı seçiliyor 
            var priceDetail = PriceCalculationHelper.PriceDetail(paket.Fiyat, paket.Vergi.VergiOrani);
            paket.KdvDahilTutar = priceDetail.KdvDahilTutar;
            paket.KdvHaricTutar = priceDetail.KdvHaricTutar;
            paket.KdvTutar = priceDetail.KdvTutar;

            return paket;
        }
        #endregion
    }
}
