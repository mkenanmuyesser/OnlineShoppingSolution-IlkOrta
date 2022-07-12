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
    public class OlcuController : BaseController
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
        IOlcuService OlcuService;
        IOlcuTipService OlcuTipService;
        public OlcuController(IIcerikAyarService iIcerikAyarService,
                              IKullaniciService iKullaniciService, 
                              IOlcuService iOlcuService, 
                              IOlcuTipService iOlcuTipService) : base(iIcerikAyarService,
                                                                      iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            OlcuService = iOlcuService;
            OlcuTipService = iOlcuTipService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.olcu_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Ölçü", "Arama", "Ölçü Arama İşlemleri", "");
            ViewBag.OlcuTipListesi = OlcuTipService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.OlcuTipId, text = x.Adi }).ToList();

            return View();
        }
       
        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.olcu_save;
            ViewBag.OlcuTipListesi = OlcuTipService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.OlcuTipId, text = x.Adi }).ToList();

            if (id == null)
            {
                ViewBag.Olcu = new Olcu();
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Ölçü", "Kayıt", "Ölçü Kayıt İşlemleri", "");
            }
            else
            {
                ViewBag.Olcu = OlcuService.FindBy(x => x.OlcuId == id).SingleOrDefault();
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Ölçü", "Güncelleme", "Ölçü Güncelleme İşlemleri", "");
            }

            return View();
        }

        public ActionResult ExcelRaporuAl(string adi, int olcuTipId, int aktiflik)
        {
            var olcuTipleri = OlcuTipService.GetAll().ToList();

            var sonucListesi = OlcuService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (olcuTipId == 0 || x.OlcuTipId == olcuTipId) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList()
            .Select(x => new
            {
                Sira = x.Sira,
                Adi = x.Adi,
                OlcuTipi = olcuTipleri.Any(u => u.OlcuTipId == x.OlcuTipId) ? olcuTipleri.First(u => u.OlcuTipId == x.OlcuTipId).Adi : "",
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            })
            .ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult KaydetGuncelle(Olcu olcu)
        {
            try
            {
                Olcu _olcu;
                
                if(olcu.OlcuId == 0)
                {
                    _olcu = new Olcu();
                    _olcu.Adi = olcu.Adi;
                    _olcu.AktifMi = olcu.AktifMi;
                    _olcu.OlcuTipId = olcu.OlcuTipId;
                    _olcu.Sira = olcu.Sira;

                    OlcuService.Add(_olcu);
                }
                else
                {
                    _olcu = OlcuService.FindBy(x => x.OlcuId == olcu.OlcuId).SingleOrDefault();

                    if(_olcu == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _olcu.Adi = olcu.Adi;
                    _olcu.AktifMi = olcu.AktifMi;
                    _olcu.OlcuTipId = olcu.OlcuTipId;
                    _olcu.Sira = olcu.Sira;

                    OlcuService.Edit(_olcu);
                }

                var flag = OlcuService.Save();

                if (flag)
                    return Json(_olcu.OlcuId, JsonRequestBehavior.DenyGet);
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
                if (id == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _olcu = OlcuService.FindBy(x => x.OlcuId == id).SingleOrDefault();

                if (_olcu == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _olcu.AktifMi = false;

                OlcuService.Edit(_olcu);

                var flag = OlcuService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult Ara(string adi, int olcuTipId, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var olcuTipleri = OlcuTipService.FindBy(x => x.AktifMi == true).ToList();

            var tempList = OlcuService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (olcuTipId == 0 || x.OlcuTipId == olcuTipId) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.OlcuId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    Adi = x.Adi,
                    AktifMi = x.AktifMi,
                    Sira = x.Sira,
                    OlcuId = x.OlcuId,
                    OlcuTipi = olcuTipleri.Any(u => u.OlcuTipId == x.OlcuTipId) ? olcuTipleri.First(u => u.OlcuTipId == x.OlcuTipId).Adi : ""
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}