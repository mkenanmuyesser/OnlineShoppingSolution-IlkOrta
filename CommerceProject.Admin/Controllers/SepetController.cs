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

namespace CommerceProject.Admin.Controllers
{
    public class SepetController : BaseController
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
        ISepetService SepetService;
        ISepetTipService SepetTipService;
        IUrunService UrunService;
        public SepetController(IIcerikAyarService iIcerikAyarService,
                               IKullaniciService iKullaniciService, 
                               ISepetService iSepetService, 
                               ISepetTipService iSepetTipService, 
                               IUrunService iUrunService) : base(iIcerikAyarService,
                                                                 iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            SepetService = iSepetService;
            SepetTipService = iSepetTipService;
            UrunService = iUrunService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.sepet_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Sepet", "", "Sepet İşlemleri", "");
            ViewBag.SepetTipListesi = SepetTipService.GetAll(true).ToList().Select(x => new { id = x.SepetTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.KullaniciListesi = KullaniciService.GetAll(true).ToList().Select(x => new { id = x.KullaniciId, text = x.Eposta, aktif = x.AktifMi }).ToList();

            return View();
        }

        public ActionResult ExcelRaporuAl(Guid? kullaniciId, int sepetTipiId, int urunId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int aktiflik)
        {
            var sonucListesi = SepetService.GetAll(true, new string[] { "SepetTip", "Urun", "Kullanici", "Kullanici.KullaniciDetay" }).ToList().Where(x =>
            (kullaniciId == null || x.KullaniciId == kullaniciId) &&
            (sepetTipiId == 0 || x.SepetTipId == sepetTipiId) &&
            (urunId == 0 || x.UrunId == urunId) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (x.GuncellemeTarihi >= baslangicTarihi && x.GuncellemeTarihi <= bitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (x.GuncellemeTarihi >= baslangicTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (x.GuncellemeTarihi <= bitisTarihi)))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList().Select(x => new
            {
                KullaniciEposta = x.Kullanici != null ? x.Kullanici.Eposta : "",
                KullaniciAdiSoyadi = x.Kullanici != null ? (x.Kullanici.KullaniciDetay != null ? (x.Kullanici.KullaniciDetay.Ad + " " + x.Kullanici.KullaniciDetay.Soyad) : "") : "",
                SepetTipi = x.SepetTip != null ? x.SepetTip.Adi : "",
                UrunBarkodu = x.Urun != null ? x.Urun.Barkod : "",
                UrunKodu = x.Urun != null ? x.Urun.UrunKod : "",
                UrunAdi = x.Urun != null ? x.Urun.Adi : "",
                UrunAdedi = x.Adet,
                OlusturmaTarihi = x.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm"),
                GuncellemeTarihi = x.GuncellemeTarihi.ToString("dd.MM.yyyy HH:mm"),
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult Ara(Guid? kullaniciId, int sepetTipiId, int urunId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = SepetService.GetAll(true, new string[] { "SepetTip", "Urun", "Kullanici", "Kullanici.KullaniciDetay" }).ToList().Where(x =>
            (kullaniciId == null || x.KullaniciId == kullaniciId) &&
            (sepetTipiId == 0 || x.SepetTipId == sepetTipiId) &&
            (urunId == 0 || x.UrunId == urunId) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (x.GuncellemeTarihi >= baslangicTarihi && x.GuncellemeTarihi <= bitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (x.GuncellemeTarihi >= baslangicTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (x.GuncellemeTarihi <= bitisTarihi)))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.SepetId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    SepetId = x.SepetId,
                    UrunId = x.UrunId,
                    UrunAdedi = x.Adet,
                    UrunAdi = x.Urun != null ? x.Urun.Adi : "",
                    UrunKodu = x.Urun != null ? x.Urun.UrunKod : "",
                    UrunBarkodu = x.Urun != null ? x.Urun.Barkod : "",
                    KullaniciEposta = x.Kullanici != null ? x.Kullanici.Eposta : "",
                    KullaniciAdiSoyadi = x.Kullanici != null ? (x.Kullanici.KullaniciDetay != null ? (x.Kullanici.KullaniciDetay.Ad + " " + x.Kullanici.KullaniciDetay.Soyad) : "") : "",
                    SepetTipi = x.SepetTip != null ? x.SepetTip.Adi : "",
                    OlusturmaTarihi = x.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm"),
                    GuncellemeTarihi = x.GuncellemeTarihi.ToString("dd.MM.yyyy HH:mm"),
                    AktifMi = x.AktifMi
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}