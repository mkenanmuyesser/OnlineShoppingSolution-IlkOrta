using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.BusinessContracts;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Business.Entities;
using System.Data;
using CommerceProject.Admin.Helper;
using static CommerceProject.Admin.Helper.PageHelper;
using System.IO;

namespace CommerceProject.Admin.Controllers
{
    public class BannerController : BaseController
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
        IBannerService BannerService;
        IBannerTipService BannerTipService;
        public BannerController(IIcerikAyarService iIcerikAyarService,
                                IKullaniciService iKullaniciService,
                                IBannerService iBannerService,
                                IBannerTipService iBannerTipService) : base(iIcerikAyarService,
                                                                            iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            BannerService = iBannerService;
            BannerTipService = iBannerTipService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.banner_index;
            ViewBag.PageProperties = PageProperties.SetPageProperties("Banner", "Arama", "Banner Arama İşlemleri", "");
            ViewBag.BannerTipListesi = BannerTipService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.BannerTipId, text = x.Adi }).ToList();

            return View();
        }
       
        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.banner_save;
            ViewBag.BannerTipListesi = BannerTipService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.BannerTipId, text = x.Adi }).ToList();

            if (id == null)
            {
                ViewBag.Banner = new Banner();
                ViewBag.PageProperties = PageProperties.SetPageProperties("Banner", "Kayıt", "Banner Kayıt İşlemleri", "");
            }
            else
            {
                ViewBag.Banner = BannerService.FindBy(x => x.BannerId == id).SingleOrDefault();
                ViewBag.PageProperties = PageProperties.SetPageProperties("Banner", "Güncelleme", "Banner Güncelleme İşlemleri", "");
            }

            return View();
        }

        public ActionResult ExcelRaporuAl(string adi, int bannerTipId, string aciklama1, string aciklama2, string link, int aktiflik)
        {
            var sonucListesi = BannerService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (bannerTipId == 0 || x.BannerTipId == bannerTipId) &&
            (string.IsNullOrEmpty(aciklama1) || x.Aciklama1.Contains(aciklama1)) &&
            (string.IsNullOrEmpty(aciklama2) || x.Aciklama2.Contains(aciklama2)) &&
            (string.IsNullOrEmpty(link) || x.Link.Contains(link)) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "BannerTip" }).ToList()
            .Select(x => new
            {
                Sira = x.Sira,
                Adi = x.Adi,
                BannerTipi = x.BannerTip != null ? x.BannerTip.Adi : "",
                Aciklama1 = x.Aciklama1,
                Aciklama2 = x.Aciklama2,
                Link = x.Link,
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            })
            .ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult KaydetGuncelle(Banner banner)
        {
            try
            {
                // Dosya kaydet
                string dosyaPath = null;
                if (!string.IsNullOrEmpty(banner.Resim))
                {
                    dosyaPath = banner.Resim;
                }
                else
                {
                    if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                    {
                        var guid = Guid.NewGuid();

                        var dosyaUzantisi = new FileInfo(Request.Files[0].FileName).Extension;

                        var savePath = Path.Combine(Server.MapPath("~/Uploads/Banner/"), guid + dosyaUzantisi);
                        dosyaPath = string.Format("/Uploads/Banner/{0}",
                                            guid + dosyaUzantisi);

                        Request.Files[0].SaveAs(savePath);
                    }
                }

                Banner _banner;

                if (banner.BannerId == 0)
                {
                    _banner = new Banner();
                    _banner.Adi = banner.Adi;
                    _banner.Aciklama1 = banner.Aciklama1;
                    _banner.Aciklama2 = banner.Aciklama2;
                    _banner.Link = banner.Link;
                    _banner.AktifMi = banner.AktifMi;
                    _banner.BannerTipId = banner.BannerTipId;
                    _banner.Sira = banner.Sira;
                    _banner.Resim = dosyaPath;

                    BannerService.Add(_banner);
                }
                else
                {
                    _banner = BannerService.FindBy(x => x.BannerId == banner.BannerId).SingleOrDefault();

                    if (_banner == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _banner.Adi = banner.Adi;
                    _banner.Aciklama1 = banner.Aciklama1;
                    _banner.Aciklama2 = banner.Aciklama2;
                    _banner.Link = banner.Link;
                    _banner.AktifMi = banner.AktifMi;
                    _banner.BannerTipId = banner.BannerTipId;
                    _banner.Sira = banner.Sira;
                    _banner.Resim = dosyaPath;

                    BannerService.Edit(_banner);
                }

                var flag = BannerService.Save();

                if (flag)
                    return Json(_banner.BannerId, JsonRequestBehavior.DenyGet);
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

                var _banner = BannerService.FindBy(x => x.BannerId == id).SingleOrDefault();

                if (_banner == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _banner.AktifMi = false;

                BannerService.Edit(_banner);

                var flag = BannerService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult Ara(string adi, int bannerTipId, string aciklama1, string aciklama2, string link, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = BannerService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (bannerTipId == 0 || x.BannerTipId == bannerTipId) &&
            (string.IsNullOrEmpty(aciklama1) || x.Aciklama1.Contains(aciklama1)) &&
            (string.IsNullOrEmpty(aciklama2) || x.Aciklama2.Contains(aciklama2)) &&
            (string.IsNullOrEmpty(link) || x.Link.Contains(link)) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "BannerTip" });

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.BannerId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    Adi = x.Adi,
                    BannerTipi = x.BannerTip != null ? x.BannerTip.Adi : "",
                    Aciklama1 = x.Aciklama1,
                    Aciklama2 = x.Aciklama2,
                    Link = x.Link,
                    AktifMi = x.AktifMi,
                    Sira = x.Sira,
                    BannerId = x.BannerId,
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