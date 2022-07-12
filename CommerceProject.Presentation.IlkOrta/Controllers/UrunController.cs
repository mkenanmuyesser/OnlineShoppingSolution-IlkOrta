using BotDetect.Web;
using BotDetect.Web.Mvc;
using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.Custom;
using CommerceProject.Business.Entities;
using CommerceProject.Business.Helper.Email;
using CommerceProject.Business.Helper.PriceCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PagedList;
using PagedList.Mvc;
using static CommerceProject.Presentation.IlkOrta.Helper.PageHelper;

namespace CommerceProject.Presentation.IlkOrta.Controllers
{
    public class UrunController : BaseController
    {
        #region Properties
        private List<int> SeciliKategoriler = new List<int>();

        private string KategoriMenu { get; set; }

        private string BreadCrumbMenu { get; set; }

        public int UrunSayfasiUstSliderBannerTipId
        {
            get
            {
                return Convert.ToInt32(BannerTipEnum.UrunSayfasiUstSliderBanner);
            }
        }

        public int UrunSayfasiUstBannerTipId
        {
            get
            {
                return Convert.ToInt32(BannerTipEnum.UrunSayfasiUstBanner);
            }
        }

        public int SayfalamaUrunPaketSayisi
        {
            get
            {
                return 40;
            }
        }
        #endregion

        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        ISepetService SepetService;
        IKategoriService KategoriService;
        IBannerService BannerService;
        IUrunService UrunService;
        IPaketService PaketService;
        IUrunYorumService UrunYorumService;
        IUrunResimService UrunResimService;
        IBankaService BankaService;
        public UrunController(IIcerikAyarService iIcerikAyarService,
                              IKullaniciService iKullaniciService,
                              ISepetService iSepetService,
                              IKategoriService iKategoriService,
                              IBannerService iBannerService,
                              IUrunService iUrunService,
                              IPaketService iPaketService,
                              IUrunYorumService iUrunYorumService,
                              IUrunResimService iUrunResimService,
                              IBankaService iBankaService) : base(iIcerikAyarService,
                                                                  iKullaniciService,
                                                                  iSepetService,
                                                                  iKategoriService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;
            SepetService = iSepetService;
            KategoriService = iKategoriService;
            UrunService = iUrunService;
            PaketService = iPaketService;
            UrunYorumService = iUrunYorumService;
            UrunResimService = iUrunResimService;
            BannerService = iBannerService;
            BankaService = iBankaService;
        }

        #region Actions
        public ActionResult Index(int? id)
        {
            if (id == null)
                return RedirectToAction("Hata", "Home");

            ViewBag.PageProperties = PageProperties.SetPageProperties("Ürünler", KategoriBreadCrumbOlustur(id), "", "", "/Urun");

            ViewBag.UstSliderBannerlar = BannerService.FindBy(x => (x.AktifMi == true &&
                                                                   x.BannerTipId == UrunSayfasiUstSliderBannerTipId), true).
                                                                   OrderBy(x => x.Sira).ToList();

            ViewBag.UstBannerlar = BannerService.FindBy(x => (x.AktifMi == true &&
                                                             x.BannerTipId == UrunSayfasiUstBannerTipId), true).
                                                             OrderBy(x => x.Sira).ToList();

            ViewBag.KategoriMenu = KategoriMenuOlustur(true, id);

            // var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            //List<Urun> takenUrunler = new List<Urun>();
            //List<Paket> takenPaketler = new List<Paket>();
            //if (icerikAyar.CacheAktifMi)
            //{
            //    urunler = UrunService.GetAllActiveProductsFromCache(SeciliKategoriler);
            //    paketler = PaketService.GetAllActivePackagesFromCache(SeciliKategoriler);

            //    ViewBag.BenzerUrunler = urunler.Take(18).ToList();
            //    ViewBag.OzelFirsatUrunleri = urunler.Take(18).ToList();
            //}
            //else
            //{
            //    urunler = UrunService.GetAllActiveProducts(SeciliKategoriler);
            //    paketler = PaketService.GetAllActivePackages(SeciliKategoriler);

            //ViewBag.BenzerUrunler = urunler.Take(18).ToList();
            //ViewBag.OzelFirsatUrunleri = urunler.Take(18).ToList();
            //}

            List<Urun> urunler = new List<Urun>();
            List<Paket> paketler = new List<Paket>();

            // Paging
            var urunlerCount = UrunService.FindBy(x => x.AktifMi && x.UrunKategori.Any(p => SeciliKategoriler.Contains(p.KategoriId)), true).Count();
            var paketlerCount = PaketService.FindBy(x => x.AktifMi && x.PaketKategori.Any(p => SeciliKategoriler.Contains(p.KategoriId)), true).Count();

            var totalCount = urunlerCount + paketlerCount;
            var pageIndex = Convert.ToInt32(TempData["PageIndex"] ?? 1);

            var pagedList = new List<int>();
            for (int i = 0; i < totalCount; i++)
            {
                pagedList.Add(i);
            }
            ViewBag.PagedListModel = pagedList.ToPagedList(pageIndex, SayfalamaUrunPaketSayisi);

            if (totalCount > 0)
            {
                paketler = PaketService.FindBy(x => x.AktifMi &&
                                                 x.PaketKategori.Any(p => SeciliKategoriler.Contains(p.KategoriId)),
                                                        true,
                                                        new string[] { "PaketResim", "PaketKategori", "PaketUrun", "PaketUrun.Urun", "PaketNitelik", "PaketNitelik.Nitelik", "Vergi" }).
                                                        OrderBy(x => x.PaketId).
                                                        Skip((pageIndex - 1) * SayfalamaUrunPaketSayisi).
                                                        Take(SayfalamaUrunPaketSayisi).
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
                                                        }).ToList();

