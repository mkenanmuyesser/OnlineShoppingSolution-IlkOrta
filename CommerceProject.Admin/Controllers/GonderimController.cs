using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.BusinessContracts;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Business.Entities;
using System.Data;
using CommerceProject.Admin.Helper;
using System.IO;

namespace CommerceProject.Admin.Controllers
{
    public class GonderimController : BaseController
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
        IGonderimService GonderimService;
        public GonderimController(IIcerikAyarService iIcerikAyarService,
                                  IKullaniciService iKullaniciService,
                                  IGonderimService iGonderimService) : base(iIcerikAyarService,
                                                                            iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            GonderimService = iGonderimService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.gonderim_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Gönderim", "Arama", "Gönderim Arama İşlemleri", "");

            return View();
        }

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.gonderim_save;

            if (id == null)
            {
                ViewBag.Gonderim = new Gonderim();
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Gönderim", "Kayıt", "Gönderim Kayıt İşlemleri", "");
            }
            else
            {
                ViewBag.Gonderim = GonderimService.FindBy(x => x.GonderimId == id).SingleOrDefault();
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Gönderim", "Güncelleme", "Gönderim Güncelleme İşlemleri", "");
            }

            return View();
        }

        public ActionResult ExcelRaporuAl(string adi, string aciklama, decimal altLimitBaslangic, decimal altLimitBitis, decimal fiyatBaslangic, decimal fiyatBitis, int aktiflik)
        {
            var sonucListesi = GonderimService.FindBy(x =>
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (
                (altLimitBaslangic == 0 && altLimitBitis == 0) ||
                ((altLimitBaslangic > 0 && altLimitBitis > 0) && (altLimitBaslangic <= x.AltLimit && altLimitBitis >= x.AltLimit)) ||
                ((altLimitBaslangic > 0 && altLimitBitis == 0) && (altLimitBaslangic <= x.AltLimit)) ||
                ((altLimitBaslangic == 0 && altLimitBitis > 0) && (altLimitBitis >= x.AltLimit))
            ) &&
            (
                (fiyatBaslangic == 0 && fiyatBitis == 0) ||
                ((fiyatBaslangic > 0 && fiyatBitis > 0) && (fiyatBaslangic <= x.Fiyat && fiyatBitis >= x.Fiyat)) ||
                ((fiyatBaslangic > 0 && fiyatBitis == 0) && (fiyatBaslangic <= x.Fiyat)) ||
                ((fiyatBaslangic == 0 && fiyatBitis > 0) && (fiyatBitis >= x.Fiyat))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList()
            .Select(x => new
            {
                Sira = x.Sira,
                Adi = x.Adi,
                Aciklama = x.Aciklama,
                GonderimSorgulamaLink = x.GonderimSorgulamaLink,
                Fiyat = x.Fiyat,
                AltLimit = x.AltLimit,
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            })
            .ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult KaydetGuncelle(Gonderim gonderim)
        {
            try
            {
                // Dosya kaydet
                string dosyaPath = null;
                if (!string.IsNullOrEmpty(gonderim.Resim))
                {
                    dosyaPath = gonderim.Resim;
                }
                else
                {
                    if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                    {
                        var guid = Guid.NewGuid();

                        var dosyaUzantisi = new FileInfo(Request.Files[0].FileName).Extension;

                        var savePath = Path.Combine(Server.MapPath("~/Uploads/Shipping/"), guid + dosyaUzantisi);
                        dosyaPath = string.Format("/Uploads/Shipping/{0}",
                                            guid + dosyaUzantisi);

                        Request.Files[0].SaveAs(savePath);
                    }
                }

                Gonderim _gonderim;

                if (gonderim.GonderimId == 0)
                {
                    _gonderim = new Gonderim();
                    _gonderim.Aciklama = gonderim.Aciklama;
                    _gonderim.GonderimSorgulamaLink = gonderim.GonderimSorgulamaLink;
                    _gonderim.Adi = gonderim.Adi;
                    _gonderim.AktifMi = gonderim.AktifMi;
                    _gonderim.AltLimit = gonderim.AltLimit;
                    _gonderim.Fiyat = gonderim.Fiyat;
                    _gonderim.Sira = gonderim.Sira;
                    _gonderim.Resim = dosyaPath;

                    GonderimService.Add(_gonderim);
                }
                else
                {
                    _gonderim = GonderimService.FindBy(x => x.GonderimId == gonderim.GonderimId).SingleOrDefault();

                    if (_gonderim == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _gonderim.Aciklama = gonderim.Aciklama;
                    _gonderim.GonderimSorgulamaLink = gonderim.GonderimSorgulamaLink;
                    _gonderim.Adi = gonderim.Adi;
                    _gonderim.AktifMi = gonderim.AktifMi;
                    _gonderim.AltLimit = gonderim.AltLimit;
                    _gonderim.Fiyat = gonderim.Fiyat;
                    _gonderim.Sira = gonderim.Sira;
                    _gonderim.Resim = dosyaPath;

                    GonderimService.Edit(_gonderim);
                }

                var flag = GonderimService.Save();

                if (flag)
                    return Json(_gonderim.GonderimId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
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

                var _gonderim = GonderimService.FindBy(x => x.GonderimId == id).SingleOrDefault();

                if (_gonderim == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _gonderim.AktifMi = false;

                GonderimService.Edit(_gonderim);

                var flag = GonderimService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult Ara(string adi, string aciklama, decimal altLimitBaslangic, decimal altLimitBitis, decimal fiyatBaslangic, decimal fiyatBitis, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = GonderimService.FindBy(x =>
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (
                (altLimitBaslangic == 0 && altLimitBitis == 0) ||
                ((altLimitBaslangic > 0 && altLimitBitis > 0) && (altLimitBaslangic <= x.AltLimit && altLimitBitis >= x.AltLimit)) ||
                ((altLimitBaslangic > 0 && altLimitBitis == 0) && (altLimitBaslangic <= x.AltLimit)) ||
                ((altLimitBaslangic == 0 && altLimitBitis > 0) && (altLimitBitis >= x.AltLimit))
            ) &&
            (
                (fiyatBaslangic == 0 && fiyatBitis == 0) ||
                ((fiyatBaslangic > 0 && fiyatBitis > 0) && (fiyatBaslangic <= x.Fiyat && fiyatBitis >= x.Fiyat)) ||
                ((fiyatBaslangic > 0 && fiyatBitis == 0) && (fiyatBaslangic <= x.Fiyat)) ||
                ((fiyatBaslangic == 0 && fiyatBitis > 0) && (fiyatBitis >= x.Fiyat))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.GonderimId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    Aciklama = x.Aciklama,
                    GonderimSorgulamaLink = x.GonderimSorgulamaLink,
                    Adi = x.Adi,
                    AktifMi = x.AktifMi,
                    Fiyat = x.Fiyat,
                    AltLimit = x.AltLimit,
                    Sira = x.Sira,
                    GonderimId = x.GonderimId,
                    Resim = x.Resim
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}