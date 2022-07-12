using BotDetect.Web;
using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.Custom;
using CommerceProject.Business.Entities;
using CommerceProject.Business.Helper.Email;
using CommerceProject.Business.Helper.Logging;
using CommerceProject.Presentation.IlkOrta.Helper;
using NLog;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using static CommerceProject.Presentation.IlkOrta.Helper.PageHelper;

namespace CommerceProject.Presentation.IlkOrta.Controllers
{
    public class KullaniciController : BaseController
    {
        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        ISepetService SepetService;
        IKategoriService KategoriService;
        IKullaniciDetayService KullaniciDetayService;
        ISiparisService SiparisService;
        IIstekListesiService IstekListesiService;
        IUrunYorumService UrunYorumService;
        IAdresService AdresService;
        IAdresIlService AdresIlService;
        IAdresIlceService AdresIlceService;
        IFaturaTipService FaturaTipService;
        IKullaniciAdresService KullaniciAdresService;
        IIadeTalepService IadeTalepService;
        IIadeTalepNedenTipService IadeTalepNedenTipService;
        IIadeTalepIstekTipService IadeTalepIstekTipService;
        ISiparisDetayService SiparisDetayService;
        ISirketService SirketService;
        IGonderimService GonderimService;
        public KullaniciController(IIcerikAyarService iIcerikAyarService,
                                   IKullaniciService iKullaniciService,
                                   ISepetService iSepetService,
                                   IKategoriService iKategoriService,
                                   IKullaniciDetayService iKullaniciDetayService,
                                   ISiparisService iSiparisService,
                                   IIstekListesiService iIstekListesiService,
                                   IUrunYorumService iUrunYorumService,
                                   IAdresService iAdresService,
                                   IAdresIlService iAdresIlService,
                                   IAdresIlceService iAdresIlceService,
                                   IFaturaTipService iFaturaTipService,
                                   IKullaniciAdresService iKullaniciAdresService,
                                   IIadeTalepService iIadeTalepService,
                                   IIadeTalepNedenTipService iIadeTalepNedenTipService,
                                   IIadeTalepIstekTipService iIadeTalepIstekTipService,
                                   ISiparisDetayService iSiparisDetayService,
                                   ISirketService iSirketService,
                                   IGonderimService iGonderimService) : base(iIcerikAyarService,
                                                                             iKullaniciService,
                                                                             iSepetService,
                                                                             iKategoriService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;
            SepetService = iSepetService;
            KategoriService = iKategoriService;

            KullaniciDetayService = iKullaniciDetayService;
            SiparisService = iSiparisService;
            IstekListesiService = iIstekListesiService;
            UrunYorumService = iUrunYorumService;
            AdresService = iAdresService;
            AdresIlService = iAdresIlService;
            AdresIlceService = iAdresIlceService;
            FaturaTipService = iFaturaTipService;
            KullaniciAdresService = iKullaniciAdresService;
            IadeTalepService = iIadeTalepService;
            IadeTalepNedenTipService = iIadeTalepNedenTipService;
            IadeTalepIstekTipService = iIadeTalepIstekTipService;
            SiparisDetayService = iSiparisDetayService;
            SirketService = iSirketService;
            GonderimService = iGonderimService;
        }

        #region Actions

        public ActionResult Giris()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Kullanıcı", "Giriş İşlemleri", "", "", "/Kullanici/Giris");

            // LogHelper.LogKaydet(LogLevel.Info, "Kullanıcı Giriş");

            return View();
        }

        public ActionResult SifreGonder()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Kullanıcı", "Şifre Gönder", "", "", "/Kullanici/SifreGonder");

