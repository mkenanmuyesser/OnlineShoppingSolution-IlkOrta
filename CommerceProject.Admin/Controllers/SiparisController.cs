using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.BusinessContracts;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Business.Entities;
using System.Data;
using CommerceProject.Admin.Helper;
using CommerceProject.Business.Custom;
using static CommerceProject.Admin.Helper.PageHelper;

namespace CommerceProject.Admin.Controllers
{
    public class SiparisController : BaseController
    {
        #region Excel Action
        public class ExcelActionResult : ActionResult
        {
            private readonly DataTable _content;

            public ExcelActionResult(DataTable content)
            {
                _content = content;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.ClearContent();
                context.HttpContext.Response.ClearHeaders();
                context.HttpContext.Response.Buffer = true;
                context.HttpContext.Response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">");
                context.HttpContext.Response.Write(@"<meta http-equiv=""content-type"" content=""application/vnd.ms-excel; charset=UTF-8"">");
                context.HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=Rapor.xls");

                //sets font
                context.HttpContext.Response.Write("<font style='font-size:10.0pt; font-family:Calibri;'>");
                context.HttpContext.Response.Write("<BR><BR><BR>");
                //sets the table border, cell spacing, border color, font of the text, background, foreground, font height
                context.HttpContext.Response.Write("<Table border='1' bgColor='#ffffff' " +
                  "borderColor='#000000' cellSpacing='0' cellPadding='0' " +
                  "style='font-size:11.0pt; font-family:Calibri; background:white;'> <TR>");

                //am getting my grid's column headers
                int columnscount = _content.Columns.Count;