                if (paketler.Count < (pageIndex * SayfalamaUrunPaketSayisi))
                {
                    // bu durumda 1. senaryo işler. ilk sayfa 9 sonuc olmalı ama 3 paket geldi, ilk 6 ürün gelmeli
                    if ((pageIndex * SayfalamaUrunPaketSayisi) - paketler.Count <= SayfalamaUrunPaketSayisi)
                    {
                        urunler = UrunService.FindBy(x => x.AktifMi &&
                                                 x.UrunKategori.Any(p => SeciliKategoriler.Contains(p.KategoriId)),
                                                true,
                                                new string[] { "UrunResim", "UrunKategori", "Vergi" }).
                                                OrderBy(x => x.UrunId).
                                                Skip(0).
                                                Take((pageIndex * SayfalamaUrunPaketSayisi) - (paketler.Count)).
                                                ToList().
                                                Select(x =>
                                                {
                                                    var anaResim = UrunResimService.GetFirst(u => u.UrunId == x.UrunId && u.AktifMi && u.UrunResimTipId == 1);
                                                    var thumbResim = UrunResimService.GetFirst(u => u.UrunId == x.UrunId && u.AktifMi && u.UrunResimTipId == 2);
                                                    x.AnaResim = anaResim == null ? thumbResim : anaResim;
                                                    x.ThumbResim = thumbResim;

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
                                                }).ToList();
                    }
                    // bu durumda 2. senaryo işler, ikinci sayfa 10-18 olmalı. ilk sayfada 3 paket 6 ürün geldi, şimdi ilk 6 ürün skip() yapılmalı
                    else
                    {
                        urunler = UrunService.FindBy(x => x.AktifMi &&
                                               x.UrunKategori.Any(p => SeciliKategoriler.Contains(p.KategoriId)),
                                                true,
                                                new string[] { "UrunResim", "UrunKategori", "Vergi" }).
                                                OrderBy(x => x.UrunId).
                                                Skip(((pageIndex - 1) * SayfalamaUrunPaketSayisi) - (paketler.Count)).
                                                Take(SayfalamaUrunPaketSayisi).
                                                ToList().
                                                Select(x =>
                                                {
                                                    var anaResim = UrunResimService.GetFirst(u => u.UrunId == x.UrunId && u.AktifMi && u.UrunResimTipId == 1);
                                                    var thumbResim = UrunResimService.GetFirst(u => u.UrunId == x.UrunId && u.AktifMi && u.UrunResimTipId == 2);
                                                    x.AnaResim = anaResim == null ? thumbResim : anaResim;
                                                    x.ThumbResim = thumbResim;

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
                                                }).ToList();
                    }
                }
            }

            // Sorting
            var sortIndex = (TempData["SortIndex"] ?? "last").ToString();
            ViewBag.SortIndex = sortIndex;

