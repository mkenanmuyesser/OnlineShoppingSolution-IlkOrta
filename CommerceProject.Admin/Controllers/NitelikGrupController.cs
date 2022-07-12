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
    public class NitelikGrupController : BaseController
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
        INitelikGrupService NitelikGrupService;
        public NitelikGrupController(IIcerikAyarService iIcerikAyarService,
                                     IKullaniciService iKullaniciService,
                                     INitelikGrupService iNitelikGrupService) : base(iIcerikAyarService,
                                                                                     iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            NitelikGrupService = iNitelikGrupService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.nitelikgrup_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Nitelik Grup", "Arama", "Nitelik Grup Arama İşlemleri", "");

            return View();
        }
       
        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.nitelikgrup_save;

            if (id == null)
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Nitelik Grup", "Kayıt", "Nitelik Grup Kayıt İşlemleri", "");
                ViewBag.NitelikGrup = new NitelikGrup();
            }
            else
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Nitelik Grup", "Güncelleme", "Nitelik Grup Güncelleme İşlemleri", "");
                ViewBag.NitelikGrup = NitelikGrupService.FindBy(x => x.NitelikGrupId == id).SingleOrDefault();
            }
            return View();
        }

        public ActionResult ExcelRaporuAl(string adi, string aciklama, int aktiflik)
        {
            var sonucListesi = NitelikGrupService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList().Select(x => new
            {
                Adi = x.Adi,
                Aciklama = x.Aciklama,
                OlusturmaTarihi = x.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm"),
                Sira = x.Sira,
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult Ara(string adi, string aciklama, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = NitelikGrupService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            );

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.NitelikGrupId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    NitelikGrupId = x.NitelikGrupId,
                    Aciklama = x.Aciklama,
                    Adi = x.Adi,
                    AktifMi = x.AktifMi,
                    OlusturmaTarihi = x.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm"),
                    Sira = x.Sira
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult KaydetGuncelle(NitelikGrup nitelikGrup)
        {
            try
            {
                NitelikGrup _nitelikGrup;

                if (nitelikGrup.NitelikGrupId == 0)
                {
                    _nitelikGrup = new NitelikGrup();
                    _nitelikGrup.Aciklama = nitelikGrup.Aciklama;
                    _nitelikGrup.Adi = nitelikGrup.Adi;
                    _nitelikGrup.AktifMi = nitelikGrup.AktifMi;
                    _nitelikGrup.OlusturmaTarihi = DateTime.Now;
                    _nitelikGrup.GuncellemeTarihi = DateTime.Now;
                    _nitelikGrup.Sira = nitelikGrup.Sira;

                    NitelikGrupService.Add(_nitelikGrup);
                }
                else
                {
                    _nitelikGrup = NitelikGrupService.FindBy(x => x.NitelikGrupId == nitelikGrup.NitelikGrupId).SingleOrDefault();

                    if (_nitelikGrup == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _nitelikGrup.Aciklama = nitelikGrup.Aciklama;
                    _nitelikGrup.Adi = nitelikGrup.Adi;
                    _nitelikGrup.AktifMi = nitelikGrup.AktifMi;
                    _nitelikGrup.GuncellemeTarihi = DateTime.Now;
                    _nitelikGrup.Sira = nitelikGrup.Sira;

                    NitelikGrupService.Edit(_nitelikGrup);
                }

                var flag = NitelikGrupService.Save();

                if (flag)
                    return Json(_nitelikGrup.NitelikGrupId, JsonRequestBehavior.DenyGet);
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

                var _nitelikGrup = NitelikGrupService.FindBy(x => x.NitelikGrupId == id).SingleOrDefault();

                if (_nitelikGrup == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _nitelikGrup.AktifMi = false;

                NitelikGrupService.Edit(_nitelikGrup);

                var flag = NitelikGrupService.Save();

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