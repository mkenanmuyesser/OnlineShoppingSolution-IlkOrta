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
using System.IO;

namespace CommerceProject.Admin.Controllers
{
    public class KategoriController : BaseController
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
        IKategoriService KategoriService;
        IUrunService UrunService;
        IUrunKategoriService UrunKategoriService;
        IVergiService VergiService;
        IOzellikService OzellikService;
        IKategoriOzellikService KategoriOzellikService;
        public KategoriController(IIcerikAyarService iIcerikAyarService,
                                  IKullaniciService iKullaniciService,
                                  IKategoriService iKategoriService,
                                  IUrunService iUrunService,
                                  IUrunKategoriService iUrunKategoriService,
                                  IVergiService iVergiService,
                                  IOzellikService iOzellikService,
                                  IKategoriOzellikService iKategoriOzellikService) : base(iIcerikAyarService,
                                                                                          iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            KategoriService = iKategoriService;
            UrunService = iUrunService;
            UrunKategoriService = iUrunKategoriService;
            VergiService = iVergiService;
            OzellikService = iOzellikService;
            KategoriOzellikService = iKategoriOzellikService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.kategori_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Kategori", "Arama", "Kategori Arama İşlemleri", "");

            return View();
        }

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.kategori_save;
            ViewBag.VergiListesi = VergiService.GetAll(true).ToList().Select(x => new { id = x.VergiId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.OzellikListesi = OzellikService.FindBy(x => x.OzellikTipId == 2).ToList();

            var ustKategoriler = KategoriService.GetAll(true).ToList();
            if (id != null)
            {
                ustKategoriler.Remove(ustKategoriler.Single(x => x.KategoriId == id));
            }
            ViewBag.UstKategoriListesi = ustKategoriler.Select(x => new { id = x.KategoriId, text = x.Adi, aktif = x.AktifMi }).ToList();

            if (id == null)
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Kategori", "Kayıt", "Kategori Kayıt İşlemleri", "");
                ViewBag.Kategori = new Kategori();
                ViewBag.KategoriOzellikler = new List<KategoriOzellik>();
            }
            else
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Kategori", "Güncelleme", "Kategori Güncelleme İşlemleri", "");
                ViewBag.Kategori = KategoriService.FindBy(x => x.KategoriId == id).SingleOrDefault();
                ViewBag.KategoriOzellikler = KategoriOzellikService.FindBy(x => x.KategoriId == id).ToList();
            }

