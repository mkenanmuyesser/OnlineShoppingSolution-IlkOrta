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
    public class HaberBlogController : BaseController
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
        IHaberBlogService HaberBlogService;
        IHaberBlogResimService HaberBlogResimService;
        IHaberBlogYorumService HaberBlogYorumService;
        IHaberBlogTipService HaberBlogTipService;
        public HaberBlogController(IIcerikAyarService iIcerikAyarService,
                                   IKullaniciService iKullaniciService,
                                   IHaberBlogService iHaberBlogService,
                                   IHaberBlogResimService iHaberBlogResimService,
                                   IHaberBlogYorumService iHaberBlogYorumService,
                                   IHaberBlogTipService iHaberBlogTipService) : base(iIcerikAyarService,
                                                                                     iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            HaberBlogService = iHaberBlogService;
            HaberBlogResimService = iHaberBlogResimService;
            HaberBlogYorumService = iHaberBlogYorumService;
            HaberBlogTipService = iHaberBlogTipService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.haberblog_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Haber/Blog", "Arama", "Haber/Blog Arama İşlemleri", "");
            ViewBag.HaberBlogTipListesi = HaberBlogTipService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.HaberBlogTipId, text = x.Adi }).ToList();

            return View();
        }

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.haberblog_save;
            ViewBag.HaberBlogTipListesi = HaberBlogTipService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.HaberBlogTipId, text = x.Adi }).ToList();

            if (id == null)
            {
                ViewBag.HaberBlog = new HaberBlog();
                ViewBag.HaberBlogResimler = new List<HaberBlogResim>();
                ViewBag.HaberBlogYorumlar = new List<HaberBlogYorum>();

                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Haber/Blog", "Kayıt", "Haber/Blog Kayıt İşlemleri", "");
            }
            else
            {
                ViewBag.HaberBlog = HaberBlogService.FindBy(x => x.HaberBlogId == id).SingleOrDefault();
                ViewBag.HaberBlogResimler = HaberBlogResimService.FindBy(x => x.HaberBlogId == id).ToList();
                ViewBag.HaberBlogYorumlar = HaberBlogYorumService.FindBy(x => x.HaberBlogId == id).ToList();
                ViewBag.Kullanicilar = KullaniciService.GetAll(true).ToList();

                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Haber/Blog", "Güncelleme", "Haber/Blog Güncelleme İşlemleri", "");
            }

            return View();
        }

        public ActionResult ExcelRaporuAl(string baslik, string icerik, int haberBlogTipId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int yorum, int aktiflik)
        {
            var haberBlogTipListesi = HaberBlogTipService.GetAll(true).ToList();

            var sonucListesi = HaberBlogService.FindBy(x =>
            (string.IsNullOrEmpty(baslik) || x.Baslik.Contains(baslik)) &&
            (string.IsNullOrEmpty(icerik) || x.Icerik.Contains(icerik)) &&
            (haberBlogTipId == 0 || x.HaberBlogTipId == haberBlogTipId) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (x.Tarih >= baslangicTarihi && x.Tarih <= bitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (x.Tarih >= baslangicTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (x.Tarih <= bitisTarihi)))
            ) &&
            (yorum == 2 || ((yorum == 0 && x.YorumYapilabilsinMi == false) || (yorum == 1 && x.YorumYapilabilsinMi == true))) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList()
            .Select(x => new
            {
                HaberBlogTipAdi = haberBlogTipListesi.Any(u => u.HaberBlogTipId == x.HaberBlogTipId) ? haberBlogTipListesi.First(u => u.HaberBlogTipId == x.HaberBlogTipId).Adi : "",
                Baslik = x.Baslik,
                Icerik = x.Icerik,
                BaslangicTarihi = x.BaslangicTarihi.HasValue ? x.BaslangicTarihi.Value.ToString("dd.MM.yyyy") : "",
                BitisTarihi = x.BitisTarihi.HasValue ? x.BitisTarihi.Value.ToString("dd.MM.yyyy") : "",
                YayinlanmaTarihi = x.Tarih.ToString("dd.MM.yyyy"),
                YorumYapilabilsinMi = x.YorumYapilabilsinMi ? "Evet" : "Hayır",
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult KaydetGuncelle(HaberBlog haberBlog)
        {
            try
            {
                HaberBlog _haberBlog;

                if (haberBlog.HaberBlogId == 0)
                {
                    _haberBlog = new HaberBlog();
                    _haberBlog.AktifMi = haberBlog.AktifMi;
                    _haberBlog.BaslangicTarihi = haberBlog.BaslangicTarihi;
                    _haberBlog.Baslik = haberBlog.Baslik;
                    _haberBlog.BitisTarihi = haberBlog.BitisTarihi;
                    _haberBlog.HaberBlogTipId = haberBlog.HaberBlogTipId;
                    _haberBlog.Icerik = haberBlog.Icerik;
                    _haberBlog.MetaDescription = haberBlog.MetaDescription;
                    _haberBlog.MetaKeywords = haberBlog.MetaKeywords;
                    _haberBlog.MetaTitle = haberBlog.MetaTitle;
                    _haberBlog.Tarih = haberBlog.Tarih;
                    _haberBlog.YorumYapilabilsinMi = haberBlog.YorumYapilabilsinMi;

                    HaberBlogService.Add(_haberBlog);
                }
                else
                {
                    _haberBlog = HaberBlogService.FindBy(x => x.HaberBlogId == haberBlog.HaberBlogId).SingleOrDefault();

                    if (_haberBlog == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _haberBlog.AktifMi = haberBlog.AktifMi;
                    _haberBlog.BaslangicTarihi = haberBlog.BaslangicTarihi;
                    _haberBlog.Baslik = haberBlog.Baslik;
                    _haberBlog.BitisTarihi = haberBlog.BitisTarihi;
                    _haberBlog.HaberBlogTipId = haberBlog.HaberBlogTipId;
                    _haberBlog.Icerik = haberBlog.Icerik;
                    _haberBlog.MetaDescription = haberBlog.MetaDescription;
                    _haberBlog.MetaKeywords = haberBlog.MetaKeywords;
                    _haberBlog.MetaTitle = haberBlog.MetaTitle;
                    _haberBlog.Tarih = haberBlog.Tarih;
                    _haberBlog.YorumYapilabilsinMi = haberBlog.YorumYapilabilsinMi;

                    HaberBlogService.Edit(_haberBlog);
                }

                var flag = HaberBlogService.Save();

                if (flag)
                    return Json(_haberBlog.HaberBlogId, JsonRequestBehavior.DenyGet);
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

                var _haberBlog = HaberBlogService.FindBy(x => x.HaberBlogId == id).SingleOrDefault();

                if (_haberBlog == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _haberBlog.AktifMi = false;

                HaberBlogService.Edit(_haberBlog);

                var flag = HaberBlogService.Save();

                if (!flag)
                    return Json(false, JsonRequestBehavior.DenyGet);

                // İlişkili resimler AktifMi=false
                var _resimler = HaberBlogResimService.FindBy(x => x.AktifMi == true && x.HaberBlogId == id).ToList();

                foreach (var _resim in _resimler)
                {
                    _resim.AktifMi = false;
                    HaberBlogResimService.Edit(_resim);

                    HaberBlogResimService.Save();
                }

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public ActionResult ResimYukle(int key)
        {
            // Dosya kaydet
            string dosyaPath = null;

            try
            {
                if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                {
                    var guid = Guid.NewGuid();

                    var dosyaUzantisi = new FileInfo(Request.Files[0].FileName).Extension;

                    var savePath = Path.Combine(Server.MapPath("~/Uploads/BlogNews/"), guid + dosyaUzantisi);
                    dosyaPath = string.Format("/Uploads/BlogNews/{0}",
                                        guid + dosyaUzantisi);

                    Request.Files[0].SaveAs(savePath);

                    // save db
                    var _haberBlogResim = new HaberBlogResim();
                    _haberBlogResim.AktifMi = true;
                    _haberBlogResim.HaberBlogId = key;
                    _haberBlogResim.ResimYolu = dosyaPath;
                    _haberBlogResim.Tarih = DateTime.Now;
                    _haberBlogResim.YeniMi = true;

                    HaberBlogResimService.Add(_haberBlogResim);
                    var flag = HaberBlogResimService.Save();

                    if (flag)
                        return Json(new { result = "OK" }, JsonRequestBehavior.DenyGet);
                    return Json(new { result = "ERROR" }, JsonRequestBehavior.DenyGet);
                }
                return Json(new { result = "ERROR" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(new { result = "ERROR" }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult Ara(string baslik, string icerik, int haberBlogTipId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int yorum, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var haberBlogTipListesi = HaberBlogTipService.GetAll(true).ToList();

            var tempList = HaberBlogService.FindBy(x =>
            (string.IsNullOrEmpty(baslik) || x.Baslik.Contains(baslik)) &&
            (string.IsNullOrEmpty(icerik) || x.Icerik.Contains(icerik)) &&
            (haberBlogTipId == 0 || x.HaberBlogTipId == haberBlogTipId) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (x.Tarih >= baslangicTarihi && x.Tarih <= bitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (x.Tarih >= baslangicTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (x.Tarih <= bitisTarihi)))
            ) &&
            (yorum == 2 || ((yorum == 0 && x.YorumYapilabilsinMi == false) || (yorum == 1 && x.YorumYapilabilsinMi == true))) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.HaberBlogId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    HaberBlogId = x.HaberBlogId,
                    AktifMi = x.AktifMi,
                    BaslangicTarihi = x.BaslangicTarihi.HasValue ? x.BaslangicTarihi.Value.ToString("dd.MM.yyyy") : "",
                    Baslik = x.Baslik,
                    BitisTarihi = x.BitisTarihi.HasValue ? x.BitisTarihi.Value.ToString("dd.MM.yyyy") : "",
                    HaberBlogTipId = x.HaberBlogTipId,
                    HaberBlogTipAdi = haberBlogTipListesi.Any(u => u.HaberBlogTipId == x.HaberBlogTipId) ? haberBlogTipListesi.First(u => u.HaberBlogTipId == x.HaberBlogTipId).Adi : "",
                    Icerik = x.Icerik,
                    MetaDescription = x.MetaDescription,
                    MetaKeywords = x.MetaKeywords,
                    MetaTitle = x.MetaTitle,
                    Tarih = x.Tarih.ToString("dd.MM.yyyy"),
                    YorumYapilabilsinMi = x.YorumYapilabilsinMi
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult YorumAktiflikDegistir(int yorumId, bool aktiflik)
        {
            try
            {
                if (yorumId == 0)
                    return Json(false, JsonRequestBehavior.AllowGet);

                var _haberBlogYorum = HaberBlogYorumService.FindBy(x => x.HaberBlogYorumId == yorumId).SingleOrDefault();

                if (_haberBlogYorum == null)
                    return Json(false, JsonRequestBehavior.AllowGet);

                _haberBlogYorum.AktifMi = aktiflik;

                HaberBlogYorumService.Edit(_haberBlogYorum);

                var flag = HaberBlogYorumService.Save();

                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ResimKaydetGuncelle(HaberBlogResim haberBlogResim)
        {
            try
            {
                if (haberBlogResim == null || haberBlogResim.ResimId == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _haberBlogResim = HaberBlogResimService.FindBy(x => x.ResimId == haberBlogResim.ResimId).SingleOrDefault();

                if (_haberBlogResim == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _haberBlogResim.Aciklama = haberBlogResim.Aciklama;
                _haberBlogResim.AktifMi = haberBlogResim.AktifMi;
                _haberBlogResim.AltAttribute = haberBlogResim.AltAttribute;
                _haberBlogResim.Tarih = haberBlogResim.Tarih;
                _haberBlogResim.TitleAttribute = haberBlogResim.TitleAttribute;
                _haberBlogResim.YeniMi = haberBlogResim.YeniMi;

                HaberBlogResimService.Edit(_haberBlogResim);

                var flag = HaberBlogResimService.Save();

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