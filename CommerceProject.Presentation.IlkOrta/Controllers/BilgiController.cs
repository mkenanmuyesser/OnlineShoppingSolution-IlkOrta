using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.Custom;
using CommerceProject.Business.Entities;
using CommerceProject.Business.Helper.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CommerceProject.Presentation.IlkOrta.Helper.PageHelper;

namespace CommerceProject.Presentation.IlkOrta.Controllers
{
    public class BilgiController : BaseController
    {
        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        ISepetService SepetService;
        IKategoriService KategoriService;
        IHesapNumarasiService HesapNumarasiService;
        ISSSService SSSService;
        IMarkaService MarkaService;
        public BilgiController(IIcerikAyarService iIcerikAyarService,
                               IKullaniciService iKullaniciService,
                               ISepetService iSepetService,
                               IKategoriService iKategoriService,
                               IHesapNumarasiService iHesapNumarasiService,
                               ISSSService iSSSService,
                               IMarkaService iMarkaService) : base(iIcerikAyarService,
                                                                   iKullaniciService,
                                                                   iSepetService,
                                                                   iKategoriService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;
            SepetService = iSepetService;
            KategoriService = iKategoriService;

            HesapNumarasiService = iHesapNumarasiService;
            SSSService = iSSSService;
            MarkaService = iMarkaService;
        }

        #region Actions

        public ActionResult Hakkimizda()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Bilgi", "Hakkımızda", "", "", "/Bilgi/Hakkimizda");

            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            if (icerikAyar.CacheAktifMi)
                ViewBag.Markalar = MarkaService.GetAllActiveBrandsFromCache();
            else
                ViewBag.Markalar = MarkaService.GetAllActiveBrands();

            return View();
        }

        public ActionResult Iletisim()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Bilgi", "İletişim", "", "", "/Bilgi/Iletisim");

            return View();
        }

        public ActionResult MesafeliSatisSozlesmesi()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Bilgi", "Mesafeli Satış Sözleşmesi", "", "", "/Bilgi/MesafeliSatisSozlesmesi");

            return View();
        }

        public ActionResult GuvenlikIlkeleri()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Bilgi", "Güvenlik İlkeleri", "", "", "/Bilgi/GuvenlikIlkeleri");

            return View();
        }

        public ActionResult TuketiciHaklari()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Bilgi", "Tüketici Hakları", "", "", "/Bilgi/TuketiciHaklari");


            return View();
        }

        public ActionResult HesapNumaralari()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Bilgi", "Hesap Numaraları", "", "", "/Bilgi/HesapNumaralari");

            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            if (icerikAyar.CacheAktifMi)
                ViewBag.BankaHesapNumaralari = HesapNumarasiService.GetAllActiveBankAccountNumbersFromCache();
            else
                ViewBag.BankaHesapNumaralari = HesapNumarasiService.GetAllActiveBankAccountNumbers();

            return View();
        }

        public ActionResult IadeKosullari()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Bilgi", "İade Koşulları", "", "", "/Bilgi/IadeKosullari");

            return View();
        }

        public ActionResult SSS()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Bilgi", "Sık Sorulan Sorular", "", "", "/Bilgi/SSS");

            var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);

            if (icerikAyar.CacheAktifMi)
                ViewBag.SSS = SSSService.GetAllActiveSSSFromCache();
            else
                ViewBag.SSS = SSSService.GetAllActiveSSS();

            return View();
        }

        public ActionResult SiteHaritasi()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Bilgi", "Site Haritası", "", "", "/Bilgi/SiteHaritasi");

            return View();
        }

        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult IletisimEpostaGonder(string iletisimAdSoyad, string iletisimEposta, string iletisimTelefon, string iletisimMesaj)
        {
            try
            {
                string konu = "İletişim E-posta";
                string icerik = "Ad Soyad : " + iletisimAdSoyad + "<br/>" + "E-posta : " + iletisimEposta + "<br/>" + "Telefon : " + iletisimTelefon + "<br/>" + "Mesaj : " + iletisimMesaj;
                string eposta = (ViewBag.IcerikAyar as IcerikAyar).GonderilecekEposta;

                var flag = EmailHelper.SendMail(konu, icerik, eposta);

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }
        #endregion
    }
}