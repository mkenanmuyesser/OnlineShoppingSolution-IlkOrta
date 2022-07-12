using ArbakCCLib.ENTITY;
using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.Custom;
using CommerceProject.Business.Entities;
using CommerceProject.Business.Helper.Bank;
using CommerceProject.Business.Helper.Logging;
using CommerceProject.Business.Helper.PriceCalculation;
using CommerceProject.Business.Helper.Program;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static CommerceProject.Presentation.IlkOrta.Helper.PageHelper;

namespace CommerceProject.Presentation.IlkOrta.Controllers
{
    public class SepetController : BaseController
    {
        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        ISepetService SepetService;
        IKategoriService KategoriService;
        IUrunService UrunService;
        IPaketService PaketService;
        IAdresService AdresService;
        IKullaniciAdresService KullaniciAdresService;
        IGonderimService GonderimService;
        ISiparisOdemeTipService SiparisOdemeTipService;
        IHesapNumarasiService HesapNumarasiService;
        IBankaService BankaService;
        ISiparisService SiparisService;
        ITaksitService TaksitService;
        ISanalPosService SanalPosService;
        public SepetController(IIcerikAyarService iIcerikAyarService,
                               IKullaniciService iKullaniciService,
                               ISepetService iSepetService,
                               IKategoriService iKategoriService,
                               IUrunService iUrunService,
                               IPaketService iPaketService,
                               IAdresService iAdresService,
                               IKullaniciAdresService iKullaniciAdresService,
                               IGonderimService iGonderimService,
                               ISiparisOdemeTipService iSiparisOdemeTipService,
                               IHesapNumarasiService iHesapNumarasiService,
                               IBankaService iBankaService,
                               ISiparisService iSiparisService,
                               ITaksitService iTaksitService,
                               ISanalPosService iSanalPosService) : base(iIcerikAyarService,
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
            AdresService = iAdresService;
            KullaniciAdresService = iKullaniciAdresService;
            GonderimService = iGonderimService;
            SiparisOdemeTipService = iSiparisOdemeTipService;
            HesapNumarasiService = iHesapNumarasiService;
            BankaService = iBankaService;
            SiparisService = iSiparisService;
            TaksitService = iTaksitService;
            SanalPosService = iSanalPosService;
        }

        #region Actions
        public ActionResult Index()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Sepet", "", "", "", "/Sepet");

            if (ViewBag.Sepet == null || !(ViewBag.Sepet as List<Sepet>).Any())
            {
                return RedirectToAction("", "");
            }
            else
            {
                //var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

                //if (icerikAyar.CacheAktifMi)
                //    ViewBag.IliskiliUrunler = UrunService.GetAllActiveProductsFromCache().Take(18).ToList();
                //else
                //ViewBag.IliskiliUrunler = UrunService.GetAllActiveProducts().Take(18).ToList();

                ViewBag.IliskiliUrunler = new List<Urun>();

                return View();
            }
        }

        public ActionResult Odeme()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Sepet", "Ödeme", "", "", "/Sepet/Odeme");

            if (ViewBag.Sepet == null || !(ViewBag.Sepet as List<Sepet>).Any())
            {
                return RedirectToAction("", "");
            }
            else
            {
                var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();
                if (girisYapanKullanici == null)
                {
                    ViewBag.KullaniciAdres = new KullaniciAdres
                    {
                        Adres = new Adres(),
                    };
                }
                else
                {
                    ViewBag.KullaniciAdresler = KullaniciAdresService.GetUserAdresses(girisYapanKullanici.KullaniciId);
                }

                var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);
                if (icerikAyar.CacheAktifMi)
                {
                    ViewBag.Gonderimler = GonderimService.GetAllActiveShippingsFromCache();
                    ViewBag.SiparisOdemeTipleri = SiparisOdemeTipService.GetAllActiveOrderPaymentTypesFromCache();
                }
                else
                {
                    ViewBag.Gonderimler = GonderimService.GetAllActiveShippings();
                    ViewBag.SiparisOdemeTipleri = SiparisOdemeTipService.GetAllActiveOrderPaymentTypes();
                }

