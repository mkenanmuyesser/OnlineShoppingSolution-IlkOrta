using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.BusinessContracts;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Business.Entities;
using System.Data;
using CommerceProject.Admin.Helper;

namespace CommerceProject.Admin.Controllers
{
    public class VergiController : BaseController
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
        IVergiService VergiService;
        public VergiController(IIcerikAyarService iIcerikAyarService,
                               IKullaniciService iKullaniciService, 
                               IVergiService iVergiService) : base(iIcerikAyarService,
                                                                   iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            VergiService = iVergiService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.vergi_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Vergi", "Arama", "Vergi Arama İşlemleri", "");
            
            return View();
        }       

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.vergi_save;

            if (id == null)
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Vergi", "Kayıt", "Vergi Kayıt İşlemleri", "");
                ViewBag.Vergi = new Vergi();
            }
            else
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Vergi", "Güncelleme", "Vergi Güncelleme İşlemleri", "");
                ViewBag.Vergi = VergiService.FindBy(x => x.VergiId == id).SingleOrDefault();
            }
            return View();
        }

        public ActionResult ExcelRaporuAl(string adi, string aciklama, decimal? vergiOraniAlt, decimal? vergiOraniUst, int aktiflik)
        {
            var sonucListesi = VergiService.FindBy(x =>
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (
                (vergiOraniAlt == null && vergiOraniUst == null) ||
                (((vergiOraniAlt != null && vergiOraniUst != null) && (x.VergiOrani >= vergiOraniAlt && x.VergiOrani <= vergiOraniUst))) ||
                (((vergiOraniAlt != null && vergiOraniUst == null) && (x.VergiOrani >= vergiOraniAlt))) ||
                (((vergiOraniAlt == null && vergiOraniUst != null) && (x.VergiOrani <= vergiOraniUst)))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList()
            .Select(x => new
            {
                Sira = x.Sira,
                Adi = x.Adi,
                Aciklama = x.Aciklama,
                VergiOrani = x.VergiOrani.ToString("#.##"),
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            })
            .ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult KaydetGuncelle(Vergi vergi)
        {
            try
            {
                Vergi _vergi;

                if (vergi.VergiId == 0)
                {
                    _vergi = new Vergi();
                    _vergi.Aciklama = vergi.Aciklama;
                    _vergi.Adi = vergi.Adi;
                    _vergi.AktifMi = vergi.AktifMi;
                    _vergi.Sira = vergi.Sira;
                    _vergi.VergiOrani = vergi.VergiOrani;

                    VergiService.Add(_vergi);
                }
                else
                {
                    _vergi = VergiService.FindBy(x => x.VergiId == vergi.VergiId).SingleOrDefault();

                    if(_vergi == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _vergi.Aciklama = vergi.Aciklama;
                    _vergi.Adi = vergi.Adi;
                    _vergi.AktifMi = vergi.AktifMi;
                    _vergi.Sira = vergi.Sira;
                    _vergi.VergiOrani = vergi.VergiOrani;

                    VergiService.Edit(_vergi);
                }

                var flag = VergiService.Save();

                if(flag)
                    return Json(_vergi.VergiId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult Sil(int id)
        {
            try
            {
                if(id == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _vergi = VergiService.FindBy(x => x.VergiId == id).SingleOrDefault();

                if (_vergi == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _vergi.AktifMi = false;

                VergiService.Edit(_vergi);

                var flag = VergiService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult Ara(string adi, string aciklama, decimal? vergiOraniAlt, decimal? vergiOraniUst, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = VergiService.FindBy(x =>
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (
                (vergiOraniAlt == null && vergiOraniUst == null) ||
                (((vergiOraniAlt != null && vergiOraniUst != null) && (x.VergiOrani >= vergiOraniAlt && x.VergiOrani <= vergiOraniUst))) ||
                (((vergiOraniAlt != null && vergiOraniUst == null) && (x.VergiOrani >= vergiOraniAlt))) ||
                (((vergiOraniAlt == null && vergiOraniUst != null) && (x.VergiOrani <= vergiOraniUst)))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.VergiId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    Aciklama = x.Aciklama,
                    Adi = x.Adi,
                    AktifMi = x.AktifMi,
                    Sira = x.Sira,
                    VergiId = x.VergiId,
                    VergiOrani = x.VergiOrani.ToString("#.##")
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}