using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.Custom;
using CommerceProject.Business.Entities;
using CommerceProject.Presentation.IlkOrta.Helper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using static CommerceProject.Presentation.IlkOrta.Helper.PageHelper;

namespace CommerceProject.Presentation.IlkOrta.Controllers
{
    public class HomeController : BaseController
    {
        #region Properties
        public int AnasayfaUstSliderBannerTipId
        {
            get
            {
                return Convert.ToInt32(BannerTipEnum.AnasayfaUstSliderBanner);
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
        IMarkaService MarkaService;
        IHaberBlogService HaberBlogService;
        IKisaLinkService KisaLinkService;
        IHaberBulteniAbonelikService HaberBulteniAbonelikService;
        IAnketService AnketService;
        IAnketCevapService AnketCevapService;
        IKampanyaService KampanyaService;
        public HomeController(IIcerikAyarService iIcerikAyarService,
                              IKullaniciService iKullaniciService,
                              ISepetService iSepetService,
                              IKategoriService iKategoriService,
                              IBannerService iBannerService,
                              IUrunService iUrunService,
                              IPaketService iPaketService,
                              IMarkaService iMarkaService,
                              IHaberBlogService iHaberBlogService,
                              IKisaLinkService iKisaLinkService,
                              IHaberBulteniAbonelikService iHaberBulteniAbonelikService,
                              IAnketService iAnketService,
                              IAnketCevapService iAnketCevapService,
                              IKampanyaService iKampanyaService) : base(iIcerikAyarService,
                                                                                                iKullaniciService,
                                                                                                iSepetService,
                                                                                                iKategoriService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;
            SepetService = iSepetService;
            KategoriService = iKategoriService;

            BannerService = iBannerService;
            UrunService = iUrunService;
            PaketService = iPaketService;
            MarkaService = iMarkaService;
            HaberBlogService = iHaberBlogService;
            KisaLinkService = iKisaLinkService;
            HaberBulteniAbonelikService = iHaberBulteniAbonelikService;
            AnketService = iAnketService;
            AnketCevapService = iAnketCevapService;
            KampanyaService = iKampanyaService;
        }

        #region Actions
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var url = filterContext.HttpContext.Request.Url;
            if (url != null)
            {
                NameValueCollection urlQuery = System.Web.HttpUtility.ParseQueryString(filterContext.HttpContext.Request.Url.Query);

                for (int i = 0; i < urlQuery.Keys.Count; i++)
                {
                    if (urlQuery.Get(urlQuery[i]) == null)
                    {
                        string shortUrl = url.ToString();
                        var uzunLink = KisaLinkService.GetLongLink(shortUrl);
                        if (!string.IsNullOrEmpty(uzunLink))
                        {
                            KisaLinkService.AddVisitor(shortUrl);
                            filterContext.Result = new RedirectResult(uzunLink);
                            return;
                        }
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }

        [ActionManager]
        public ActionResult Index()
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            ViewBag.PageProperties = PageProperties.SetPageProperties("Anasayfa", "", "", "", "/");

            ViewBag.UstSliderBannerlar = BannerService.FindBy(x => (x.AktifMi == true &&
                                                                    x.BannerTipId == AnasayfaUstSliderBannerTipId), true).
                                                                   OrderBy(x => x.Sira).ToList();

            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            if (icerikAyar.CacheAktifMi)
            {
                ViewBag.OkulPaketListesi = KategoriService.GetCategoriesByPropertyFromCache(11).Select(x => new { id = x.KategoriId, text = x.Adi }).ToList();

                ViewBag.GununFirsatUrunleri = UrunService.GetAllActiveProductsByPropertyFromCache(3);

                ViewBag.VitrinUrunleri = UrunService.GetAllActiveProductsByPropertyFromCache(4);

                ViewBag.EnYeniUrunler = UrunService.GetAllActiveProductsByPropertyFromCache(5);

                ViewBag.CokSatanUrunler = UrunService.GetAllActiveProductsByPropertyFromCache(6);

                ViewBag.IndirimdekiUrunler = UrunService.GetAllActiveProductsByPropertyFromCache(7);

                ViewBag.AnasayfaKategoriler = KategoriService.GetCategoriesByPropertyFromCache(10);

                ViewBag.Markalar = MarkaService.GetAllActiveBrandsFromCache();
            }
            else
            {
                ViewBag.OkulPaketListesi = KategoriService.GetCategoriesByProperty(11).Select(x => new { id = x.KategoriId, text = x.Adi }).ToList();

                ViewBag.GununFirsatUrunleri = UrunService.GetAllActiveProductsByProperty(3);
            
                ViewBag.VitrinUrunleri = UrunService.GetAllActiveProductsByProperty(4);

                ViewBag.EnYeniUrunler = UrunService.GetAllActiveProductsByProperty(5);

                ViewBag.CokSatanUrunler = UrunService.GetAllActiveProductsByProperty(6);

                ViewBag.IndirimdekiUrunler = UrunService.GetAllActiveProductsByProperty(7);

                ViewBag.AnasayfaKategoriler = KategoriService.GetCategoriesByProperty(10);

                ViewBag.Markalar = MarkaService.GetAllActiveBrands();
            }

            var suAn = DateTime.Now.Date;

            // Yayında kampanya var mı?
            var yayindaOlanKampanya = KampanyaService.GetFirst(x => x.AktifMi == true && x.AnaSayfadaGosterilsinMi == true &&
            (
                ((x.BaslangicTarihi != null && x.BitisTarihi == null) && (DbFunctions.TruncateTime(x.BaslangicTarihi) <= suAn)) ||
                ((x.BaslangicTarihi == null && x.BitisTarihi != null) && (DbFunctions.TruncateTime(x.BitisTarihi) >= suAn)) ||
                ((x.BaslangicTarihi != null && x.BitisTarihi != null) && (DbFunctions.TruncateTime(x.BaslangicTarihi) <= suAn && DbFunctions.TruncateTime(x.BitisTarihi) >= suAn))
            )
            , true, new string[] { "KampanyaResim" });

            if(yayindaOlanKampanya != null)
            {
                ViewBag.Kampanya = yayindaOlanKampanya;
            }

            // Yayında anket var mı?
            var yayindaOlanAnket = AnketService.GetFirst(x => x.AktifMi == true && x.YayindaMi == true && 
            (
                ((x.BaslangicTarihi != null && x.BitisTarihi == null) && (DbFunctions.TruncateTime(x.BaslangicTarihi) <= suAn)) ||
                ((x.BaslangicTarihi == null && x.BitisTarihi != null) && (DbFunctions.TruncateTime(x.BitisTarihi) >= suAn)) ||
                ((x.BaslangicTarihi != null && x.BitisTarihi != null) && (DbFunctions.TruncateTime(x.BaslangicTarihi) <= suAn && DbFunctions.TruncateTime(x.BitisTarihi) >= suAn))
            )
            , true, new string[] { "AnketSoru" });

            // Yayında anket var mı?
            if(yayindaOlanAnket != null)
            {
                // Kullanıcı giriş yapmış mı?
                if (girisYapanKullanici != null)
                {
                    ViewBag.Anket = yayindaOlanAnket;
                }
                else
                {
                    if(yayindaOlanAnket.ZiyaretciCevapAktifMi)
                    {
                        ViewBag.Anket = yayindaOlanAnket;
                    }
                }
            }

            return View();
        }

        public ActionResult Hata(string id)
        {
            switch (id)
            {
                default:
                case "SayfaBulunamadi":
                    ViewBag.PageProperties = PageProperties.SetPageProperties("Hata", "", "", "", "/Home/Hata");

                    ViewBag.Icerik = "Aradığınız Sayfa Bulunamadı. Lütfen Tekrar Deneyiniz.";
                    break;
            }

            return View();
        }

        public ActionResult Bakim()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Bakım", "", "", "", "/Home/Bakim");

            return View();
        }

        public ActionResult Test()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Test", "", "", "", "/Home/Test");

            return View();
        }

        public ActionResult MenuKullaniciPartial()
        {
            return PartialView("~/Views/Shared/Partials/MenuKullaniciPartial.cshtml");
        }

        public ActionResult MenuSepetPartial()
        {
            return PartialView("~/Views/Shared/Partials/MenuSepetPartial.cshtml");
        }

        public ActionResult AnketYapPartial()
        {
            var suAn = DateTime.Now.Date;
            var yayindaOlanAnket = AnketService.GetSingle(x => x.AktifMi == true && x.YayindaMi == true && (DbFunctions.TruncateTime(x.BaslangicTarihi) <= suAn && DbFunctions.TruncateTime(x.BitisTarihi) >= suAn)
            , true, new string[] { "AnketSoru" });

            ViewBag.Anket = yayindaOlanAnket;

            return PartialView("~/Views/Home/Partials/AnketYapPartial.cshtml");
        }

        public ActionResult KampanyaPartial()
        {
            var suAn = DateTime.Now.Date;
            var yayindaOlanKampanya = KampanyaService.GetFirst(x => x.AktifMi == true && x.AnaSayfadaGosterilsinMi == true &&
            (
                ((x.BaslangicTarihi != null && x.BitisTarihi == null) && (DbFunctions.TruncateTime(x.BaslangicTarihi) <= suAn)) ||
                ((x.BaslangicTarihi == null && x.BitisTarihi != null) && (DbFunctions.TruncateTime(x.BitisTarihi) >= suAn)) ||
                ((x.BaslangicTarihi != null && x.BitisTarihi != null) && (DbFunctions.TruncateTime(x.BaslangicTarihi) <= suAn && DbFunctions.TruncateTime(x.BitisTarihi) >= suAn))
            )
            , true, new string[] { "KampanyaResim" });

            ViewBag.Kampanya = yayindaOlanKampanya;

            return PartialView("~/Views/Home/Partials/KampanyaPartial.cshtml");
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult HaberBulteniAboneOl(string haberBulteniEposta)
        {
            try
            {
                var flag = HaberBulteniAbonelikService.Subscribe(haberBulteniEposta);

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult BagliKategoriGetir(int id)
        {
            try
            {
                if (id == 0)
                    return Json(null, JsonRequestBehavior.AllowGet);

                var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

                if (icerikAyar.CacheAktifMi)
                {
                    var okullar = KategoriService.GetAllActiveCategoriesWithNestedFromCache(id).Select(x => new { id = x.KategoriId, text = x.Adi }).ToList();
                    return Json(okullar, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var okullar = KategoriService.GetAllActiveCategoriesWithNested(id).Select(x => new { id = x.KategoriId, text = x.Adi }).ToList();
                    return Json(okullar, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AnketSonucKaydet(List<AnketCevap> cevaplar)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            var kullaniciGuid = girisYapanKullanici != null ? girisYapanKullanici.KullaniciId : Guid.Empty;

            var suAn = DateTime.Now;
            AnketCevap _anketCevap;
            foreach (var cevap in cevaplar)
            {
                _anketCevap = new AnketCevap();
                _anketCevap.AnketSoruId = cevap.AnketSoruId;
                _anketCevap.CevapSonuc = cevap.CevapSonuc;
                _anketCevap.KullaniciId = kullaniciGuid;
                _anketCevap.Tarih = suAn;

                AnketCevapService.Add(_anketCevap);
            }

            var flag = AnketCevapService.Save();

            return Json(flag, JsonRequestBehavior.DenyGet);
        }
        #endregion
    }
}