            return View();
        }

        [AuthorizeManager]
        public ActionResult List()
        {
            ViewBag.PageName = PageHelper.Pages.kategori_list;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Kategori", "Liste", "Kategori Listesi", "");

            return View();
        }

        public ActionResult KategoriTreeViewPartial()
        {
            ViewBag.KategoriNodeListesi = new PageHelper().FillRecursive(KategoriService.GetAll(true).ToList(), null);

            return PartialView("~/Views/Kategori/Partials/KategoriTreeViewPartial.cshtml");
        }

        public ActionResult KategoriNodePartial(KategoriTreeNodeDataObj node)
        {
            return PartialView("~/Views/Kategori/Partials/KategoriNodePartial.cshtml", node);
        }

        public ActionResult UrunKategoriEkleGuncellePartial(int? id)
        {
            if(id == null)
            {
                ViewBag.UrunKategori = new UrunKategori();
            }
            else
            {
                ViewBag.UrunKategori = UrunKategoriService.FindBy(x => x.UrunKategoriId == id, true, new string[] { "Urun" }).SingleOrDefault();
            }

            return PartialView("~/Views/Kategori/Partials/UrunKategoriEkleGuncellePartial.cshtml");
        }

        public ActionResult ExcelRaporuAl(string adi, string aciklama, int ustKategoriId, int aktiflik)
        {
            var sonucListesi = KategoriService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (ustKategoriId == 0 || x.UstKategoriId == ustKategoriId) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList().Select(x => new
            {
                Adi = x.Adi,
                Aciklama = x.Aciklama,
                Sira = x.Sira,
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }

        #endregion

        #region Ajax Methos
        [HttpGet]
        public JsonResult Ara(string adi, string aciklama, int ustKategoriId, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = KategoriService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (ustKategoriId == 0 || x.UstKategoriId == ustKategoriId) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.KategoriId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    Aciklama = x.Aciklama,
                    Adi = x.Adi,
                    AktifMi = x.AktifMi,
                    KategoriId = x.KategoriId,
                    Resim = x.Resim,
                    Sira = x.Sira
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult KaydetGuncelle(Kategori kategori)
        {
            try
            {
                // Dosya kaydet
                string dosyaPath = null;
                if (!string.IsNullOrEmpty(kategori.Resim))
                {
                    dosyaPath = kategori.Resim;
                }
                else
                {
                    if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                    {
                        var guid = Guid.NewGuid();

                        var dosyaUzantisi = new FileInfo(Request.Files[0].FileName).Extension;

                        var savePath = Path.Combine(Server.MapPath("~/Uploads/Category/"), guid + dosyaUzantisi);
                        dosyaPath = string.Format("/Uploads/Category/{0}",
                                            guid + dosyaUzantisi);

                        Request.Files[0].SaveAs(savePath);
                    }
                }

                Kategori _kategori;

                if (kategori.KategoriId == 0)
                {
                    _kategori = new Kategori();
                    _kategori.Aciklama = kategori.Aciklama;
                    _kategori.Adi = kategori.Adi;
                    _kategori.AktifMi = kategori.AktifMi;
                    _kategori.GuncellemeTarihi = DateTime.Now;
                    _kategori.MetaDescription = kategori.MetaDescription;
                    _kategori.MetaKeywords = kategori.MetaKeywords;
                    _kategori.MetaTitle = kategori.MetaTitle;
                    _kategori.OlusturmaTarihi = DateTime.Now;
                    _kategori.Resim = dosyaPath;
                    _kategori.Sira = kategori.Sira;
                    _kategori.Tarih = kategori.Tarih;
                    _kategori.UstKategoriId = kategori.UstKategoriId;
                    _kategori.VergiId = kategori.VergiId;

                    KategoriService.Add(_kategori);
                }
                else
                {
                    _kategori = KategoriService.FindBy(x => x.KategoriId == kategori.KategoriId).SingleOrDefault();

                    if (_kategori == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _kategori.Aciklama = kategori.Aciklama;
                    _kategori.Adi = kategori.Adi;
                    _kategori.AktifMi = kategori.AktifMi;
                    _kategori.GuncellemeTarihi = DateTime.Now;
                    _kategori.MetaDescription = kategori.MetaDescription;
                    _kategori.MetaKeywords = kategori.MetaKeywords;
                    _kategori.MetaTitle = kategori.MetaTitle;
                    _kategori.Resim = dosyaPath;
                    _kategori.Sira = kategori.Sira;
                    _kategori.Tarih = kategori.Tarih;
                    _kategori.UstKategoriId = kategori.UstKategoriId;
                    _kategori.VergiId = kategori.VergiId;

                    KategoriService.Edit(_kategori);
                }

                var flag = KategoriService.Save();

                if (!flag)
                    return Json(0, JsonRequestBehavior.DenyGet);

                // Kategori özellikleri
                if (kategori.KategoriId > 0)
                {
                    var kategoriOzellikler = KategoriOzellikService.FindBy(x => x.KategoriId == kategori.KategoriId).ToList();
                    foreach (var ko in kategoriOzellikler)
                    {
                        ko.AktifMi = false;

                        KategoriOzellikService.Edit(ko);
                    }
                    KategoriOzellikService.Save();

                    KategoriOzellik _kategoriOzellik;
                    foreach (var kategoriOzellik in kategori.KategoriOzellik)
                    {
                        _kategoriOzellik = KategoriOzellikService.FindBy(x => x.KategoriId == kategori.KategoriId && x.OzellikId == kategoriOzellik.OzellikId).SingleOrDefault();

                        if (_kategoriOzellik != null)
                        {
                            _kategoriOzellik.AktifMi = true;

                            KategoriOzellikService.Edit(_kategoriOzellik);
                        }
                        else
                        {
                            _kategoriOzellik = new KategoriOzellik();
                            _kategoriOzellik.AktifMi = true;
                            _kategoriOzellik.OzellikId = kategoriOzellik.OzellikId;
                            _kategoriOzellik.KategoriId = kategori.KategoriId;

                            KategoriOzellikService.Add(_kategoriOzellik);
                        }
                    }
                    KategoriOzellikService.Save();
                }

                if (flag)
                    return Json(_kategori.KategoriId, JsonRequestBehavior.DenyGet);
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

                var _kategori = KategoriService.FindBy(x => x.KategoriId == id).SingleOrDefault();

                if (_kategori == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _kategori.AktifMi = false;

                KategoriService.Edit(_kategori);

                var flag = KategoriService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult KategoriAra(string term)
        {
            return Json(
                KategoriService.FindBy(x =>
                x.Adi.ToLower().StartsWith(term.ToLower().Trim())
                ).Select(x => new { id = x.KategoriId, text = x.Adi }).ToList(),
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTreeData(string parent)
        {
            var kategoriler = KategoriService.GetAll(true).ToList();

            return Json(kategoriler.Where(x =>
            ((parent == "#" || parent == null) ? x.UstKategoriId == null : x.UstKategoriId.ToString() == parent)
            ).Select(x => new
            {
                id = x.KategoriId,
                text = x.Adi,
                parent = x.UstKategoriId == null ? "#" : x.UstKategoriId.ToString(),
                children = kategoriler.Any(u => u.UstKategoriId == x.KategoriId)
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UrunKategoriAra(int kategoriId, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = UrunKategoriService.FindBy(x => x.KategoriId == kategoriId, true, new string[] { "Urun" });

            var count = tempList.Count();

            var sonucListesi = tempList.OrderByDescending(x => x.UrunKategoriId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    AktifMi = x.AktifMi,
                    UrunKodu = x.Urun.UrunKod,
                    UrunBarkodu = x.Urun.Barkod,
                    UrunAdi = x.Urun.Adi,
                    Sira = x.Sira,
                    UrunId = x.UrunId,
                    UrunKategoriId = x.UrunKategoriId
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UrunKategoriKaydetGuncelle(UrunKategori urunKategori)
        {
            try
            {
                UrunKategori _urunKategori;

                if(urunKategori.UrunKategoriId == 0)
                {
                    // Bu kategori için ürün kaydı daha önce yapılmış mı?
                    var _oncekiUrunKategori = UrunKategoriService.FindBy(x => x.UrunId == urunKategori.UrunId && x.KategoriId == urunKategori.KategoriId && x.AktifMi == true).SingleOrDefault();
                    if (_oncekiUrunKategori != null)
                    {
                        return Json(-1, JsonRequestBehavior.DenyGet);
                    }

                    _urunKategori = new UrunKategori();
                    _urunKategori.AktifMi = urunKategori.AktifMi;
                    _urunKategori.KategoriId = urunKategori.KategoriId;
                    _urunKategori.Sira = urunKategori.Sira;
                    _urunKategori.UrunId = urunKategori.UrunId;

                    UrunKategoriService.Add(_urunKategori);
                }
                else
                {
                    _urunKategori = UrunKategoriService.FindBy(x => x.UrunKategoriId == urunKategori.UrunKategoriId).SingleOrDefault();

                    if(_urunKategori == null)
                        return Json(0, JsonRequestBehavior.DenyGet);

                    _urunKategori.AktifMi = urunKategori.AktifMi;
                    _urunKategori.Sira = urunKategori.Sira;
                    _urunKategori.UrunId = urunKategori.UrunId;

                    UrunKategoriService.Edit(_urunKategori);
                }

                var flag = UrunKategoriService.Save();

                if (flag)
                    return Json(_urunKategori.UrunKategoriId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }
        #endregion
    }
}