            switch (sortIndex)
            {
                case "last":
                    urunler = urunler.OrderByDescending(x => x.OlusturmaTarihi).ToList();
                    paketler = paketler.OrderByDescending(x => x.OlusturmaTarihi).ToList();
                    break;
                case "a-z":
                    urunler = urunler.OrderBy(x => x.Adi).ToList();
                    paketler = paketler.OrderBy(x => x.Adi).ToList();
                    break;
                case "z-a":
                    urunler = urunler.OrderByDescending(x => x.Adi).ToList();
                    paketler = paketler.OrderByDescending(x => x.Adi).ToList();
                    break;
                case "price-up":
                    urunler = urunler.OrderBy(x => x.Fiyat).ToList();
                    paketler = paketler.OrderBy(x => x.Fiyat).ToList();
                    break;
                case "price-down":
                    urunler = urunler.OrderByDescending(x => x.Fiyat).ToList();
                    paketler = paketler.OrderByDescending(x => x.Fiyat).ToList();
                    break;
                case "sell":
                    urunler = urunler.OrderByDescending(x => x.SiparisDetay.Where(u => u.AktifMi == true).Count()).ToList();
                    break;
            }

            ViewBag.Urunler = urunler;
            ViewBag.Paketler = paketler;
            ViewBag.TotalCount = totalCount;
            ViewBag.PageIndex = pageIndex;

