using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.Custom;
using CommerceProject.Business.Entities;
using CommerceProject.Business.Helper.PriceCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using static CommerceProject.Presentation.IlkOrta.Helper.PageHelper;

namespace CommerceProject.Presentation.IlkOrta.Controllers
{
    public class AramaController : BaseController
    {
        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        ISepetService SepetService;
        IKategoriService KategoriService;
        IUrunService UrunService;
        IPaketService PaketService;
        IUrunResimService UrunResimService;
        public AramaController(IIcerikAyarService iIcerikAyarService,
                               IKullaniciService iKullaniciService,
                               ISepetService iSepetService,
                               IKategoriService iKategoriService,
                               IUrunService iUrunService,
                               IPaketService iPaketService,
                               IUrunResimService iUrunResimService) : base(iIcerikAyarService,
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
            UrunResimService = iUrunResimService;
        }

        #region Actions
        [HttpGet]
        [ActionName("Index")]
        public ActionResult IndexGet(string searchParameter, string pageIndex)
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Arama Sonuçları", "", "", "", "/Arama");
            TempData["SearchParameter"] = searchParameter;

            Ara(searchParameter, Convert.ToInt32(pageIndex));

            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost(string searchParameter, string pageIndex)
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Arama Sonuçları", "", "", "", "/Arama");
            TempData["SearchParameter"] = searchParameter;

            Ara(searchParameter, Convert.ToInt32(pageIndex));

            return View();
        }
        #endregion

        #region Ajax Methods
        private void Ara(string aramaParametre, int pageIndex)
        {
            //var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            string aranacakKelime = aramaParametre == null ? "" : aramaParametre.Trim();
            //aranacak kelime girilmezse sonuç getirmeyecek. boşu boşuna yorulmasın
            if (!string.IsNullOrEmpty(aranacakKelime))
            {
                ViewBag.AranacakKelime = aranacakKelime;

                //List<Urun> urunler;
                //List<Paket> paketler;
                //List<Urun> takenUrunler = new List<Urun>();
                //List<Paket> takenPaketler = new List<Paket>();
                //if (icerikAyar.CacheAktifMi)
                //{ 
                //    urunler = UrunService.SearchProductsFromCache(aranacakKelime); //ViewBag.Urunler
                //    paketler = PaketService.SearchPackagesFromCache(aranacakKelime); //ViewBag.Paketler
                //}
                //else
                //{
                //    urunler = UrunService.SearchProducts(aranacakKelime);
                //    paketler = PaketService.SearchPackages(aranacakKelime);
                //}

                List<Urun> urunler = new List<Urun>();
                List<Paket> paketler = new List<Paket>();

                // Paging
                var urunlerCount = UrunService.FindBy(x => x.AktifMi && x.Adi.Contains(aranacakKelime), true).Count();
                var paketlerCount = PaketService.FindBy(x => x.AktifMi && x.Adi.Contains(aranacakKelime), true).Count();

                var totalCount = urunlerCount + paketlerCount;
                pageIndex = pageIndex == 0 ? 1 : pageIndex;

                var pagedList = new List<int>();
                for (int i = 0; i < totalCount; i++)
                {
                    pagedList.Add(i);
                }
                ViewBag.PagedListModel = pagedList.ToPagedList(pageIndex, 9);

                if (totalCount > 0)
                {
                    paketler = PaketService.FindBy(x => x.AktifMi &&
                                                       x.Adi.Contains(aranacakKelime),
                                                            true,
                                                            new string[] { "PaketResim", "PaketKategori", "PaketUrun", "PaketUrun.Urun", "PaketNitelik", "PaketNitelik.Nitelik", "Vergi" }).
                                                            OrderBy(x => x.PaketId).
                                                            Skip((pageIndex - 1) * 9).
                                                            Take(9).
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

                    if (paketler.Count < (pageIndex * 9))
                    {
                        // bu durumda 1. senaryo işler. ilk sayfa 9 sonuc olmalı ama 3 paket geldi, ilk 6 ürün gelmeli
                        if ((pageIndex * 9) - paketler.Count <= 9)
                        {
                            urunler = UrunService.FindBy(x => x.AktifMi &&
                                                   x.Adi.Contains(aranacakKelime),
                                                    true,
                                                    new string[] { "UrunKategori", "UrunYorum", "UrunNitelik", "UrunNitelik.Nitelik", "UrunOzellik", "Vergi", "Marka", "SiparisDetay" }).
                                                    OrderBy(x => x.UrunId).
                                                    Skip(0).
                                                    Take((pageIndex * 9) - (paketler.Count)).
                                                    ToList().
                                                    Select(x =>
                                                    {
                                                        var anaResim = UrunResimService.GetFirst(u => u.UrunId == x.UrunId && u.AktifMi && u.UrunResimTipId == 1);
                                                        var thumbResim = UrunResimService.GetFirst(u => u.UrunId == x.UrunId && u.AktifMi && u.UrunResimTipId == 2);
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
                                                    }).ToList();
                        }
                        // bu durumda 2. senaryo işler, ikinci sayfa 10-18 olmalı. ilk sayfada 3 paket 6 ürün geldi, şimdi ilk 6 ürün skip() yapılmalı
                        else
                        {
                            urunler = UrunService.FindBy(x => x.AktifMi &&
                                                              x.Adi.Contains(aranacakKelime),
                                                    true,
                                                    new string[] { "UrunKategori", "UrunYorum", "UrunNitelik", "UrunNitelik.Nitelik", "UrunOzellik", "Vergi", "Marka", "SiparisDetay" }).
                                                    OrderBy(x => x.UrunId).
                                                    Skip(((pageIndex - 1) * 9) - (paketler.Count)).
                                                    Take(9).
                                                    ToList().
                                                    Select(x =>
                                                    {
                                                        var anaResim = UrunResimService.GetFirst(u => u.UrunId == x.UrunId && u.AktifMi && u.UrunResimTipId == 1);
                                                        var thumbResim = UrunResimService.GetFirst(u => u.UrunId == x.UrunId && u.AktifMi && u.UrunResimTipId == 2);
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
                                                    }).ToList();
                        }
                    }
                }

                ViewBag.Urunler = urunler;
                ViewBag.Paketler = paketler;
                ViewBag.TotalCount = totalCount;
                ViewBag.PageIndex = pageIndex;
            }
        }
        #endregion
    }
}