                for (int j = 0; j < columnscount; j++)
                {   //write in new column
                    context.HttpContext.Response.Write("<Td bgColor='#cacaca'>");
                    //Get column headers  and make it as bold in excel columns
                    context.HttpContext.Response.Write("<B>");
                    context.HttpContext.Response.Write(_content.Columns[j].ColumnName.ToString());
                    context.HttpContext.Response.Write("</B>");
                    context.HttpContext.Response.Write("</Td>");
                }
                context.HttpContext.Response.Write("</TR>");
                foreach (DataRow row in _content.Rows)
                {
                    context.HttpContext.Response.Write("<TR>");

                    for (int i = 0; i < _content.Columns.Count; i++)
                    {
                        context.HttpContext.Response.Write("<Td>");
                        context.HttpContext.Response.Write(row[i].ToString());
                        context.HttpContext.Response.Write("</Td>");
                    }

                    context.HttpContext.Response.Write("</TR>");
                }
                context.HttpContext.Response.Write("</Table>");
                context.HttpContext.Response.Write("</font>");
                context.HttpContext.Response.Flush();
                context.HttpContext.Response.End();
            }
        }
        #endregion

        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        ISiparisService SiparisService;
        ISiparisDetayService SiparisDetayService;
        ISiparisHareketService SiparisHareketService;
        ISiparisGonderimService SiparisGonderimService;
        IUrunService UrunService;
        IMarkaService MarkaService;
        ISiparisDurumTipService SiparisDurumTipService;
        IOdemeDurumTipService OdemeDurumTipService;
        ISiparisOdemeTipService SiparisOdemeTipService;
        IAdresService AdresService;
        IAdresIlService AdresIlService;
        IAdresIlceService AdresIlceService;
        IGonderimService GonderimService;
        ITeslimZamaniService TeslimZamaniService;
        IFaturaTipService FaturaTipService;
        IKomisyonIskontoTipService KomisyonIskontoTipService;
        public SiparisController(IIcerikAyarService iIcerikAyarService,
                                 IKullaniciService iKullaniciService, 
                                 ISiparisService iSiparisService,
                                 ISiparisDetayService iSiparisDetayService,
                                 ISiparisHareketService iSiparisHareketService,
                                 ISiparisGonderimService iSiparisGonderimService,
                                 IUrunService iUrunService,
                                 IMarkaService iMarkaService,
                                 ISiparisDurumTipService iSiparisDurumTipService,
                                 IOdemeDurumTipService iOdemeDurumTipService,
                                 ISiparisOdemeTipService iSiparisOdemeTipService,
                                 IAdresService iAdresService,
                                 IAdresIlService iAdresIlService,
                                 IAdresIlceService iAdresIlceService,
                                 IGonderimService iGonderimService,
                                 ITeslimZamaniService iTeslimZamaniService,
                                 IFaturaTipService iFaturaTipService,
                                 IKomisyonIskontoTipService iKomisyonIskontoTipService) : base(iIcerikAyarService,
                                                                                               iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            SiparisService = iSiparisService;
            SiparisDetayService = iSiparisDetayService;
            SiparisHareketService = iSiparisHareketService;
            SiparisGonderimService = iSiparisGonderimService;
            UrunService = iUrunService;
            MarkaService = iMarkaService;
            SiparisDurumTipService = iSiparisDurumTipService;
            OdemeDurumTipService = iOdemeDurumTipService;
            SiparisOdemeTipService = iSiparisOdemeTipService;
            AdresService = iAdresService;
            AdresIlService = iAdresIlService;
            AdresIlceService = iAdresIlceService;
            GonderimService = iGonderimService;
            TeslimZamaniService = iTeslimZamaniService;
            FaturaTipService = iFaturaTipService;
            KomisyonIskontoTipService = iKomisyonIskontoTipService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = Pages.siparis_index;
            ViewBag.PageProperties = PageProperties.SetPageProperties("Sipariş", "Arama", "Sipariş Arama İşlemleri", "");
            ViewBag.SiparisDurumTipListesi = SiparisDurumTipService.GetAll(true).ToList().Select(x => new { id = x.SiparisDurumTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.OdemeDurumTipListesi = OdemeDurumTipService.GetAll(true).ToList().Select(x => new { id = x.OdemeDurumTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.SiparisOdemeTipListesi = SiparisOdemeTipService.GetAll(true).ToList().Select(x => new { id = x.SiparisOdemeTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.IlListesi = AdresIlService.GetAll(true).ToList().Select(x => new { id = x.AdresIlId, text = x.IlAdi, aktif = x.AktifMi }).ToList();

            return View();
        }       

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = Pages.siparis_save;

            if (id == null)
            {
                ViewBag.PageProperties = PageProperties.SetPageProperties("Sipariş", "Kayıt", "Sipariş Kayıt İşlemleri", "");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.PageProperties = PageProperties.SetPageProperties("Sipariş", "Güncelleme", "Sipariş Güncelleme İşlemleri", "");
                ViewBag.Siparis = SiparisService.GetSingle(x => x.SiparisId == id, true, new string[] { "Adres", "Adres.AdresIl", "Adres.AdresIlce", "Adres1", "Adres1.AdresIl", "Adres1.AdresIlce", "SiparisOdemeTip", "SiparisDurumTip", "OdemeDurumTip", "Kullanici", "Kullanici.KullaniciDetay", "SiparisDetay", "SiparisDetay.Urun", "SiparisDetay.SiparisGonderim", "SiparisDetay.SiparisGonderim.Gonderim", "SiparisDetay.SiparisGonderim.TeslimZamani", "SiparisHareket" });
                ViewBag.SiparisDurumTipListesi = SiparisDurumTipService.GetAll(true).ToList().Select(x => new { id = x.SiparisDurumTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
                ViewBag.OdemeDurumTipListesi = OdemeDurumTipService.GetAll(true).ToList().Select(x => new { id = x.OdemeDurumTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
                ViewBag.SiparisOdemeTipListesi = SiparisOdemeTipService.GetAll(true).ToList().Select(x => new { id = x.SiparisOdemeTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
                ViewBag.IlListesi = AdresIlService.GetAll(true).ToList().Select(x => new { id = x.AdresIlId, text = x.IlAdi, aktif = x.AktifMi }).ToList();
                ViewBag.IlceListesi = AdresIlceService.GetAll(true).ToList().Select(x => new { id = x.AdresIlceId, text = x.IlceAdi, ilId = x.AdresIlId, aktif = x.AktifMi }).ToList();
                ViewBag.GonderimTipiListesi = GonderimService.GetAll(true).ToList().Select(x => new { id = x.GonderimId, text = x.Adi, aktif = x.AktifMi }).ToList();
                ViewBag.TeslimZamaniTipiListesi = TeslimZamaniService.GetAll(true).ToList().Select(x => new { id = x.TeslimZamaniId, text = x.Adi, aktif = x.AktifMi }).ToList();
                ViewBag.FaturaTipListesi = FaturaTipService.GetAll(true).ToList().Select(x => new { id = x.FaturaTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
            }

            return View();
        }

        [AuthorizeManager]
        public ActionResult OdemeYontemi()
        {
            ViewBag.PageName = Pages.odemeyontemi_save;
            ViewBag.PageProperties = PageProperties.SetPageProperties("Sipariş Ödeme Yöntemleri", "Güncelleme", "Sipariş Ödeme Yöntemleri Güncelleme İşlemleri", "");

            ViewBag.SiparisOdemeTipListesi = SiparisOdemeTipService.GetAll(true, new string[] { "KomisyonIskontoTip" }).ToList();

            return View();
        }

        public ActionResult SiparisDetayPartial(int id)
        {
            ViewBag.Siparis = SiparisService.GetSingle(x => x.SiparisId == id, true, new string[] { "Adres", "Adres.FaturaTip", "Adres.AdresIl", "Adres.AdresIlce", "Adres1", "Adres1.FaturaTip", "Adres1.AdresIl", "Adres1.AdresIlce", "SiparisOdemeTip", "SiparisDurumTip", "OdemeDurumTip", "Kullanici", "Kullanici.KullaniciDetay", "SiparisDetay", "SiparisDetay.Urun" });

            return PartialView("~/Views/Siparis/Partials/SiparisDetayPartial.cshtml");
        }

        public ActionResult SiparisUrunPartial(int id)
        {
            ViewBag.Siparis = SiparisService.GetSingle(x => x.SiparisId == id, true, new string[] { "SiparisDetay", "SiparisDetay.Urun" });

            return PartialView("~/Views/Siparis/Partials/SiparisUrunPartial.cshtml");
        }

        public ActionResult SiparisGonderimPartial(int id)
        {
            ViewBag.Siparis = SiparisService.GetSingle(x => x.SiparisId == id, true, new string[] { "SiparisDetay.SiparisGonderim", "SiparisDetay.SiparisGonderim.Gonderim", "SiparisDetay.SiparisGonderim.TeslimZamani" });

            return PartialView("~/Views/Siparis/Partials/SiparisGonderimPartial.cshtml");
        }

        public ActionResult SiparisHareketPartial(int id)
        {
            ViewBag.Siparis = SiparisService.GetSingle(x => x.SiparisId == id, true, new string[] { "SiparisHareket" });

            return PartialView("~/Views/Siparis/Partials/SiparisHareketPartial.cshtml");
        }

        public ActionResult SiparisOdemeTipGuncellePartial(int? id = null)
        {
            ViewBag.KomisyonIskontoTipListesi = KomisyonIskontoTipService.GetAll(true).ToList().Select(x => new { id = x.KomisyonIskontoTipId, text = x.Adi, aktif = x.AktifMi }).ToList();

            if (id == null)
            {
                ViewBag.SiparisOdemeTip = new SiparisOdemeTip();
            }
            else
            {
                ViewBag.SiparisOdemeTip = SiparisOdemeTipService.FindBy(x => x.SiparisOdemeTipId == id).SingleOrDefault();
            }

            return PartialView("~/Views/Siparis/Partials/SiparisOdemeTipGuncellePartial.cshtml");
        }

        public ActionResult SiparisProformaFaturaPartial(int id)
        {
            ViewBag.Siparis = SiparisService.GetSingle(x => x.SiparisId == id, true, new string[] { "Adres", "Adres.FaturaTip", "Adres.AdresIl", "Adres.AdresIlce", "Adres1", "Adres1.FaturaTip", "Adres1.AdresIl", "Adres1.AdresIlce", "SiparisOdemeTip", "SiparisDurumTip", "OdemeDurumTip", "Kullanici", "Kullanici.KullaniciDetay", "SiparisDetay", "SiparisDetay.Urun" });

            return PartialView("~/Views/Siparis/Partials/SiparisProformaFaturaPartial.cshtml");
        }

        public ActionResult ExcelRaporuAl(int siparisNo, int urunId, int markaId, int siparisDurumId, int odemeDurumId, int odemeTipId, int faturaAdresIlId, string faturaAdresAd, string faturaAdresSoyad, int gonderimAdresIlId, string gonderimAdresAd, string gonderimAdresSoyad, string aciklama, DateTime? baslangicTarihi, DateTime? bitisTarihi, int aktiflik)
        {
            var sonucListesi = SiparisService.FindBy(x =>
            (siparisNo == 0 || x.SiparisId == siparisNo) &&
            (siparisDurumId == 0 || x.SiparisDurumTipId == siparisDurumId) &&
            (odemeDurumId == 0 || x.OdemeDurumTipId == odemeDurumId) &&
            (odemeTipId == 0 || x.SiparisOdemeTipId == odemeTipId) &&
            (faturaAdresIlId == 0 || x.Adres.AdresIlId == faturaAdresIlId) &&
            (string.IsNullOrEmpty(faturaAdresAd) || x.Adres.Ad.Contains(faturaAdresAd)) &&
            (string.IsNullOrEmpty(faturaAdresSoyad) || x.Adres.Soyad.Contains(faturaAdresSoyad)) &&
            (gonderimAdresIlId == 0 || x.Adres1.AdresIlId == gonderimAdresIlId) &&
            (string.IsNullOrEmpty(gonderimAdresAd) || x.Adres1.Ad.Contains(gonderimAdresAd)) &&
            (string.IsNullOrEmpty(gonderimAdresSoyad) || x.Adres1.Soyad.Contains(gonderimAdresSoyad)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= x.SiparisTarihi && bitisTarihi >= x.SiparisTarihi)) ||
                ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= x.SiparisTarihi)) ||
                ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= x.SiparisTarihi))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "SiparisDetay", "SiparisDetay.Urun", "Adres", "Adres1", "SiparisOdemeTip", "SiparisDurumTip", "OdemeDurumTip", "Kullanici", "Kullanici.KullaniciDetay" }).ToList();


            var tempList = sonucListesi.ToList();

            // Ürüne göre filtreleme
            if (urunId > 0)
            {
                foreach (var sp in tempList)
                {
                    if (sp.SiparisDetay.Where(x => x.UrunId == urunId).Count() == 0)
                    {
                        sonucListesi.Remove(sp);
                    }
                }
            }

            // Markaya göre filtreleme
            if (markaId > 0)
            {
                foreach (var sp in tempList)
                {
                    if (sp.SiparisDetay.Where(x => x.Urun.MarkaId == markaId).Count() == 0)
                    {
                        sonucListesi.Remove(sp);
                    }
                }
            }

            var list = sonucListesi.Select(x => new
            {
                SiparisNo = "#" + x.SiparisId,
                SiparisDurumu = x.SiparisDurumTip.Adi,
                OdemeDurumu = x.OdemeDurumTip.Adi,
                OdemeTipi = x.SiparisOdemeTip.Adi,
                MusteriAdSoyad = x.Kullanici.KullaniciDetay.Ad + " " + x.Kullanici.KullaniciDetay.Soyad,
                MusteriEposta = x.Kullanici.Eposta,
                SiparisTarihi = x.SiparisTarihi.ToString("dd.MM.yyyy HH:mm"),
                SonIslemTarihi = x.SonIslemTarihi.ToString("dd.MM.yyyy HH:mm"),
                AktifMi = x.AktifMi
            }).ToList();

            var dt = new PageHelper().ToDataTable(list);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult Ara(int siparisNo, int urunId, int markaId, int siparisDurumId, int odemeDurumId, int odemeTipId, int faturaAdresIlId, string faturaAdresAd, string faturaAdresSoyad, int gonderimAdresIlId, string gonderimAdresAd, string gonderimAdresSoyad, string aciklama, DateTime? baslangicTarihi, DateTime? bitisTarihi, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var sonucListesi = SiparisService.FindBy(x =>
            (siparisNo == 0 || x.SiparisId == siparisNo) &&
            (siparisDurumId == 0 || x.SiparisDurumTipId == siparisDurumId) &&
            (odemeDurumId == 0 || x.OdemeDurumTipId == odemeDurumId) &&
            (odemeTipId == 0 || x.SiparisOdemeTipId == odemeTipId) &&
            (faturaAdresIlId == 0 || x.Adres.AdresIlId == faturaAdresIlId) &&
            (string.IsNullOrEmpty(faturaAdresAd) || x.Adres.Ad.Contains(faturaAdresAd)) &&
            (string.IsNullOrEmpty(faturaAdresSoyad) || x.Adres.Soyad.Contains(faturaAdresSoyad)) &&
            (gonderimAdresIlId == 0 || x.Adres1.AdresIlId == gonderimAdresIlId) &&
            (string.IsNullOrEmpty(gonderimAdresAd) || x.Adres1.Ad.Contains(gonderimAdresAd)) &&
            (string.IsNullOrEmpty(gonderimAdresSoyad) || x.Adres1.Soyad.Contains(gonderimAdresSoyad)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= x.SiparisTarihi && bitisTarihi >= x.SiparisTarihi)) ||
                ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= x.SiparisTarihi)) ||
                ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= x.SiparisTarihi))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "SiparisDetay", "SiparisDetay.Urun", "Adres", "Adres1", "SiparisOdemeTip", "SiparisDurumTip", "OdemeDurumTip", "Kullanici", "Kullanici.KullaniciDetay" }).ToList();


            var tempList = sonucListesi.ToList();            

            // Ürüne göre filtreleme
            if (urunId > 0)
            {
                foreach (var sp in tempList)
                {
                    if(sp.SiparisDetay.Where(x => x.UrunId == urunId).Count() == 0)
                    {
                        sonucListesi.Remove(sp);
                    }
                }
            }

            // Markaya göre filtreleme
            if(markaId > 0)
            {
                foreach (var sp in tempList)
                {
                    if(sp.SiparisDetay.Where(x => x.Urun.MarkaId == markaId).Count() == 0)
                    {
                        sonucListesi.Remove(sp);
                    }
                }
            }

            var count = sonucListesi.Count;

            var siparisToplam = sonucListesi.Sum(u => u.OdenecekTutar);
            var siparisIskonto = sonucListesi.Sum(u => u.ToplamIskonto);
            var siparisKomisyon = sonucListesi.Sum(u => u.ToplamKomisyon);
            var iadeToplam = sonucListesi.Sum(u => u.IadeToplam);

            sonucListesi = sonucListesi.Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            var siparisOzetToplam = sonucListesi.Sum(u => u.OdenecekTutar);
            var siparisOzetIskonto = sonucListesi.Sum(u => u.ToplamIskonto);
            var siparisOzetKomisyon = sonucListesi.Sum(u => u.ToplamKomisyon);
            var iadeOzetToplam = sonucListesi.Sum(u => u.IadeToplam);

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    SiparisId = x.SiparisId,
                    SiparisDurumu = x.SiparisDurumTip.Adi,
                    OdemeDurumu = x.OdemeDurumTip.Adi,
                    OdemeTipi = x.SiparisOdemeTip.Adi,
                    MusteriAdSoyad = x.Kullanici.KullaniciDetay.Ad + " " + x.Kullanici.KullaniciDetay.Soyad,
                    MusteriEposta = x.Kullanici.Eposta,
                    SiparisTarihi = x.SiparisTarihi.ToString("dd.MM.yyyy HH:mm"),
                    SonIslemTarihi = x.SonIslemTarihi.ToString("dd.MM.yyyy HH:mm"),
                    AktifMi = x.AktifMi
                }).ToList(),
                siparisOzetToplam = siparisOzetToplam,
                siparisOzetIskonto = siparisOzetIskonto,
                siparisOzetKomisyon = siparisOzetKomisyon,
                iadeOzetToplam = iadeOzetToplam,

                siparisToplam = siparisToplam,
                siparisIskonto = siparisIskonto,
                siparisKomisyon = siparisKomisyon,
                iadeToplam = iadeToplam,
                //siparisKarToplam = (siparisToplam - siparisIskonto - siparisVergi - iadeToplam),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SiparisKaydetGuncelle(Siparis siparis)
        {
            try
            {
                var _siparis = SiparisService.FindBy(x => x.SiparisId == siparis.SiparisId).SingleOrDefault();

                if(siparis == null)
                {
                    return Json(false, JsonRequestBehavior.DenyGet);
                }

                _siparis.SiparisOdemeTipId = siparis.SiparisOdemeTipId;
                _siparis.SiparisDurumTipId = siparis.SiparisDurumTipId;
                _siparis.OdemeDurumTipId = siparis.OdemeDurumTipId;
                _siparis.AktifMi = siparis.AktifMi;

                SiparisService.Edit(_siparis);

                var flag = SiparisService.Save();

                if (flag)
                    return Json(_siparis.SiparisId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult AdresKaydetGuncelle(Adres faturaAdres, Adres gonderimAdres)
        {
            try
            {
                if(faturaAdres == null || gonderimAdres == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _faturaAdres = AdresService.FindBy(x => x.AdresId == faturaAdres.AdresId).SingleOrDefault();

                if(_faturaAdres != null)
                {
                    _faturaAdres.Aciklama = faturaAdres.Aciklama;
                    _faturaAdres.AdresAdi = faturaAdres.AdresAdi;
                    _faturaAdres.FaturaTipId = faturaAdres.FaturaTipId;
                    _faturaAdres.Ad = faturaAdres.Ad;
                    _faturaAdres.AdresBilgi = faturaAdres.AdresBilgi;
                    _faturaAdres.AdresIlceId = faturaAdres.AdresIlceId;
                    _faturaAdres.AdresIlId = faturaAdres.AdresIlId;
                    _faturaAdres.AktifMi = faturaAdres.AktifMi;
                    _faturaAdres.PostaKodu = faturaAdres.PostaKodu;
                    _faturaAdres.FirmaUnvan = faturaAdres.FirmaUnvan;
                    _faturaAdres.Soyad = faturaAdres.Soyad;
                    _faturaAdres.TcNo = faturaAdres.TcNo;
                    _faturaAdres.Telefon1 = faturaAdres.Telefon1;
                    _faturaAdres.Telefon2 = faturaAdres.Telefon2;
                    _faturaAdres.VergiDairesi = faturaAdres.VergiDairesi;
                    _faturaAdres.VergiNo = faturaAdres.VergiNo;

                    AdresService.Edit(_faturaAdres);
                }

                var _gonderimAdres = AdresService.FindBy(x => x.AdresId == gonderimAdres.AdresId).SingleOrDefault();

                if(_gonderimAdres != null)
                {
                    _gonderimAdres.Aciklama = gonderimAdres.Aciklama;
                    _faturaAdres.AdresAdi = faturaAdres.AdresAdi;
                    _faturaAdres.FaturaTipId = faturaAdres.FaturaTipId;
                    _gonderimAdres.Ad = gonderimAdres.Ad;
                    _gonderimAdres.AdresBilgi = gonderimAdres.AdresBilgi;
                    _gonderimAdres.AdresIlceId = gonderimAdres.AdresIlceId;
                    _gonderimAdres.AdresIlId = gonderimAdres.AdresIlId;
                    _gonderimAdres.AktifMi = gonderimAdres.AktifMi;
                    _gonderimAdres.PostaKodu = gonderimAdres.PostaKodu;
                    _gonderimAdres.FirmaUnvan = gonderimAdres.FirmaUnvan;
                    _gonderimAdres.Soyad = gonderimAdres.Soyad;
                    _gonderimAdres.TcNo = gonderimAdres.TcNo;
                    _gonderimAdres.Telefon1 = gonderimAdres.Telefon1;
                    _gonderimAdres.Telefon2 = gonderimAdres.Telefon2;
                    _gonderimAdres.VergiDairesi = gonderimAdres.VergiDairesi;
                    _gonderimAdres.VergiNo = gonderimAdres.VergiNo;

                    AdresService.Edit(_gonderimAdres);
                }

                var flag = AdresService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult SiparisDetayAktiflikGuncelle(int siparisDetayId, bool aktifMi)
        {
            try
            {
                if (siparisDetayId == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _siparisDetay = SiparisDetayService.FindBy(x => x.SiparisDetayId == siparisDetayId).SingleOrDefault();

                if (_siparisDetay == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _siparisDetay.AktifMi = aktifMi;

                SiparisDetayService.Edit(_siparisDetay);

                var flag = SiparisDetayService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult SiparisHareketKaydetGuncelle(SiparisHareket siparisHareket)
        {
            try
            {
                if (siparisHareket == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                SiparisHareket _siparisHareket;
                if (siparisHareket.SiparisHareketId == 0)
                {
                    _siparisHareket = new SiparisHareket();
                    _siparisHareket.Aciklama = siparisHareket.Aciklama;
                    _siparisHareket.AktifMi = siparisHareket.AktifMi;
                    _siparisHareket.KullaniciyaGosterilecekMi = siparisHareket.KullaniciyaGosterilecekMi;
                    _siparisHareket.SiparisId = siparisHareket.SiparisId;
                    _siparisHareket.Tarih = siparisHareket.Tarih;

                    SiparisHareketService.Add(_siparisHareket);
                }
                else
                {
                    _siparisHareket = SiparisHareketService.FindBy(x => x.SiparisHareketId == siparisHareket.SiparisHareketId).SingleOrDefault();

                    if (_siparisHareket != null)
                    {
                        _siparisHareket.Aciklama = siparisHareket.Aciklama;
                        _siparisHareket.AktifMi = siparisHareket.AktifMi;
                        _siparisHareket.KullaniciyaGosterilecekMi = siparisHareket.KullaniciyaGosterilecekMi;
                        _siparisHareket.Tarih = siparisHareket.Tarih;

                        SiparisHareketService.Edit(_siparisHareket);
                    }
                }

                var flag = SiparisHareketService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult Sil(int id)
        {
            try
            {
                if (id == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _siparis = SiparisService.FindBy(x => x.SiparisId == id).SingleOrDefault();

                if (_siparis == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _siparis.AktifMi = false;

                SiparisService.Edit(_siparis);

                var flag = SiparisService.Save();

                // Sipariş detayları
                var siparisDetaylari = SiparisDetayService.FindBy(x => x.SiparisId == id).ToList();
                foreach (var _spDetay in siparisDetaylari)
                {
                    _spDetay.AktifMi = false;

                    SiparisDetayService.Edit(_spDetay);

                    SiparisDetayService.Save();
                }

                // Sipariş hareketleri
                var siparisHareketleri = SiparisHareketService.FindBy(x => x.SiparisId == id).ToList();
                foreach (var _spHareket in siparisHareketleri)
                {
                    _spHareket.AktifMi = false;

                    SiparisHareketService.Edit(_spHareket);

                    SiparisHareketService.Save();
                }

                // Sipariş gönderimleri
                var siparisGonderimleri = SiparisGonderimService.FindBy(x => x.SiparisDetay.SiparisId == id, false, new string[] { "SiparisDetay" }).ToList();
                foreach (var _spGonderim in siparisGonderimleri)
                {
                    _spGonderim.AktifMi = false;

                    SiparisGonderimService.Edit(_spGonderim);

                    SiparisGonderimService.Save();
                }

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult SiparisOdemeTipKaydetGuncelle(SiparisOdemeTip siparisOdemeTip)
        {
            try
            {
                SiparisOdemeTip _siparisOdemeTip;
                if(siparisOdemeTip.SiparisOdemeTipId == 0)
                {
                    _siparisOdemeTip = new SiparisOdemeTip();
                    _siparisOdemeTip.Adi = siparisOdemeTip.Adi;
                    _siparisOdemeTip.AktifMi = siparisOdemeTip.AktifMi;
                    _siparisOdemeTip.KomisyonIskontoTipId = siparisOdemeTip.KomisyonIskontoTipId;
                    _siparisOdemeTip.Miktar = siparisOdemeTip.Miktar;
                    _siparisOdemeTip.Sira = siparisOdemeTip.Sira;

                    SiparisOdemeTipService.Add(_siparisOdemeTip);
                }
                else
                {
                    _siparisOdemeTip = SiparisOdemeTipService.FindBy(x => x.SiparisOdemeTipId == siparisOdemeTip.SiparisOdemeTipId).SingleOrDefault();

                    if(_siparisOdemeTip == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _siparisOdemeTip.AktifMi = siparisOdemeTip.AktifMi;
                    _siparisOdemeTip.KomisyonIskontoTipId = siparisOdemeTip.KomisyonIskontoTipId;
                    _siparisOdemeTip.Miktar = siparisOdemeTip.Miktar;

                    SiparisOdemeTipService.Edit(_siparisOdemeTip);
                }

                var flag = SiparisOdemeTipService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }
        #endregion
    }
}