                return View();
            }
        }

        public ActionResult OdemeSonuc()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Sepet", "Ödeme Sonuc", "", "", "/Sepet/OdemeSonuc");

            string hataMesaji = Request.Form.Get("errmsg");
            var siparisSonuc = TempData["SiparisSonuc"] as SiparisSonucDataObj;

            if (siparisSonuc == null)
            {
                LogHelper.LogKaydet(LogLevel.Error, "OdemeSonuc null bilgi");
                return RedirectToAction("Odeme", "Sepet");
            }
            else if (!string.IsNullOrEmpty(hataMesaji))
            {
                LogHelper.LogKaydet(LogLevel.Error, "OdemeSonuc : " + hataMesaji);
            }
            else
            {
                // sipariş başarılı bir şekilde bitirildiği için sepettekileri silelim
                if (siparisSonuc.SiparisBasariliMi)
                {
                    var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();
                    if (girisYapanKullanici == null)
                    {
                        SepetService.ClearUserProductsFromBasket(Guid.Empty);
                    }
                    else
                    {
                        SepetService.SetProcessUserProductsFromBasket(girisYapanKullanici.KullaniciId);
                    }

                    ViewBag.Sepet = new List<Sepet>();
                }
                else
                {
                    LogHelper.LogKaydet(LogLevel.Error, "Sipariş başarısız");
                }
            }

            return View();
        }

        public ActionResult KullaniciGirisPartial()
        {
            return PartialView("~/Views/Sepet/Partials/KullaniciGirisPartial.cshtml");
        }

        public ActionResult AdresBilgiPartial()
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();
            if (girisYapanKullanici == null)
            {
                ViewBag.KullaniciAdres = new KullaniciAdres
                {
                    Adres = new Adres(),
                };
            }
            else
            {
                ViewBag.KullaniciAdresler = KullaniciAdresService.GetUserAdresses(girisYapanKullanici.KullaniciId);
            }

            return PartialView("~/Views/Sepet/Partials/AdresBilgiPartial.cshtml");
        }

        public ActionResult KrediKartiOdemePartial()
        {
            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            var sepettekiUrunler = ViewBag.Sepet as List<Sepet>;
            var toplamfiyat = sepettekiUrunler.Sum(x => x.Adet * x.Urun.KdvDahilTutar);
            //toplam fiyat üzerinden taksitli fiyatlar hesaplanacak
            if (icerikAyar.CacheAktifMi)
            {
                ViewBag.Bankalar = BankaService.GetInstallmentCalculationDataByPriceFromCache(toplamfiyat);
            }
            else
            {
                ViewBag.Bankalar = BankaService.GetInstallmentCalculationDataByPrice(toplamfiyat);
            }

            return PartialView("~/Views/Sepet/Partials/KrediKartiOdemePartial.cshtml");
        }

        public ActionResult BankaHavaleOdemePartial()
        {
            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            if (icerikAyar.CacheAktifMi)
                ViewBag.BankaHesapNumaralari = HesapNumarasiService.GetAllActiveBankAccountNumbersFromCache();
            else
                ViewBag.BankaHesapNumaralari = HesapNumarasiService.GetAllActiveBankAccountNumbers();

            return PartialView("~/Views/Sepet/Partials/BankaHavaleOdemePartial.cshtml");
        }

        public ActionResult KapidaOdemePartial()
        {
            return PartialView("~/Views/Sepet/Partials/KapidaOdemePartial.cshtml");
        }

        public ActionResult OdemeTipPartial()
        {
            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);
            if (icerikAyar.CacheAktifMi)
                ViewBag.SiparisOdemeTipleri = SiparisOdemeTipService.GetAllActiveOrderPaymentTypesFromCache();
            else
                ViewBag.SiparisOdemeTipleri = SiparisOdemeTipService.GetAllActiveOrderPaymentTypes();

            return PartialView("~/Views/Sepet/Partials/OdemeTipPartial.cshtml");
        }

        public ActionResult GonderimTipPartial()
        {
            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);
            if (icerikAyar.CacheAktifMi)
                ViewBag.Gonderimler = GonderimService.GetAllActiveShippingsFromCache();
            else
                ViewBag.Gonderimler = GonderimService.GetAllActiveShippings();

            return PartialView("~/Views/Sepet/Partials/GonderimTipPartial.cshtml");
        }

        public ActionResult SiparisOzetPartial(SiparisOzetDataObj siparisOzetDataObj)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();
            if (girisYapanKullanici == null)
                siparisOzetDataObj.KullaniciId = Guid.Empty;
            else
                siparisOzetDataObj.KullaniciId = girisYapanKullanici.KullaniciId;

            var sepettekiUrunler = ViewBag.Sepet as List<Sepet>;
            siparisOzetDataObj.SepettekiUrunler = sepettekiUrunler;

            var siparisOzet = PriceCalculationHelper.PriceSummary(siparisOzetDataObj);

            if (siparisOzet == null)
            {
                return RedirectToAction("Hata", "Home");
            }
            else
            {
                return PartialView("~/Views/Sepet/Partials/SiparisOzetPartial.cshtml", siparisOzet);
            }
        }

        // view yok
        public ActionResult DDDPost()
        {
            var siparisSonuc = TempData["SiparisSonuc"] as SiparisSonucDataObj;

            if (siparisSonuc == null || siparisSonuc.KrediKartiBilgi == null || siparisSonuc.BankaId == null)
            {
                LogHelper.LogKaydet(LogLevel.Error, "DDDPost null bilgi");
                return RedirectToAction("Hata", "Home");
            }
            else
            {
                var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

                PosForm dddPosForm = new PosForm()
                {
                    OrderID = siparisSonuc.SiparisId.ToString(),
                    Amount = Convert.ToDecimal(siparisSonuc.OdenecekTutar.ToString("#.##")),
                    Email = icerikAyar.GonderilecekEposta,
                    IPAdress = BankTool.GetIp(),
                    CcNumber = siparisSonuc.KrediKartiBilgi.KartNumarasi.Replace("-", "").Trim(),
                    Cvc = siparisSonuc.KrediKartiBilgi.GuvenlikKodu.ToString().Trim(),
                    ExpireMonth = siparisSonuc.KrediKartiBilgi.Ay.ToString(),
                    ExpireYear = siparisSonuc.KrediKartiBilgi.Yil.ToString(),
                    Installment = siparisSonuc.KrediKartiBilgi.TaksitSayisi,
                };

                TempData["SiparisSonuc"] = siparisSonuc;

                switch (siparisSonuc.BankaId.Value)
                {
                    case 1:
                        BankDDDHelper.Controller.CCManager.SendPayment(System.Web.HttpContext.Current, dddPosForm, ArbakCCLib.ENTITY.ENUMS.Banks.GARANTI);
                        break;
                    case 2:
                        BankDDDHelper.Controller.CCManager.SendPayment(System.Web.HttpContext.Current, dddPosForm, ArbakCCLib.ENTITY.ENUMS.Banks.YAPIKREDI);
                        break;
                    case 3:
                        // vakıfbank yok
                        break;
                    case 4:
                        BankDDDHelper.Controller.CCManager.SendPayment(System.Web.HttpContext.Current, dddPosForm, ArbakCCLib.ENTITY.ENUMS.Banks.AKBANK);
                        break;
                    case 5:
                        BankDDDHelper.Controller.CCManager.SendPayment(System.Web.HttpContext.Current, dddPosForm, ArbakCCLib.ENTITY.ENUMS.Banks.ISBANKASI);
                        break;
                    case 6:
                        BankDDDHelper.Controller.CCManager.SendPayment(System.Web.HttpContext.Current, dddPosForm, ArbakCCLib.ENTITY.ENUMS.Banks.FINANSBANK);
                        break;
                    case 7:
                        // denizbank yok
                        break;
                    case 8:
                        BankDDDHelper.Controller.CCManager.SendPayment(System.Web.HttpContext.Current, dddPosForm, ArbakCCLib.ENTITY.ENUMS.Banks.HALKBANK);
                        break;
                }

                return null;
            }
        }

        // view yok
        public ActionResult DDDOdemeYonlendir()
        {
            var siparisSonuc = TempData["SiparisSonuc"] as SiparisSonucDataObj;
            if (siparisSonuc == null || siparisSonuc.KrediKartiBilgi == null || siparisSonuc.BankaId == null)
            {
                LogHelper.LogKaydet(LogLevel.Error, "DDDOdemeYonlendir null bilgi");
                return RedirectToAction("Hata", "Home");
            }
            else
            {
                if (Request.QueryString["bank"] != null && Request.QueryString["OrderID"] != null)
                {
                    TempData["SiparisSonuc"] = siparisSonuc;

                    BankDDDHelper.Controller.CCManager.ReceivePayment(System.Web.HttpContext.Current.Request, System.Web.HttpContext.Current.Response);

                    return null;
                }
                else
                {
                    LogHelper.LogKaydet(LogLevel.Error, "DDDOdemeYonlendir querystring null bilgi");
                    return RedirectToAction("Hata", "Home");
                }
            }
        }

        // view yok
        public ActionResult DDDOdemeOnay()
        {
            var siparisSonuc = TempData["SiparisSonuc"] as SiparisSonucDataObj;

            if (siparisSonuc == null || siparisSonuc.KrediKartiBilgi == null || siparisSonuc.BankaId == null)
            {
                LogHelper.LogKaydet(LogLevel.Error, "DDDOdemeOnay null bilgi");
                return RedirectToAction("Hata", "Home");
            }
            else
            {
                if (Request.QueryString["bank"] != null && Request.QueryString["OrderID"] != null)
                {
                    PaymentResult result = BankDDDHelper.Controller.CCManager.ConfirmPayment(System.Web.HttpContext.Current.Server, System.Web.HttpContext.Current.Request, System.Web.HttpContext.Current.Session);
                    if (result.Result)
                    {
                        // burada gerekli bilgileri kaydedip siparişi aktife çekeceğiz
                        var siparis = SiparisService.GetById(siparisSonuc.SiparisId);
                        if (siparis != null && !siparis.AktifMi)
                        {
                            siparis.AktifMi = true;
                            siparis.KrediKartiTransferId = result.TransID;
                            SiparisService.Save();

                            siparisSonuc.TransferId = result.TransID;
                            siparisSonuc.SiparisBasariliMi = true;

                            TempData["SiparisSonuc"] = siparisSonuc;

                            return RedirectToAction("OdemeSonuc", "Sepet");
                        }
                        else
                        {
                            LogHelper.LogKaydet(LogLevel.Error, "DDDOdemeOnay yanlış sipariş veya aktif sipariş : SiparisId(" + siparisSonuc.SiparisId + ")");
                            return RedirectToAction("Hata", "Home");
                        }
                    }
                    else
                    {
                        LogHelper.LogKaydet(LogLevel.Error, result.Description);
                        return RedirectToAction("Hata", "Home");
                    }
                }
                else
                {
                    LogHelper.LogKaydet(LogLevel.Error, "DDDOdemeOnay querystring null bilgi");
                    return RedirectToAction("Hata", "Home");
                }
            }
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult SepetAra()
        {
            var sonucListesi = ViewBag.Sepet as List<Sepet>;

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    UrunId = x.UrunId,
                    UrunAdi = x.Urun.Adi,
                    ThumbResimVarmi = x.Urun.ThumbResim == null ? false : true,
                    ThumbResim = x.Urun.ThumbResim == null ? "" : x.Urun.ThumbResim.ResimYolu,
                    KdvDahilTutar = string.Format("{0:N}", x.Urun.KdvDahilTutar),
                    Adet = x.Adet,
                    ToplamTutar = string.Format("{0:N}", (x.Urun.KdvDahilTutar * x.Adet)),
                }).ToList(),
                urunFiyati = string.Format("{0:N} <i class='fa fa-try'></i>", sonucListesi.Sum(x => x.Adet * x.Urun.KdvHaricTutar)),
                kdv = string.Format("{0:N} <i class='fa fa-try'></i>", sonucListesi.Sum(x => x.Adet * x.Urun.KdvTutar)),
                araToplamTutar = string.Format("{0:N} <i class='fa fa-try'></i>", sonucListesi.Sum(x => x.Adet * x.Urun.KdvDahilTutar))
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SepetUrunSil(int id)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            var flag = SepetService.RemoveProductsFromBasket(girisYapanKullanici == null ? Guid.Empty : girisYapanKullanici.KullaniciId, id);

            return Json(flag, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SepeteEkle(int urunId, int adet)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            var flag = SepetService.SetUserProductsFromBasket(girisYapanKullanici == null ? Guid.Empty : girisYapanKullanici.KullaniciId, urunId, adet);

            if (flag == true)
                return Json(flag, JsonRequestBehavior.DenyGet);
            else
                return Json("", JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SepettenCikar(int urunId, int adet)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            var flag = SepetService.RemoveUserProductsFromBasket(girisYapanKullanici == null ? Guid.Empty : girisYapanKullanici.KullaniciId, urunId, adet);

            if (flag == true)
                return Json(flag, JsonRequestBehavior.DenyGet);
            else
                return Json("", JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public JsonResult SepetiTemizle()
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            var flag = SepetService.ClearUserProductsFromBasket(girisYapanKullanici == null ? Guid.Empty : girisYapanKullanici.KullaniciId);

            if (flag == true)
                return Json(flag, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SepetBilgiGoster()
        {
            var sepettekiUrunler = ViewBag.Sepet as List<Sepet>;

            return Json(sepettekiUrunler, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SepetePaketEkleSecimli(int paketId, int adet, List<int> seciliUrunler)
        {
            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            Paket paket;
            if (icerikAyar.CacheAktifMi)
                paket = PaketService.GetSinglePackageFromCache(paketId, seciliUrunler);
            else
                paket = PaketService.GetSinglePackage(paketId, seciliUrunler);


            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            var flag = SepetService.SetUserPackagesFromBasket(girisYapanKullanici == null ? Guid.Empty : girisYapanKullanici.KullaniciId, adet, paket);

            if (flag == true)
                return Json(flag, JsonRequestBehavior.DenyGet);
            else
                return Json("", JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SepetePaketEkle(int paketId, int adet)
        {
            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            Paket paket;
            if (icerikAyar.CacheAktifMi)
                paket = PaketService.GetSinglePackageFromCache(paketId);
            else
                paket = PaketService.GetSinglePackage(paketId);


            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            var flag = SepetService.SetUserPackagesFromBasket(girisYapanKullanici == null ? Guid.Empty : girisYapanKullanici.KullaniciId, adet, paket);

            if (flag == true)
                return Json(flag, JsonRequestBehavior.DenyGet);
            else
                return Json("", JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult HemenPaketAlSecimli(int paketId, int adet, List<int> seciliUrunler)
        {
            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            Paket paket;
            if (icerikAyar.CacheAktifMi)
                paket = PaketService.GetSinglePackageFromCache(paketId, seciliUrunler);
            else
                paket = PaketService.GetSinglePackage(paketId, seciliUrunler);


            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            var flag = SepetService.SetUserPackagesFromBasket(girisYapanKullanici == null ? Guid.Empty : girisYapanKullanici.KullaniciId, adet, paket);

            if (flag == true)
                return Json("/Sepet", JsonRequestBehavior.DenyGet);
            else
                return Json("", JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult HemenAl(int urunId, int adet)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            var flag = SepetService.SetUserProductsFromBasket(girisYapanKullanici == null ? Guid.Empty : girisYapanKullanici.KullaniciId, urunId, adet);

            if (flag == true)
                return Json("/Sepet", JsonRequestBehavior.DenyGet);
            else
                return Json("", JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult HemenPaketAl(int paketId, int adet)
        {
            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            Paket paket;
            if (icerikAyar.CacheAktifMi)
                paket = PaketService.GetSinglePackageFromCache(paketId);
            else
                paket = PaketService.GetSinglePackage(paketId);


            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            var flag = SepetService.SetUserPackagesFromBasket(girisYapanKullanici == null ? Guid.Empty : girisYapanKullanici.KullaniciId, adet, paket);

            if (flag == true)
                return Json("/Sepet", JsonRequestBehavior.DenyGet);
            else
                return Json("", JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SiparisTamamla(SiparisOzetDataObj siparisOzetDataObj)
        {
            bool dddSanalPosIslemiMi = false;

            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();
            if (girisYapanKullanici == null)
                siparisOzetDataObj.KullaniciId = Guid.Empty;
            else
                siparisOzetDataObj.KullaniciId = girisYapanKullanici.KullaniciId;

            var sepettekiUrunler = ViewBag.Sepet as List<Sepet>;

            siparisOzetDataObj.SepettekiUrunler = sepettekiUrunler;
            var siparisSonuc = PriceCalculationHelper.PriceSummary(siparisOzetDataObj);

            // Aşama 1 - Eğer ödeme tipi kredi kartı seçilmiş ise kredi kartı doğrulaması ve ödeme sonucu doğru olarak dönmesi gerekiyor.
            if (siparisOzetDataObj.OdemeTipId == 1)
            {
                if (siparisOzetDataObj.KrediKartiBilgi != null)
                {
                    var krediKartiBilgi = siparisOzetDataObj.KrediKartiBilgi;
                    siparisSonuc.KrediKartiBilgi = krediKartiBilgi;
                    var bankalar = BankaService.FindBy(x => x.AktifMi && x.SanalPos.Any(p => p.AktifMi), false, new string[] { "SanalPos" }).OrderBy(x => x.Sira);
                    if (bankalar.Any())
                    {
                        // Aşama 2 - Eğer taksit seçilmemiş, peşin ödeme olacak ise siradaki ilk bankaya ait sanal pos bilgilerini alacak ve işlem yapacak
                        if (siparisOzetDataObj.TaksitId.Value == 0)
                        {
                            // ilk banka bilgilerini al ve sanal pos kaydı varmı bak
                            var banka = bankalar.First();
                            var sanalPos = banka.SanalPos.First();
                            siparisOzetDataObj.BankaId = banka.BankaId;
                            siparisSonuc.BankaId = banka.BankaId;

                            // Burası önemli dikkat! Sanal pos bilgisini alıyoruz. eğer sanal pos 3d aktif ise siparişi pasif yapamız ve sepeti silmememiz gerekiyor. 

                            if (sanalPos.DDDAktifMi)
                            {
                                dddSanalPosIslemiMi = true;
                            }
                            else
                            {
                                // işlem 3d işlemi değilse bankaya bilgileri gönderiyoruz.

                                // kullanıcıdan kart bilgilerini alıyoruz.
                                VirtualPosForm posForm = new VirtualPosForm
                                {
                                    ay = krediKartiBilgi.Ay,
                                    yil = krediKartiBilgi.Yil,
                                    guvenlikKodu = krediKartiBilgi.GuvenlikKodu,
                                    kartNumarasi = Convert.ToInt64(krediKartiBilgi.KartNumarasi.Replace("-", "").Trim()),
                                    kartSahibi = krediKartiBilgi.KartSahibi,
                                    taksit = 0,
                                    tutar = Convert.ToDouble(siparisSonuc.OdenecekTutar),
                                };

                                var posResult = PosIslemYap(banka.BankaId, posForm);
                                var result = PosIslemSonuc(posResult);

                                if (posResult.sonuc == false)
                                    return Json(result, JsonRequestBehavior.DenyGet);
                            }
                        }
                        else
                        {
                            // Aşama 3 - Eğer taksit seçimi yapılmış ise banka seçimi de otomatik olarak yapılmış oluyor. O bankaya ait sanal pos bilgilerini alacak ve işlem yapacak
                            var taksitBilgi = TaksitService.FindBy(x => x.TaksitId == siparisOzetDataObj.TaksitId.Value, false, new string[] { "Banka", "Banka.SanalPos" }).SingleOrDefault();
                            if (taksitBilgi != null)
                            {
                                var banka = taksitBilgi.Banka;
                                var sanalPos = banka.SanalPos.First();
                                siparisOzetDataObj.BankaId = banka.BankaId;
                                siparisSonuc.BankaId = banka.BankaId;
                                siparisSonuc.KrediKartiBilgi.TaksitSayisi = taksitBilgi.TaksitSayisi;

                                if (sanalPos.DDDAktifMi)
                                {
                                    dddSanalPosIslemiMi = true;
                                }
                                else
                                {
                                    // işlem 3d işlemi değilse bankaya bilgileri gönderiyoruz.

                                    // kullanıcıdan kart bilgilerini alıyoruz.
                                    VirtualPosForm posForm = new VirtualPosForm
                                    {
                                        ay = krediKartiBilgi.Ay,
                                        yil = krediKartiBilgi.Yil,
                                        guvenlikKodu = krediKartiBilgi.GuvenlikKodu,
                                        kartNumarasi = Convert.ToInt64(krediKartiBilgi.KartNumarasi.Replace("-", "").Trim()),
                                        kartSahibi = krediKartiBilgi.KartSahibi,
                                        taksit = taksitBilgi.TaksitSayisi,
                                        tutar = Convert.ToDouble(siparisSonuc.OdenecekTutar),
                                    };

                                    var posResult = PosIslemYap(banka.BankaId, posForm);
                                    var result = PosIslemSonuc(posResult);

                                    if (posResult.sonuc == false)
                                        return Json(new { message = result }, JsonRequestBehavior.DenyGet);
                                }
                            }
                            else
                            {
                                return Json("", JsonRequestBehavior.DenyGet);
                            }
                        }
                    }
                    else
                    {
                        return Json("", JsonRequestBehavior.DenyGet);
                    }
                }
                else
                {
                    return Json("", JsonRequestBehavior.DenyGet);
                }
            }

            if (girisYapanKullanici == null)
            {
                siparisSonuc.KullaniciId = siparisOzetDataObj.KullaniciId;
                siparisSonuc.Kullanici = KullaniciService.GetSingle(x => x.KullaniciId == Guid.Empty, false);

                //eğer kullanıcı giriş yapmadıysa adres kaydı oluşturulacak.

                Adres adres = siparisOzetDataObj.AdresData;
                adres.Tarih = DateTime.Now;
                adres.AktifMi = true;

                AdresService.Add(adres);
                AdresService.Save();

                siparisSonuc.GonderimAdres = adres;
                siparisSonuc.FaturaAdres = adres;

                siparisOzetDataObj.FaturaAdresId = adres.AdresId;
                siparisOzetDataObj.GonderimAdresId = adres.AdresId;
            }
            else
            {
                siparisSonuc.KullaniciId = siparisOzetDataObj.KullaniciId;
                siparisSonuc.Kullanici = girisYapanKullanici;
                siparisSonuc.GonderimAdres = KullaniciAdresService.GetUserAdress(girisYapanKullanici.KullaniciId, siparisOzetDataObj.GonderimAdresId.Value).Adres;
                siparisSonuc.FaturaAdres = KullaniciAdresService.GetUserAdress(girisYapanKullanici.KullaniciId, siparisOzetDataObj.FaturaAdresId.Value).Adres;
            }

            //sipariş kaydı oluşturulacak
            var siparis = new Siparis
            {
                KullaniciId = siparisOzetDataObj.KullaniciId,
                FaturaAdresId = siparisOzetDataObj.FaturaAdresId.Value,
                GonderimAdresId = siparisOzetDataObj.GonderimAdresId.Value,
                SiparisOdemeTipId = siparisOzetDataObj.OdemeTipId,
                // Kredi kartı seçildiyse onaylandı(3dsiz), diğer durumlarda ödeme durumu beklemede,
                SiparisDurumTipId = siparisOzetDataObj.OdemeTipId == 1 && !dddSanalPosIslemiMi ? 3 : 1,
                // Kredi kartı seçildiyse ödendi(3dsiz), diğer durumlarda ödeme durumu beklemede,
                OdemeDurumTipId = siparisOzetDataObj.OdemeTipId == 1 ? 2 : 1,
                KrediKartiTipId = siparisOzetDataObj.OdemeTipId == 1 ? (int?)siparisOzetDataObj.KrediKartiBilgi.KrediKartiTipId : null,
                KrediKartiBankaId = siparisOzetDataObj.OdemeTipId == 1 ? (int?)siparisOzetDataObj.BankaId.Value : null,
                KrediKartiAdSoyad = siparisOzetDataObj.OdemeTipId == 1 ? siparisOzetDataObj.KrediKartiBilgi.KartSahibi : null,
                // maskeli numaraya bir işlem yapılmadı buraya bakılacak.
                KrediKartiMaskeliNumara = siparisOzetDataObj.OdemeTipId == 1 ? siparisOzetDataObj.KrediKartiBilgi.KrediKartiMaskeliNumara : null,
                KrediKartiTaksit = siparisOzetDataObj.OdemeTipId == 1 ? (int?)siparisOzetDataObj.KrediKartiBilgi.Taksit : null,
                KrediKartiTransferId = null,
                ToplamIskonto = siparisSonuc.ToplamIskonto,
                ToplamKomisyon = siparisSonuc.ToplamKomisyon,
                KullanilanParaPuanMiktar = siparisSonuc.ParaPuanMiktar,
                KalanParaPuanMiktar = 0,
                KuponMiktar = siparisSonuc.KuponMiktar,
                UrunKdvDahilToplamTutar = siparisSonuc.UrunToplam.KdvDahilTutar,
                UrunKdvHaricToplamTutar = siparisSonuc.UrunToplam.KdvHaricTutar,
                UrunKdvToplamTutar = siparisSonuc.UrunToplam.KdvTutar,
                OdemeTipUcreti = siparisSonuc.OdemeTipUcreti,
                GonderimUcreti = siparisSonuc.GonderimUcreti,
                OdenecekTutar = siparisSonuc.OdenecekTutar,
                IadeToplam = 0,
                Aciklama = "",
                KullaniciIp = ProgramHelper.GetClientIpAddress(Request),
                SiparisTarihi = DateTime.Now,
                SonIslemTarihi = DateTime.Now,
                OdemeTarihi = siparisOzetDataObj.OdemeTipId == 1 ? (DateTime?)DateTime.Now : null,
                // 3d sanal pos ise sipariş pasif olacak
                AktifMi = dddSanalPosIslemiMi ? false : true
            };

            //bu siparişe bağlı sipariş detayları yani ürünler girilecek.
            foreach (var sepettekiUrun in siparisSonuc.SepettekiUrunler)
            {
                //FiyatDataObj fiyatDataObj = PriceCalculationHelper.PriceDetail(sepettekiUrun.Urun.Fiyat, sepettekiUrun.Urun.Vergi.VergiOrani);
                var siparisDetay = new SiparisDetay
                {
                    Siparis = siparis,
                    UrunId = sepettekiUrun.UrunId,
                    Adet = sepettekiUrun.Adet,
                    UrunBirimKdvDahilTutar = sepettekiUrun.Urun.UrunBirimKdvDahilTutar,
                    UrunBirimKdvHaricTutar = sepettekiUrun.Urun.UrunBirimKdvHaricTutar,
                    UrunBirimKdvTutar = sepettekiUrun.Urun.UrunBirimKdvTutar,
                    HesaplananBirimKdvDahilTutar = sepettekiUrun.Urun.KdvDahilTutar / sepettekiUrun.Adet,
                    HesaplananBirimKdvHaricTutar = sepettekiUrun.Urun.KdvHaricTutar / sepettekiUrun.Adet,
                    HesaplananBirimKdvTutar = sepettekiUrun.Urun.KdvTutar / sepettekiUrun.Adet,
                    HesaplananKdvDahilTutar = sepettekiUrun.Urun.KdvDahilTutar,
                    HesaplananKdvHaricTutar = sepettekiUrun.Urun.KdvHaricTutar,
                    HesaplananKdvTutar = sepettekiUrun.Urun.KdvTutar,
                    AktifMi = true
                };

                siparis.SiparisDetay.Add(siparisDetay);
            }

            // sipariş kaydı yapılacak
            SiparisService.Add(siparis);
            var flag = SiparisService.Save();

            if (flag == true)
            {
                siparisSonuc.SiparisId = siparis.SiparisId;
                siparisSonuc.SiparisTarihi = siparis.SiparisTarihi;
                siparisSonuc.OdemeTipi = SiparisOdemeTipService.GetById(siparis.SiparisOdemeTipId).Adi;

                if (dddSanalPosIslemiMi)
                {
                    siparisSonuc.SiparisBasariliMi = false;
                    TempData["SiparisSonuc"] = siparisSonuc;
                    return Json(new { success = "true", location = "/Sepet/DDDPost" }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    siparisSonuc.SiparisBasariliMi = true;
                    siparisSonuc.KrediKartiBilgi = null;
                    TempData["SiparisSonuc"] = siparisSonuc;
                    return Json(new { success = "true", location = "/Sepet/OdemeSonuc" }, JsonRequestBehavior.DenyGet);
                }
            }
            else
                return Json("", JsonRequestBehavior.DenyGet);
        }

        private VirtualPosResultForm PosIslemYap(int bankaId, VirtualPosForm posForm)
        {
            VirtualPosResultForm resultForm = null;
            SanalPos sanalPos = SanalPosService.GetSingle(x => x.BankaId == bankaId);

            switch (bankaId)
            {
                case 1:
                    resultForm = BankHelper.GarantiBankasi(posForm, sanalPos);
                    break;
                case 2:
                    resultForm = BankHelper.YapiKrediBankasi(posForm);
                    break;
                case 3:
                    resultForm = BankHelper.VakifBank(posForm);
                    break;
                case 4:
                    resultForm = BankHelper.AkBank(posForm);
                    break;
                case 5:
                    resultForm = BankHelper.IsBankasi(posForm);
                    break;
                case 6:
                    resultForm = BankHelper.FinansBank(posForm);
                    break;
                case 7:
                    resultForm = BankHelper.DenizBank(posForm);
                    break;
            }

            return resultForm;
        }

        private string PosIslemSonuc(VirtualPosResultForm posResultForm)
        {
            // işlem yapılsın
            StringBuilder sb = new StringBuilder();
            if (posResultForm.sonuc)
            {
                // Çekim işlemi başarılı ise, geri dönen bilgileri alıyoruz.
                // Genellikle bu bilgiler veritabanında saklanır.
                // Bankadan bankaya değişiklik göstereceği için, alanlardan bazıları boş gelebilir.
                sb.Append(posResultForm.referansNo);
                sb.Append(posResultForm.groupId);
                sb.Append(posResultForm.transId);
                sb.Append(posResultForm.code);
            }
            else
            {
                // Çekim işlemi herhangi bir sebepden dolayı olumsuz sonuçlanmışsa, bankadan dönen hatayı alıyoruz.
                // Hata kodlarının açıklamaları ilgili banka dökümantasyonunda belirtilmiştir.
                sb.Append(posResultForm.sonuc);
                sb.Append(posResultForm.hataMesaji);
                sb.Append(posResultForm.hataKodu);
            }

            return sb.ToString();
        }
        #endregion
    }
}