            return View();
        }

        public ActionResult IndexPaging(int? id, int pageIndex = 1, string sortIndex = "last")
        {
            TempData["PageIndex"] = pageIndex;
            TempData["SortIndex"] = sortIndex;

            return RedirectToAction("Index", "Urun", new { id = id });
        }

        public ActionResult Detay(int? id)
        {
            //var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            Urun urun = null;
            if (id == null)
            {
                return RedirectToAction("Hata", "Home");
            }
            else
            {
                //if (icerikAyar.CacheAktifMi)
                //    urun = UrunService.GetSingleProductFromCache(id.Value);
                //else
                //    urun = UrunService.GetSingleProduct(id.Value);                

                urun = UrunService.GetSingleProduct(id.Value);

                if (urun == null)
                    return RedirectToAction("Hata", "Home");
                else
                    ViewBag.UrunData = urun;

            }

            ViewBag.PageProperties = PageProperties.SetPageProperties(urun.Adi, "", "", "", "/Urun/Detay/" + id.ToString());

            urun.ToplamSatisSayisi = UrunService.GetProductsTotalSales(id.Value);

            //if (icerikAyar.CacheAktifMi)
            //{
            //    ViewBag.IliskiliUrunler = UrunService.GetAllActiveRelatedProductsFromCache(id.Value).ToList();

            //    ViewBag.Bankalar = BankaService.GetInstallmentCalculationDataByPriceFromCache(urun.Fiyat);
            //}
            //else
            //{
            //    ViewBag.IliskiliUrunler = UrunService.GetAllActiveRelatedProducts(id.Value).ToList();

            //    ViewBag.Bankalar = BankaService.GetInstallmentCalculationDataByPrice(urun.Fiyat);
            //}

            ViewBag.IliskiliUrunler = UrunService.FindBy(x => x.AktifMi &&
                                                x.UrunId != urun.UrunId &&
                                                x.IlgiliUrun.Any(p => p.UrunId1 == urun.UrunId || p.UrunId2 == urun.UrunId),
                                                true,
                                                new string[] { "UrunResim", "IlgiliUrun", "Vergi" }).
                                                OrderBy(x => x.UrunId).
                                                Take(10).
                                                ToList().
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
                                                }).ToList();

            ViewBag.Bankalar = BankaService.GetInstallmentCalculationDataByPrice(urun.Fiyat);

            return View();
        }

        public ActionResult Paket(int? id)
        {
            //var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            Paket paket = null;
            if (id == null)
            {
                return RedirectToAction("Hata", "Home");
            }
            else
            {
                //if (icerikAyar.CacheAktifMi)
                //    paket = PaketService.GetSinglePackageFromCache(id.Value);
                //else
                //    paket = PaketService.GetSinglePackage(id.Value);

                paket = PaketService.GetSinglePackage(id.Value);

                if (paket == null)
                    return RedirectToAction("Hata", "Home");
                else
                    ViewBag.PaketData = paket;
            }

            ViewBag.PageProperties = PageProperties.SetPageProperties(paket.Adi, "", "", "", "/Urun/Paket/" + id.ToString());

            //if (icerikAyar.CacheAktifMi)
            //{
            //    var urunler = UrunService.GetAllActiveProductsFromCache();

            //    ViewBag.IliskiliUrunler = urunler.Take(18).ToList();

            //    ViewBag.Bankalar = BankaService.GetInstallmentCalculationDataByPriceFromCache(paket.Fiyat);
            //}
            //else
            //{
            //    var urunler = UrunService.GetAllActiveProducts();

            //    ViewBag.IliskiliUrunler = urunler.Take(18).ToList();

            //    ViewBag.Bankalar = BankaService.GetInstallmentCalculationDataByPrice(paket.Fiyat);
            //}

            var pakettekiUrunListesi = paket.PaketUrun.Select(u => u.UrunId).ToList();

            ViewBag.IliskiliUrunler = UrunService.FindBy(x => x.AktifMi &&
                                                              x.IlgiliUrun.Any(p => pakettekiUrunListesi.Contains(p.UrunId1) || pakettekiUrunListesi.Contains(p.UrunId2)),
                                               true,
                                               new string[] { "UrunResim", "IlgiliUrun", "Vergi" }).
                                               OrderBy(x => x.UrunId).
                                               Take(10).
                                               ToList().
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
                                               }).ToList();


            ViewBag.Bankalar = BankaService.GetInstallmentCalculationDataByPrice(paket.Fiyat);

            return View();
        }

        public ActionResult UrunYorumPartial(int? id)
        {
            return PartialView("~/Views/Urun/Partials/UrunYorumPartial.cshtml");
        }
        #endregion

        #region Ajax Methods
        private string KategoriMenuOlustur(bool seciliKategori, int? id)
        {
            if (id.HasValue)
            {
                // var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

                var kategori = KategoriService.GetSingle(x => x.AktifMi && x.KategoriId == id.Value, true, new string[] { "Kategori1" });              

                if (kategori != null)
                {
                    kategori.SeciliKategori = seciliKategori;

                    SeciliKategoriler.Add(kategori.KategoriId);

                    var altKategoriler = kategori.Kategori1.Where(x => x.AktifMi).OrderBy(x => x.Sira);

                    if (altKategoriler.Any())
                    {
                        KategoriMenu += string.Format("<li class='{0}'><a href = '/Urun/Index/{1}'><span class='open-sub'></span>{2}</a><ul class='sub'>", kategori.SeciliKategori ? "parent active" : "parent active", kategori.Slug, kategori.Adi);

                        foreach (var altKategoriItem in altKategoriler)
                        {
                            KategoriMenuOlustur(false, altKategoriItem.KategoriId);
                        }

                        KategoriMenu += "</ul></li>";
                    }
                    else
                    {
                        KategoriMenu += string.Format("<li class='parent active'><a href = '/Urun/Index/{0}'>{1}</a></li>", kategori.KategoriId, kategori.Adi);
                    }
                }
            }

            return KategoriMenu;
        }

        private string KategoriBreadCrumbOlustur(int? id)
        {
            if (id.HasValue)
            {
                // var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

                var kategoriHierarchy = new List<Kategori>();
                var kategori = KategoriService.GetSingle(x => x.KategoriId == id.Value, true);
                kategori.SeciliKategori = true;

                if (kategori != null)
                {
                    kategoriHierarchy.Add(kategori);

                    var ustKategoriId = kategori.UstKategoriId;
                    while (ustKategoriId != null)
                    {
                        var parent = KategoriService.GetSingle(x => x.KategoriId == ustKategoriId.Value, true);
                        kategoriHierarchy.Add(parent);
                        ustKategoriId = parent.UstKategoriId;
                    }
                }

                foreach (var kategoriItem in kategoriHierarchy)
                {
                    BreadCrumbMenu = string.Format("<li class='{0}'><a href = '/Urun/Index/{1}'>{2}</a></li>", kategoriItem.SeciliKategori ? "active" : "", kategoriItem.Slug, kategoriItem.Adi) + BreadCrumbMenu;

                    BreadCrumbMenu += "</ul></li>";
                }
            }

            return BreadCrumbMenu;
        }

        [HttpGet]
        public JsonResult UrunPaketAra(int? id, int sayfaSayisi, int sayfaSirasi)
        {
            ViewBag.KategoriMenu = KategoriMenuOlustur(true, id == 0 ? null : id);
            //List<Urun> sonucListesi = new List<Urun>();

            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            if (icerikAyar.CacheAktifMi)
            {
                var urunler = UrunService.GetAllActiveProductsFromCache(SeciliKategoriler);
                var kategoriler = PaketService.GetAllActivePackagesFromCache(SeciliKategoriler);

                ViewBag.Urunler = urunler;
                ViewBag.Kategoriler = kategoriler;
                //sonucListesi = urunler;
            }
            else
            {
                var urunler = UrunService.GetAllActiveProducts(SeciliKategoriler);
                var kategoriler = PaketService.GetAllActivePackages(SeciliKategoriler);
                ViewBag.Urunler = urunler;
                ViewBag.Kategoriler = kategoriler;
                //sonucListesi = urunler;
            }

            //var count = sonucListesi.Count;

            //sonucListesi = sonucListesi.Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                //sonucListesi = sonucListesi.Select(x => new
                //{
                //    AdSoyadSirket = x.Ad + " " + x.Soyad + "-" + x.Sirket,
                //    AdresBilgi = x.AdresBilgi,
                //    PostaKodu = x.PostaKodu,
                //    IlceAdi = x.AdresIlce == null ? "" : x.AdresIlce.IlceAdi,
                //    IlAdi = x.AdresIl == null ? "" : x.AdresIl.IlAdi,
                //    Telefon1 = x.Telefon1,
                //    Telefon2 = x.Telefon2,
                //    AdresId = x.AdresId
                //}).ToList(),
                //sonucListesi,
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                //toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PaketUrunHesapla(int paketId, List<int> seciliUrunler)
        {
            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            Paket paket;
            if (icerikAyar.CacheAktifMi)
                paket = PaketService.GetSinglePackageFromCache(paketId, seciliUrunler);
            else
                paket = PaketService.GetSinglePackage(paketId, seciliUrunler);

            if (paketId == 0 || seciliUrunler == null || !seciliUrunler.Any())
            {
                return Json(new { eskifiyat = string.Format("{0:N}", 0), fiyat = string.Format("{0:N}", 0), indirimMiktari = 0 }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(new { eskifiyat = string.Format("{0:N}", paket.EskiFiyat), fiyat = string.Format("{0:N}", paket.Fiyat), indirimMiktari = paket.IndirimMiktari }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult YorumGonder(string captchaId, string instanceId, string userInput, int urunId, int puan, string yorum)
        {
            bool ajaxValidationResult = Captcha.AjaxValidate(captchaId, userInput, instanceId);

            //string sonuc = "Beklenmedik bir sorun oluştu. Lütfen tekrar deneyin.";
            if (ajaxValidationResult)
            {
                var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

                if (girisYapanKullanici == null)
                {
                    return Json(new
                    {
                        messageType = "warning",
                        message = "Lütfen yorum yapabilmek için kullanıcı girişi yapınız."
                    }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    var flag = UrunYorumService.AddUserComment(girisYapanKullanici.KullaniciId, urunId, puan, yorum);

                    if (flag)
                        return Json(new
                        {
                            messageType = "success",
                            message = "Yorumunuz başarılı bir şekilde gönderilmiştir."
                        }, JsonRequestBehavior.DenyGet);
                    else
                        return Json(new
                        {
                            messageType = "error",
                            message = "Beklenmedik bir sorun oluştu. Lütfen tekrar deneyin."
                        }, JsonRequestBehavior.DenyGet);
                }
            }
            else
            {
                return Json(new
                {
                    messageType = "warning",
                    message = "Resimdeki kod yanlış girildi."
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult TavsiyeEt(string captchaId, string instanceId, string userInput, int urunId, string gonderenEposta, string gonderilenEposta, string yorum)
        {
            bool ajaxValidationResult = Captcha.AjaxValidate(captchaId, userInput, instanceId);

            //string sonuc = "Beklenmedik bir sorun oluştu. Lütfen tekrar deneyin.";
            if (ajaxValidationResult)
            {
                var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

                string konu = icerikAyar.SirketAd + " Ürün Tavsiyesi";
                string icerik = gonderenEposta + " tarafından ilginizi çekecek bir ürün tavsiye edildi.</br>" +
                    "Arkadaşınızın yorumu : " + yorum + "</br>" +
                    "Ürüne gitmek için <a href='" + icerikAyar.Url + "/Urun/Detay/" + urunId.ToString() + "' target='blank'>tıklayınız</a>";

                var flag = EmailHelper.SendMail(konu, icerik, gonderilenEposta);

                if (flag)
                    return Json(new
                    {
                        messageType = "success",
                        message = "Tavsiye e-postanız başarılı bir şekilde gönderilmiştir."
                    }, JsonRequestBehavior.DenyGet);
                else
                    return Json(new
                    {
                        messageType = "error",
                        message = "Beklenmedik bir sorun oluştu. Lütfen tekrar deneyin."
                    }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(new
                {
                    messageType = "warning",
                    message = "Resimdeki kod yanlış girildi."
                }, JsonRequestBehavior.DenyGet);
            }
        }
        #endregion
    }
}