            return View();
        }

        public ActionResult Siparis()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Kullanıcı", "Siparişler", "", "", "/Kullanici/Siparis");

            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);
            if (icerikAyar.CacheAktifMi)
                ViewBag.Gonderimler = GonderimService.GetAllActiveShippingsFromCache();
            else
                ViewBag.Gonderimler = GonderimService.GetAllActiveShippings();

            return View();
        }

        [AuthorizeManager]
        public ActionResult Index()
        {
            if (ViewBag.Kullanici == null)
                return Redirect("~");

            ViewBag.PageProperties = PageProperties.SetPageProperties("Kullanıcı", "", "", "", "/Kullanici");

            return View();
        }

        [AuthorizeManager]
        public ActionResult Bilgi()
        {
            if (ViewBag.Kullanici == null)
                return Redirect("~");

            ViewBag.PageProperties = PageProperties.SetPageProperties("Kullanıcı", "Bilgi", "", "", "/Kullanici/Bilgi");

            var girisYapanKullanici = ViewBag.Kullanici as Kullanici;
            ViewBag.KullaniciDetay = girisYapanKullanici.KullaniciDetay;

            return View();
        }

        [AuthorizeManager]
        public ActionResult Adres()
        {
            if (ViewBag.Kullanici == null)
                return Redirect("~");

            ViewBag.PageProperties = PageProperties.SetPageProperties("Kullanıcı", "Adresler", "", "", "/Kullanici/Adres");

            return View();
        }

        [AuthorizeManager]
        public ActionResult Yorum()
        {
            if (ViewBag.Kullanici == null)
                return Redirect("~");

            ViewBag.PageProperties = PageProperties.SetPageProperties("Kullanıcı", "Yorumlar", "", "", "/Kullanici/Yorum");

            return View();
        }

        [AuthorizeManager]
        public ActionResult IstekListesi()
        {
            if (ViewBag.Kullanici == null)
                return Redirect("~");

            ViewBag.PageProperties = PageProperties.SetPageProperties("Kullanıcı", "İstek Listesi", "", "", "/Kullanici/IstekListesi");

            return View();
        }

        public ActionResult UyeGirisPartial()
        {
            return PartialView("~/Views/Kullanici/Partials/UyeGirisPartial.cshtml");
        }

        public ActionResult UyeKayitPartial()
        {
            ViewBag.SirketListesi = SirketService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.SirketId, text = x.Adi }).ToList();

            return PartialView("~/Views/Kullanici/Partials/UyeKayitPartial.cshtml");
        }

        public ActionResult AdresDetayPartial(string currentController, int? id = null)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            ViewBag.FaturaTipListesi = FaturaTipService.GetAll(false).Select(x => new { id = x.FaturaTipId, text = x.Adi }).ToList();
            ViewBag.CurrentController = currentController;

            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            if (icerikAyar.CacheAktifMi)
                ViewBag.IlListesi = AdresIlService.GetAllActiveCitiesFromCache().Select(x => new { id = x.AdresIlId, text = x.IlAdi });
            else
                ViewBag.IlListesi = AdresIlService.GetAllActiveCities().Select(x => new { id = x.AdresIlId, text = x.IlAdi });

            if (girisYapanKullanici == null)
            {
                ViewBag.KullaniciAdres = new KullaniciAdres
                {
                    Adres = new Adres(),
                };
            }
            else
            {
                if (id != null)
                {
                    var kullaniciAdres = KullaniciAdresService.GetUserAdress(girisYapanKullanici.KullaniciId, id.Value);
                    if (kullaniciAdres == null)
                    {
                        return Redirect("~");
                    }
                    else
                    {
                        ViewBag.KullaniciAdres = kullaniciAdres;
                    }
                }
                else
                {
                    ViewBag.KullaniciAdres = new KullaniciAdres
                    {
                        Adres = new Adres(),
                    };
                }
            }

            return PartialView("~/Views/Kullanici/Partials/AdresDetayPartial.cshtml");
        }

        public ActionResult SiparisDetayPartial(int siparisId)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                ViewBag.Siparis = new Siparis();
            }
            else
            {
                var siparis = SiparisService.GetSingle(x => x.SiparisId == siparisId, true, new string[] { "Adres", "Adres.FaturaTip", "Adres.AdresIl", "Adres.AdresIlce", "Adres1", "Adres1.FaturaTip", "Adres1.AdresIl", "Adres1.AdresIlce", "SiparisOdemeTip", "SiparisDurumTip", "OdemeDurumTip", "Kullanici", "Kullanici.KullaniciDetay", "SiparisDetay", "SiparisDetay.Urun", "SiparisDetay.Urun.Vergi" });

                if (siparis.KullaniciId != girisYapanKullanici.KullaniciId)
                {
                    ViewBag.Mesaj = "Sipariş kullanıcısı ile giriş yapılan kullanıcı uyuşmamaktadır.";
                    return PartialView("~/Views/Kullanici/Partials/SiparisDetayPartial.cshtml");
                }

                ViewBag.Siparis = siparis;
            }


            return PartialView("~/Views/Kullanici/Partials/SiparisDetayPartial.cshtml");
        }

        public ActionResult SiparisGonderimPartial(int siparisId)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                ViewBag.Siparis = new Siparis();
            }
            else
            {
                var siparis = SiparisService.GetSingle(x => x.SiparisId == siparisId, true, new string[] { "SiparisDetay.SiparisGonderim", "SiparisDetay.SiparisGonderim.Gonderim", "SiparisDetay.SiparisGonderim.TeslimZamani", "SiparisDetay.Urun" });

                if (siparis.KullaniciId != girisYapanKullanici.KullaniciId)
                {
                    ViewBag.Mesaj = "Sipariş kullanıcısı ile giriş yapılan kullanıcı uyuşmamaktadır.";
                    return PartialView("~/Views/Kullanici/Partials/SiparisGonderimPartial.cshtml");
                }

                ViewBag.Siparis = siparis;
            }

            return PartialView("~/Views/Kullanici/Partials/SiparisGonderimPartial.cshtml");
        }

        public ActionResult SiparisIadeTalepPartial(int siparisId)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                ViewBag.Siparis = new Siparis();
            }
            else
            {
                var siparis = SiparisService.GetSingle(x => x.SiparisId == siparisId, true, new string[] { "SiparisDetay", "SiparisDetay.Urun", "SiparisDetay.Urun.UrunResim", "SiparisDetay.IadeTalep", "SiparisDetay.IadeTalep.IadeTalepDurumTip", "SiparisDetay.IadeTalep.IadeTalepIstekTip", "SiparisDetay.IadeTalep.IadeTalepNedenTip" });

                if (siparis.KullaniciId != girisYapanKullanici.KullaniciId)
                {
                    ViewBag.Mesaj = "Sipariş kullanıcısı ile giriş yapılan kullanıcı uyuşmamaktadır.";
                    return PartialView("~/Views/Kullanici/Partials/SiparisIadeTalepPartial.cshtml");
                }

                ViewBag.Siparis = siparis;

                ViewBag.IadeTalepNedenTipListesi = IadeTalepNedenTipService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.IadeTalepNedenTipId, text = x.Adi }).ToList();
                ViewBag.IadeTalepIstekTipListesi = IadeTalepIstekTipService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.IadeTalepIstekTipId, text = x.Adi }).ToList();

                var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);
                ViewBag.IadeKosullari = icerikAyar.IadeKosullari;
            }

            return PartialView("~/Views/Kullanici/Partials/SiparisIadeTalepPartial.cshtml");
        }

        #endregion

        #region Ajax Methods
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult KullaniciOlustur(string ad, string soyad, int? sirketId, string sifre, string eposta, string captchaId, string instanceId, string
     userInput)
        {
            var flag = false;

            flag = Captcha.AjaxValidate(captchaId, userInput, instanceId);

            if (!flag)
                return Json(new
                {
                    messageType = "warning",
                    message = "Girilen captcha hatalı."
                }, JsonRequestBehavior.DenyGet);

            var kul = KullaniciService.FindBy(x => x.AktifMi == true && x.Eposta == eposta).SingleOrDefault();
            if (kul != null)
                return Json(new
                {
                    messageType = "warning",
                    message = "Bu eposta adresi ile kullanıcı kaydı zaten mevcut."
                }, JsonRequestBehavior.DenyGet);

            flag = KullaniciService.CreateUser(ad, soyad, sifre, eposta, sirketId);
            if (flag)
            {
                KullaniciGiris(eposta, sifre, true, "Kullanici");

                return Json(new
                {
                    messageType = "success",
                    message = "Kullanıcı kaydı başarıyla tamamlanmıştır."
                }, JsonRequestBehavior.DenyGet);

            }
            else
            {
                return Json(new
                {
                    messageType = "error",
                    message = "İşlem sırasında bir hata oluştu. Lütfen tekrar deneyin."
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public ActionResult KullaniciGiris(string eposta, string sifre, bool beniHatirla, string currentController)
        {
            var kullaniciGirisSonuc = KullaniciService.LoginUser(eposta, sifre, beniHatirla);

            if (kullaniciGirisSonuc == true)
            {
                var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

                // Cookide ürün var ise sepete ekle ve cookiden sil
                // Aşama 1 - Cookideki ürünleri al
                var cookieUrunList = SepetService.GetUserProductsFromBasket(Guid.Empty);
                if (cookieUrunList.Any())
                {
                    // Aşama 2 - Ürünleri kullanıcının sepetine ekle
                    foreach (var cookieUrun in cookieUrunList)
                    {
                        SepetService.SetUserProductsFromBasket(girisYapanKullanici.KullaniciId, cookieUrun.UrunId, cookieUrun.Adet);
                    }
                }
                // Aşama 3 - Cookiedeki ürünleri sil
                SepetService.ClearUserProductsFromBasket(Guid.Empty);

                if (currentController == "Kullanici")
                {
                    return Json(new
                    {
                        Url = Url.Action("", ""),
                        Controller = currentController,
                        Sonuc=true,
                    }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    return Json(new
                    {
                        Url = Url.Action("Odeme", "Sepet"),
                        Controller = currentController,
                        Sonuc = true,
                    }, JsonRequestBehavior.DenyGet);
                }
            }
            else
                return Json(new
                {
                    Url = Url.Action("", ""),
                    Controller = "",
                    Sonuc = false,
                }, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public ActionResult KullaniciCikis()
        {
            KullaniciService.LogoutUser();

            return RedirectToAction("", "");
        }

        [HttpGet]
        public ActionResult KullaniciDetayGuncelle(string ad, string soyad)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
            else
            {
                var kullaniciDetayGuncelleSonuc = KullaniciDetayService.UpdateUserDetail(girisYapanKullanici, ad, soyad);

                if (kullaniciDetayGuncelleSonuc == true)
                    return View();
                else
                    return View();
            }
        }

        [HttpGet]
        public ActionResult KullaniciSifreDegistir(string eskiSifre, string yeniSifre)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
            else
            {
                var kullaniciSifreDegistirSonuc = KullaniciService.UpdateUserPassword(eskiSifre, yeniSifre);

                if (kullaniciSifreDegistirSonuc == true)
                    return View();
                else
                    return View();
            }
        }

        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult SifreGonder(string epostaAdresi, string captchaId, string instanceId, string
     userInput)
        {
            var flag = Captcha.AjaxValidate(captchaId, userInput, instanceId);

            if (flag)
            {
                try
                {
                    var yeniSifre = KullaniciService.GenerateUserPassword(epostaAdresi);
                    if (string.IsNullOrEmpty(yeniSifre))
                    {
                        flag = false;
                    }
                    else
                    {
                        string konu = "Şifre Güncelleme";
                        string icerik = "Yeni Şifreniz : " + yeniSifre + "<br/>" + "Lütfen Şifrenizi Kimse İle Paylaşmayınız.";

                        flag = EmailHelper.SendMail(konu, icerik, epostaAdresi);
                    }

                    return Json(flag, JsonRequestBehavior.DenyGet);
                }
                catch (Exception)
                {
                    return Json(false, JsonRequestBehavior.DenyGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult KullaniciAdresGetir()
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
            else
            {
                var sonucListesi = KullaniciAdresService.GetUserAdresses(girisYapanKullanici.KullaniciId);
                var data = sonucListesi.Select(x => new
                {
                    AdresAdi = x.Adres.AdresAdi,
                    FaturaTipi = x.Adres.FaturaTip.Adi,
                    AdSoyad = x.Adres.Ad + " " + x.Adres.Soyad,
                    Firma = x.Adres.FirmaUnvan,
                    AdresBilgi = x.Adres.AdresBilgi,
                    PostaKodu = x.Adres.PostaKodu,
                    IlceAdi = x.Adres.AdresIlce == null ? "" : x.Adres.AdresIlce.IlceAdi,
                    IlAdi = x.Adres.AdresIl == null ? "" : x.Adres.AdresIl.IlAdi,
                    Telefon1 = x.Adres.Telefon1,
                    Telefon2 = x.Adres.Telefon2,
                    AdresId = x.AdresId
                }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult AdresAra(int sayfaSayisi, int sayfaSirasi)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
            else
            {
                var sonucListesi = KullaniciAdresService.GetUserAdresses(girisYapanKullanici.KullaniciId);

                var count = sonucListesi.Count;

                sonucListesi = sonucListesi.Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

                return Json(new
                {
                    sonucListesi = sonucListesi.Select(x => new
                    {
                        AdresAdi = x.Adres.AdresAdi,
                        FaturaTipi = x.Adres.FaturaTip.Adi,
                        AdSoyad = x.Adres.Ad + " " + x.Adres.Soyad,
                        Firma = x.Adres.FirmaUnvan,
                        AdresBilgi = x.Adres.AdresBilgi,
                        PostaKodu = x.Adres.PostaKodu,
                        IlceAdi = x.Adres.AdresIlce == null ? "" : x.Adres.AdresIlce.IlceAdi,
                        IlAdi = x.Adres.AdresIl == null ? "" : x.Adres.AdresIl.IlAdi,
                        Telefon1 = x.Adres.Telefon1,
                        Telefon2 = x.Adres.Telefon2,
                        AdresId = x.AdresId
                    }).ToList(),
                    sayfaSayisi = sayfaSayisi,
                    sayfaSirasi = sayfaSirasi,
                    toplamKayitSayisi = count
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AdresKaydet(Adres adres)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                return Json(new
                {
                    messageType = "warning",
                    message = "Kullanıcı Girişi Yapılması Gerekmektedir."
                }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                KullaniciAdres _kullaniciAdres;
                int id = adres.AdresId;

                if (id == 0)
                {
                    _kullaniciAdres = new KullaniciAdres
                    {
                        KullaniciId = girisYapanKullanici.KullaniciId,
                        OlusturmaTarihi = DateTime.Now,
                        GuncellemeTarihi = DateTime.Now,
                        Sira = 0,
                        AktifMi = true
                    };

                    Adres _adres = new Adres();
                    _adres.Tarih = DateTime.Now;
                    _adres.AktifMi = true;

                    _adres.AdresAdi = adres.AdresAdi;
                    _adres.FaturaTipId = adres.FaturaTipId;
                    _adres.Ad = adres.Ad;
                    _adres.Soyad = adres.Soyad;
                    _adres.TcNo = adres.TcNo;
                    _adres.FirmaUnvan = adres.FirmaUnvan;
                    _adres.VergiDairesi = adres.VergiDairesi;
                    _adres.VergiNo = adres.VergiNo;
                    _adres.AdresBilgi = adres.AdresBilgi;
                    _adres.Aciklama = adres.Aciklama;
                    _adres.PostaKodu = adres.PostaKodu;
                    _adres.AdresIlceId = adres.AdresIlceId;
                    _adres.AdresIlId = adres.AdresIlId;
                    _adres.Telefon1 = adres.Telefon1;
                    _adres.Telefon2 = adres.Telefon2;

                    _kullaniciAdres.Adres = _adres;

                    KullaniciAdresService.Add(_kullaniciAdres);
                }
                else
                {
                    _kullaniciAdres = KullaniciAdresService.GetSingle(x => x.AdresId == id &&
                                                                           x.KullaniciId == girisYapanKullanici.KullaniciId, true, new string[] { });

                    if (_kullaniciAdres == null)
                        return Json(new
                        {
                            messageType = "error",
                            message = "Kullanıcı adresi bulunamadı."
                        }, JsonRequestBehavior.DenyGet);

                    Adres _adres = AdresService.GetSingle(x => x.AdresId == id, true, new string[] { });

                    _adres.AdresAdi = adres.AdresAdi;
                    _adres.FaturaTipId = adres.FaturaTipId;
                    _adres.Ad = adres.Ad;
                    _adres.Soyad = adres.Soyad;
                    _adres.TcNo = adres.TcNo;
                    _adres.FirmaUnvan = adres.FirmaUnvan;
                    _adres.VergiDairesi = adres.VergiDairesi;
                    _adres.VergiNo = adres.VergiNo;
                    _adres.AdresBilgi = adres.AdresBilgi;
                    _adres.Aciklama = adres.Aciklama;
                    _adres.PostaKodu = adres.PostaKodu;
                    _adres.AdresIlceId = adres.AdresIlceId;
                    _adres.AdresIlId = adres.AdresIlId;
                    _adres.Telefon1 = adres.Telefon1;
                    _adres.Telefon2 = adres.Telefon2;

                    AdresService.Edit(_adres);

                    _kullaniciAdres.GuncellemeTarihi = DateTime.Now;
                    KullaniciAdresService.Edit(_kullaniciAdres);
                }

                try
                {
                    if (KullaniciAdresService.Save() && AdresService.Save())
                        if (id == 0)
                            return Json(new
                            {
                                messageType = "success",
                                message = "Adres Kayıt Edilmiştir."
                            }, JsonRequestBehavior.DenyGet);
                        else
                            return Json(new
                            {
                                messageType = "success",
                                message = "Adres Güncellenmiştir."
                            }, JsonRequestBehavior.DenyGet);
                    else
                        return Json(new
                        {
                            messageType = "error",
                            message = "Beklenmedik bir sorun oluştu. Lütfen tekrar deneyin."
                        }, JsonRequestBehavior.DenyGet);

                }
                catch (Exception)
                {
                    return Json(new
                    {
                        messageType = "error",
                        message = "Beklenmedik bir sorun oluştu. Lütfen tekrar deneyin."
                    }, JsonRequestBehavior.DenyGet);
                }
            }
        }

        [HttpPost]
        public JsonResult AdresSil(int id)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
            else
            {
                try
                {
                    if (id == 0)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    var _kullaniciAdres = KullaniciAdresService.GetSingle(x => x.AdresId == id &&
                                                                      x.KullaniciId == girisYapanKullanici.KullaniciId, true, new string[] { "Adres" });

                    if (_kullaniciAdres == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _kullaniciAdres.AktifMi = false;
                    _kullaniciAdres.Adres.AktifMi = false;

                    KullaniciAdresService.Edit(_kullaniciAdres);

                    var flag = KullaniciAdresService.Save();

                    if (!flag)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    return Json(flag, JsonRequestBehavior.DenyGet);
                }
                catch (Exception)
                {
                    return Json(false, JsonRequestBehavior.DenyGet);
                }
            }
        }

        [HttpGet]
        public JsonResult IstekListesiAra(int sayfaSayisi, int sayfaSirasi)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
            else
            {
                var sonucListesi = IstekListesiService.GetUserWishList(girisYapanKullanici.KullaniciId);

                var count = sonucListesi.Count;

                sonucListesi = sonucListesi.Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

                return Json(new
                {
                    sonucListesi = sonucListesi.Select(x => new
                    {
                        IstekListesiId = x.IstekListesiId,
                        UrunNo = x.UrunId,
                        Tarih = x.Tarih.ToString("dd.MM.yyyy HH:mm"),
                        StoktaVarMi = x.Urun.StoktaVarMi,
                    }).ToList(),
                    sayfaSayisi = sayfaSayisi,
                    sayfaSirasi = sayfaSirasi,
                    toplamKayitSayisi = count
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult IadeListesiAra(int sayfaSayisi, int sayfaSirasi)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
            else
            {
                var sonucListesi = IadeTalepService.GetUserReturnList(girisYapanKullanici.KullaniciId);

                var count = sonucListesi.Count;

                sonucListesi = sonucListesi.Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

                return Json(new
                {
                    sonucListesi = sonucListesi.Select(x => new
                    {
                        IadeTalepId = x.IadeTalepId,
                        Adet = x.Adet,
                        UrunNo = x.SiparisDetay.Urun.UrunId,
                        Tarih = x.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm"),
                        IadeTalepDurumTipId = x.IadeTalepDurumTipId,
                        TalepDurumu = x.IadeTalepDurumTip.Adi
                    }).ToList(),
                    sayfaSayisi = sayfaSayisi,
                    sayfaSirasi = sayfaSirasi,
                    toplamKayitSayisi = count
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult IstekListesineEkle(int urunId)
        {
            //var flag = "";

            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (urunId == 0)
            {
                return Json(new
                {
                    messageType = "warning",
                    message = "Ürün Bulunamadı."
                }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                if (girisYapanKullanici == null)
                {
                    return Json(new
                    {
                        messageType = "warning",
                        message = "Kullanıcı Girişi Yapılması Gerekmektedir."
                    }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    //aynı ürün favori listesine daha önce kayıt edilmiş mi? Edildiyse kayıt edilmeyecek. 
                    if (IstekListesiService.FindBy(x => x.AktifMi &&
                                                        x.UrunId == urunId &&
                                                        x.KullaniciId == girisYapanKullanici.KullaniciId, false).Any())
                    {
                        return Json(new
                        {
                            messageType = "warning",
                            message = "Ürün Daha Önce İstek Listenize Eklenmiştir."
                        }, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        try
                        {
                            IstekListesi _istekListesi = new IstekListesi();
                            _istekListesi.KullaniciId = girisYapanKullanici.KullaniciId;
                            _istekListesi.Tarih = DateTime.Now;
                            _istekListesi.AktifMi = true;
                            _istekListesi.UrunId = urunId;

                            IstekListesiService.Add(_istekListesi);

                            if (IstekListesiService.Save())
                                return Json(new
                                {
                                    messageType = "success",
                                    message = "Ürün İstek Listenize Eklenmiştir."
                                }, JsonRequestBehavior.DenyGet);
                            else
                                return Json(new
                                {
                                    messageType = "error",
                                    message = "Beklenmedik bir sorun oluştu. Lütfen tekrar deneyin."
                                }, JsonRequestBehavior.DenyGet);
                        }
                        catch (Exception)
                        {
                            return Json(new
                            {
                                messageType = "error",
                                message = "Beklenmedik bir sorun oluştu. Lütfen tekrar deneyin."
                            }, JsonRequestBehavior.DenyGet);
                        }
                    }
                }
            }
        }

        [HttpPost]
        public JsonResult IadeTalepEkle(IadeTalep iadeTalep)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (iadeTalep == null)
            {
                return Json(new
                {
                    messageType = "warning",
                    message = "İade Talebiniz Belirtilmemiş."
                }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                if (girisYapanKullanici == null)
                {
                    return Json(new
                    {
                        messageType = "warning",
                        message = "Kullanıcı Girişi Yapılması Gerekmektedir."
                    }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    // Sipariş ilgili kullanıcının mı?
                    var siparisDetay = SiparisDetayService.FindBy(x => x.AktifMi && x.SiparisDetayId == iadeTalep.SiparisDetayId, true, new string[] { "Siparis" }).SingleOrDefault();

                    if (siparisDetay == null)
                    {
                        return Json(new
                        {
                            messageType = "warning",
                            message = "Sipariş Detayınıza Ulaşılamadı."
                        }, JsonRequestBehavior.DenyGet);
                    }

                    if (siparisDetay.Siparis.KullaniciId != girisYapanKullanici.KullaniciId)
                    {
                        return Json(new
                        {
                            messageType = "error",
                            message = "Sipariş Bilgisi ile Kullanıcı Bilgileri Uyuşmamaktadır."
                        }, JsonRequestBehavior.DenyGet);
                    }

                    // Sipariş detayla ilgili daha önce iade talebi girilmiş mi? varsa kaç tane?
                    var iadeTalepOncekiler = IadeTalepService.FindBy(x => x.AktifMi && x.SiparisDetayId == iadeTalep.SiparisDetayId).ToList();

                    var oncekiMiktar = iadeTalepOncekiler.Count > 0 ? iadeTalepOncekiler.Sum(x => x.Adet) : 0;

                    if ((oncekiMiktar + iadeTalep.Adet) > siparisDetay.Adet)
                    {
                        return Json(new
                        {
                            messageType = "warning",
                            message = "Belirttiğiniz toplam iade miktarı sipariş ettiğiniz miktardan fazladır. </br></br> <b>Sipariş edilen miktar:</b> " + siparisDetay.Adet + "</br><b>İstenilen toplam iade miktarı:</b>" + (oncekiMiktar + iadeTalep.Adet)
                        }, JsonRequestBehavior.DenyGet);
                    }

                    try
                    {
                        var _iadeTalep = new IadeTalep();
                        _iadeTalep.Adet = iadeTalep.Adet;
                        _iadeTalep.AktifMi = true;
                        _iadeTalep.GuncellemeTarihi = DateTime.Now;
                        _iadeTalep.IadeIstek = string.Empty;
                        _iadeTalep.IadeNeden = string.Empty;
                        _iadeTalep.IadeTalepDurumTipId = 1;
                        _iadeTalep.IadeTalepIstekTipId = iadeTalep.IadeTalepIstekTipId;
                        _iadeTalep.IadeTalepNedenTipId = iadeTalep.IadeTalepNedenTipId;
                        _iadeTalep.KullaniciAciklama = iadeTalep.KullaniciAciklama;
                        _iadeTalep.OlusturmaTarihi = DateTime.Now;
                        _iadeTalep.SiparisDetayId = iadeTalep.SiparisDetayId;

                        IadeTalepService.Add(_iadeTalep);

                        var flag = IadeTalepService.Save();

                        if (flag)
                            return Json(new
                            {
                                messageType = "success",
                                message = "İade Talebiniz Başarıyla Alınmıştır."
                            }, JsonRequestBehavior.DenyGet);

                        return Json(new
                        {
                            messageType = "error",
                            message = "Beklenmedik bir sorun oluştu. Lütfen tekrar deneyin."
                        }, JsonRequestBehavior.DenyGet);
                    }
                    catch (Exception)
                    {
                        return Json(new
                        {
                            messageType = "error",
                            message = "Beklenmedik bir sorun oluştu. Lütfen tekrar deneyin."
                        }, JsonRequestBehavior.DenyGet);
                    }
                }
            }
        }

        [HttpPost]
        public JsonResult IstekListesiSil(int id)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
            else
            {
                try
                {
                    if (id == 0)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    var _istekListesi = IstekListesiService.GetSingle(x => x.IstekListesiId == id &&
                                                             x.KullaniciId == girisYapanKullanici.KullaniciId, true);

                    if (_istekListesi == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _istekListesi.AktifMi = false;

                    IstekListesiService.Edit(_istekListesi);

                    var flag = IstekListesiService.Save();

                    if (!flag)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    return Json(flag, JsonRequestBehavior.DenyGet);
                }
                catch (Exception)
                {
                    return Json(false, JsonRequestBehavior.DenyGet);
                }
            }
        }

        [HttpPost]
        public JsonResult IadeTalepSil(int id)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
            else
            {
                try
                {
                    if (id == 0)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    var _iadeTalep = IadeTalepService.GetSingle(x => x.IadeTalepId == id &&
                                                             x.SiparisDetay.Siparis.KullaniciId == girisYapanKullanici.KullaniciId, true, new string[] { "SiparisDetay", "SiparisDetay.Siparis" });

                    if (_iadeTalep == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    if (_iadeTalep.IadeTalepDurumTipId != 1)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    if (_iadeTalep.SiparisDetay.Siparis.KullaniciId != girisYapanKullanici.KullaniciId)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _iadeTalep.AktifMi = false;

                    IadeTalepService.Edit(_iadeTalep);

                    var flag = IadeTalepService.Save();

                    if (!flag)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    return Json(flag, JsonRequestBehavior.DenyGet);
                }
                catch (Exception)
                {
                    return Json(false, JsonRequestBehavior.DenyGet);
                }
            }
        }

        [HttpGet]
        public JsonResult YorumAra(int sayfaSayisi, int sayfaSirasi)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
            else
            {
                var sonucListesi = UrunYorumService.GetUserComments(girisYapanKullanici.KullaniciId);

                var count = sonucListesi.Count;

                sonucListesi = sonucListesi.Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

                return Json(new
                {
                    sonucListesi = sonucListesi.Select(x => new
                    {
                        UrunNo = x.UrunId,
                        UrunYorumId = x.UrunYorumId,
                        Tarih = x.Tarih.ToString("dd.MM.yyyy HH:mm"),
                        Yorum = x.Yorum,
                        GosterilsinMi = x.GosterilsinMi,
                    }).ToList(),
                    sayfaSayisi = sayfaSayisi,
                    sayfaSirasi = sayfaSirasi,
                    toplamKayitSayisi = count
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult YorumSil(int id)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
            else
            {
                try
                {
                    if (id == 0)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    var _urunYorum = UrunYorumService.GetSingle(x => x.UrunYorumId == id &&
                                                             x.KullaniciId == girisYapanKullanici.KullaniciId, true);

                    if (_urunYorum == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _urunYorum.AktifMi = false;

                    UrunYorumService.Edit(_urunYorum);

                    var flag = UrunYorumService.Save();

                    if (!flag)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    return Json(flag, JsonRequestBehavior.DenyGet);
                }
                catch (Exception)
                {
                    return Json(false, JsonRequestBehavior.DenyGet);
                }
            }
        }

        [HttpGet]
        public JsonResult SiparisAra(int sayfaSayisi, int sayfaSirasi)
        {
            var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();

            if (girisYapanKullanici == null)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
            else
            {
                var sonucListesi = SiparisService.GetUserOrders(girisYapanKullanici.KullaniciId);

                var count = sonucListesi.Count;

                sonucListesi = sonucListesi.Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

                return Json(new
                {
                    sonucListesi = sonucListesi.Select(x => new
                    {
                        SiparisId = x.SiparisId,
                        SiparisTarihi = x.SiparisTarihi.ToString("dd.MM.yyyy HH:mm"),
                        SonIslemTarihi = x.SonIslemTarihi.ToString("dd.MM.yyyy HH:mm"),
                        GonderilenKisi = x.Adres.Ad + " " + x.Adres.Soyad,
                        SiparisToplam = string.Format("{0:N}", x.OdenecekTutar),
                        SiparisDurumTipId = x.SiparisDurumTipId,
                        SiparisDurumTipAdi = x.SiparisDurumTip.Adi,
                    }).ToList(),
                    sayfaSayisi = sayfaSayisi,
                    sayfaSirasi = sayfaSirasi,
                    toplamKayitSayisi = count
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult BagliIlceGetir(int id)
        {
            try
            {
                if (id == 0)
                    return Json(null, JsonRequestBehavior.AllowGet);

                var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

                if (icerikAyar.CacheAktifMi)
                {
                    var adresIlceler = AdresIlceService.GetAllActiveTownsByCityIdFromCache(id).Select(x => new { id = x.AdresIlceId, text = x.IlceAdi }).ToList();
                    return Json(adresIlceler, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var adresIlceler = AdresIlceService.GetAllActiveTownsByCityId(id).Select(x => new { id = x.AdresIlceId, text = x.IlceAdi }).ToList();
                    return Json(adresIlceler, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}