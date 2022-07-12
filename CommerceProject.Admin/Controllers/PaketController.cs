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
    public class PaketController : BaseController
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
        IPaketService PaketService;
        IPaketUrunService PaketUrunService;
        IPaketResimService PaketResimService;
        IPaketKategoriService PaketKategoriService;
        IUrunService UrunService;
        IKategoriService KategoriService;
        IVergiService VergiService;
        INitelikService NitelikService;
        INitelikGrupService NitelikGrupService;
        IPaketNitelikService PaketNitelikService;
        public PaketController(IIcerikAyarService iIcerikAyarService,
                               IKullaniciService iKullaniciService,
                               IPaketService iPaketService,
                               IPaketUrunService iPaketUrunService,
                               IPaketResimService iPaketResimService,
                               IPaketKategoriService iPaketKategoriService,
                               IUrunService iUrunService,
                               IKategoriService iKategoriService,
                               IVergiService iVergiService,
                               INitelikService iNitelikService,
                               INitelikGrupService iNitelikGrupService,
                               IPaketNitelikService iPaketNitelikService) : base(iIcerikAyarService,
                                                                                 iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            PaketService = iPaketService;
            PaketUrunService = iPaketUrunService;
            PaketResimService = iPaketResimService;
            PaketKategoriService = iPaketKategoriService;
            UrunService = iUrunService;
            KategoriService = iKategoriService;
            VergiService = iVergiService;
            NitelikService = iNitelikService;
            NitelikGrupService = iNitelikGrupService;
            PaketNitelikService = iPaketNitelikService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.paket_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Paket", "Arama", "Paket Arama İşlemleri", "");

            return View();
        }      

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.paket_save;
            ViewBag.VergiListesi = VergiService.GetAll(true).ToList().Select(x => new { id = x.VergiId, text = x.Adi + " (" + x.VergiOrani + ")", aktif = x.AktifMi }).ToList();
            ViewBag.NitelikGrupListesi = NitelikGrupService.GetAll(true).ToList().Select(x => new { id = x.NitelikGrupId, text = x.Adi, aktif = x.AktifMi }).ToList();

            if (id == null)
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Paket", "Kayıt", "Paket Kayıt İşlemleri", "");

                ViewBag.Paket = new Paket();
                ViewBag.PaketResimler = new List<PaketResim>();
                ViewBag.PaketKategoriler = new List<PaketKategori>();
                ViewBag.PaketUrunler = new List<PaketUrun>();
                ViewBag.PaketNitelikler = new List<PaketNitelik>();
            }
            else
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Paket", "Güncelleme", "Paket Güncelleme İşlemleri", "");

                ViewBag.Paket = PaketService.FindBy(x => x.PaketId == id).SingleOrDefault();
                ViewBag.PaketResimler = PaketResimService.FindBy(x => x.PaketId == id).ToList();
                ViewBag.PaketKategoriler = PaketKategoriService.FindBy(x => x.PaketId == id, true, new string[] { "Kategori" }).ToList();
                ViewBag.PaketUrunler = PaketUrunService.FindBy(x => x.PaketId == id, true, new string[] { "Urun" }).ToList();
                ViewBag.PaketNitelikler = PaketNitelikService.FindBy(x => x.PaketId == id, true, new string[] { "Nitelik" }).ToList();
            }

            return View();
        }

        public ActionResult PaketUrunPartial(int id)
        {
            ViewBag.PaketUrunler = PaketUrunService.FindBy(x => x.PaketId == id, true, new string[] { "Urun" }).ToList();

            return PartialView("~/Views/Paket/Partials/PaketUrunPartial.cshtml");
        }

        public ActionResult PaketResimPartial(int id)
        {
            ViewBag.PaketResimler = PaketResimService.FindBy(x => x.PaketId == id).ToList();

            return PartialView("~/Views/Paket/Partials/PaketResimPartial.cshtml");
        }

        public ActionResult ExcelRaporuAl(string adi, string kodu, int urunId, int kategoriId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int satinAlmaAktiflik, int onSiparisAktiflik, int aktiflik)
        {
            var sonucListesi = PaketService.GetAll(true, new string[] { "PaketUrun", "PaketKategori", "PaketKategori.Kategori" }).ToList().Where(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(kodu) || x.PaketKod.Contains(kodu)) &&
            (urunId == 0 || x.PaketUrun.Any(u => u.UrunId == urunId)) &&
            (kategoriId == 0 || x.PaketKategori.Any(u => u.KategoriId == kategoriId)) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= x.BaslangicTarihi && baslangicTarihi <= x.BitisTarihi && bitisTarihi >= x.BaslangicTarihi && bitisTarihi >= x.BitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= x.BaslangicTarihi && baslangicTarihi <= x.BitisTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= x.BaslangicTarihi && bitisTarihi >= x.BitisTarihi)))
            ) &&
            (satinAlmaAktiflik == 2 || ((satinAlmaAktiflik == 0 && x.SatinAlAktifMi == false) || (satinAlmaAktiflik == 1 && x.SatinAlAktifMi == true))) &&
            (onSiparisAktiflik == 2 || ((onSiparisAktiflik == 0 && x.OnSiparisAktifMi == false) || (onSiparisAktiflik == 1 && x.OnSiparisAktifMi == true))) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList().Select(x => new
            {
                PaketKod = "#" + x.PaketKod,
                Sira = x.Sira,
                Adi = x.Adi,
                KisaAciklama = x.KisaAciklama,
                BaslangicTarihi = x.BaslangicTarihi.HasValue ? x.BaslangicTarihi.Value.ToString("dd.MM.yyyy") : "-",
                BitisTarihi = x.BitisTarihi.HasValue ? x.BitisTarihi.Value.ToString("dd.MM.yyyy") : "-",
                Kategori = x.PaketKategori.Count > 0 ? string.Join(", ", x.PaketKategori.Where(u => u.AktifMi == true).ToList().Select(u => u.Kategori.Adi)) : "",
                SatinAlAktifMi = x.SatinAlAktifMi ? "Evet" : "Hayır",
                OnSiparisAktifMi = x.OnSiparisAktifMi ? "Evet" : "Hayır",
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult Ara(string adi, string kodu, int urunId, int kategoriId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int satinAlmaAktiflik, int onSiparisAktiflik, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = PaketService.GetAll(true, new string[] { "PaketUrun", "PaketKategori", "PaketKategori.Kategori" }).ToList().Where(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(kodu) || x.PaketKod.Contains(kodu)) &&
            (urunId == 0 || x.PaketUrun.Any(u => u.UrunId == urunId)) &&
            (kategoriId == 0 || x.PaketKategori.Any(u => u.KategoriId == kategoriId)) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= x.BaslangicTarihi && baslangicTarihi <= x.BitisTarihi && bitisTarihi >= x.BaslangicTarihi && bitisTarihi >= x.BitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= x.BaslangicTarihi && baslangicTarihi <= x.BitisTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= x.BaslangicTarihi && bitisTarihi >= x.BitisTarihi)))
            ) &&
            (satinAlmaAktiflik == 2 || ((satinAlmaAktiflik == 0 && x.SatinAlAktifMi == false) || (satinAlmaAktiflik == 1 && x.SatinAlAktifMi == true))) &&
            (onSiparisAktiflik == 2 || ((onSiparisAktiflik == 0 && x.OnSiparisAktifMi == false) || (onSiparisAktiflik == 1 && x.OnSiparisAktifMi == true))) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.PaketId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    Adi = x.Adi,
                    AktifMi = x.AktifMi,
                    PaketId = x.PaketId,
                    BaslangicTarihi = x.BaslangicTarihi.HasValue ? x.BaslangicTarihi.Value.ToString("dd.MM.yyyy") : "-",
                    BitisTarihi = x.BitisTarihi.HasValue ? x.BitisTarihi.Value.ToString("dd.MM.yyyy") : "-",
                    KisaAciklama = x.KisaAciklama,
                    OnSiparisAktifMi = x.OnSiparisAktifMi,
                    Kategori = x.PaketKategori.Count > 0 ? string.Join(", ", x.PaketKategori.Where(u => u.AktifMi == true).ToList().Select(u => u.Kategori.Adi)) : "",
                    PaketKod = "#" + x.PaketKod,
                    SatinAlAktifMi = x.SatinAlAktifMi,
                    Sira = x.Sira
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult KaydetGuncelle(Paket paket)
        {
            try
            {
                Paket _paket;

                if (paket.PaketId == 0)
                {
                    _paket = new Paket();
                    _paket.Adi = paket.Adi;
                    _paket.AktifMi = paket.AktifMi;
                    _paket.BaslangicTarihi = paket.BaslangicTarihi;
                    _paket.BitisTarihi = paket.BitisTarihi;
                    _paket.GuncellemeTarihi = DateTime.Now;
                    _paket.KisaAciklama = paket.KisaAciklama;
                    _paket.MetaDescription = paket.MetaDescription;
                    _paket.MetaKeywords = paket.MetaKeywords;
                    _paket.MetaTitle = paket.MetaTitle;
                    _paket.OlusturmaTarihi = DateTime.Now;
                    _paket.OnSiparisAktifMi = paket.OnSiparisAktifMi;
                    _paket.PaketKod = paket.PaketKod;
                    _paket.SatinAlAktifMi = paket.SatinAlAktifMi;
                    _paket.Sira = paket.Sira;
                    _paket.Tags = paket.Tags;
                    _paket.UzunAciklama = paket.UzunAciklama;
                    _paket.YeniMi = paket.YeniMi;
                    _paket.VergiId = paket.VergiId;
                    _paket.NitelikGrupId = paket.NitelikGrupId;

                    PaketService.Add(_paket);
                }
                else
                {
                    _paket = PaketService.FindBy(x => x.PaketId == paket.PaketId).SingleOrDefault();

                    if (_paket == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _paket.Adi = paket.Adi;
                    _paket.AktifMi = paket.AktifMi;
                    _paket.BaslangicTarihi = paket.BaslangicTarihi;
                    _paket.BitisTarihi = paket.BitisTarihi;
                    _paket.GuncellemeTarihi = DateTime.Now;
                    _paket.KisaAciklama = paket.KisaAciklama;
                    _paket.MetaDescription = paket.MetaDescription;
                    _paket.MetaKeywords = paket.MetaKeywords;
                    _paket.MetaTitle = paket.MetaTitle;
                    _paket.OnSiparisAktifMi = paket.OnSiparisAktifMi;
                    _paket.PaketKod = paket.PaketKod;
                    _paket.SatinAlAktifMi = paket.SatinAlAktifMi;
                    _paket.Sira = paket.Sira;
                    _paket.Tags = paket.Tags;
                    _paket.UzunAciklama = paket.UzunAciklama;
                    _paket.YeniMi = paket.YeniMi;
                    _paket.VergiId = paket.VergiId;
                    _paket.NitelikGrupId = paket.NitelikGrupId;

                    PaketService.Edit(_paket);
                }

                var flag = PaketService.Save();

                if(!flag)
                    return Json(0, JsonRequestBehavior.DenyGet);

                var paketId = _paket.PaketId;

                // Paket nitelikleri
                PaketNitelik _paketNitelik;
                if(paket.PaketId > 0)
                {
                    var paketNitelikListesi = PaketNitelikService.FindBy(x => x.PaketId == paket.PaketId).ToList();
                    foreach (var _pn in paketNitelikListesi)
                    {
                        PaketNitelikService.Delete(_pn);
                    }
                }

                foreach (var pn in paket.PaketNitelik)
                {
                    _paketNitelik = new PaketNitelik();
                    _paketNitelik.AktifMi = pn.AktifMi;
                    _paketNitelik.NitelikDegeri = pn.NitelikDegeri;
                    _paketNitelik.NitelikId = pn.NitelikId;
                    _paketNitelik.PaketId = paketId;
                    _paketNitelik.Sira = pn.Sira;

                    PaketNitelikService.Add(_paketNitelik);
                }

                PaketNitelikService.Save();

                // Paket kategorileri
                if (paket.PaketId > 0)
                {
                    PaketKategori _paketKategori;
                    foreach (var kategori in paket.PaketKategori)
                    {
                        if (kategori.PaketKategoriId == 0)
                        {
                            _paketKategori = new PaketKategori();
                            _paketKategori.AktifMi = kategori.AktifMi;
                            _paketKategori.KategoriId = kategori.KategoriId;
                            _paketKategori.PaketId = paket.PaketId;
                            _paketKategori.Sira = kategori.Sira;

                            PaketKategoriService.Add(_paketKategori);
                        }
                        else
                        {
                            _paketKategori = PaketKategoriService.FindBy(x => x.PaketKategoriId == kategori.PaketKategoriId).SingleOrDefault();

                            if (_paketKategori != null)
                            {
                                _paketKategori.AktifMi = kategori.AktifMi;
                                _paketKategori.Sira = kategori.Sira;

                                PaketKategoriService.Edit(_paketKategori);
                            }
                        }
                    }
                    PaketKategoriService.Save();
                }

                // Paket ürünleri
                if (paket.PaketId > 0)
                {
                    PaketUrun _paketUrun;
                    foreach (var urun in paket.PaketUrun)
                    {
                        if (urun.PaketUrunId == 0)
                        {
                            _paketUrun = new PaketUrun();
                            _paketUrun.Adet = urun.Adet;
                            _paketUrun.AktifMi = urun.AktifMi;
                            _paketUrun.OlusturmaTarihi = DateTime.Now;
                            _paketUrun.GuncellemeTarihi = DateTime.Now;
                            _paketUrun.PaketId = paket.PaketId;
                            _paketUrun.UrunId = urun.UrunId;

                            PaketUrunService.Add(_paketUrun);
                        }
                        else
                        {
                            _paketUrun = PaketUrunService.FindBy(x => x.PaketUrunId == urun.PaketUrunId).SingleOrDefault();

                            if (_paketUrun != null)
                            {
                                _paketUrun.Adet = urun.Adet;
                                _paketUrun.AktifMi = urun.AktifMi;
                                _paketUrun.GuncellemeTarihi = DateTime.Now;
                            }
                        }
                    }
                    PaketUrunService.Save();
                }

                if (flag)
                    return Json(_paket.PaketId, JsonRequestBehavior.DenyGet);
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

                var _paket = PaketService.FindBy(x => x.PaketId == id).SingleOrDefault();

                if (_paket == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _paket.AktifMi = false;

                PaketService.Edit(_paket);

                var flag = PaketService.Save();

                if (!flag)
                    return Json(false, JsonRequestBehavior.DenyGet);

                // İlişkili resimler AktifMi=false
                var _resimler = PaketResimService.FindBy(x => x.AktifMi == true && x.PaketId == id).ToList();

                foreach (var _resim in _resimler)
                {
                    _resim.AktifMi = false;
                    PaketResimService.Edit(_resim);

                    PaketResimService.Save();
                }

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
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

                    var savePath = Path.Combine(Server.MapPath("~/Uploads/Package/"), guid + dosyaUzantisi);
                    dosyaPath = string.Format("/Uploads/Package/{0}",
                                        guid + dosyaUzantisi);

                    Request.Files[0].SaveAs(savePath);

                    // save db
                    var _paketResim = new PaketResim();
                    _paketResim.AktifMi = true;
                    _paketResim.PaketId = key;
                    _paketResim.ResimYolu = dosyaPath;
                    _paketResim.Tarih = DateTime.Now;
                    _paketResim.YeniMi = true;

                    PaketResimService.Add(_paketResim);
                    var flag = PaketResimService.Save();

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

        [HttpPost]
        public JsonResult ResimKaydetGuncelle(PaketResim paketResim)
        {
            try
            {
                if (paketResim == null || paketResim.PaketResimId == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _paketResim = PaketResimService.FindBy(x => x.PaketResimId == paketResim.PaketResimId).SingleOrDefault();

                if (_paketResim == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _paketResim.Aciklama = paketResim.Aciklama;
                _paketResim.AktifMi = paketResim.AktifMi;
                _paketResim.AltAttribute = paketResim.AltAttribute;
                _paketResim.Tarih = paketResim.Tarih;
                _paketResim.Sira = paketResim.Sira;
                _paketResim.TitleAttribute = paketResim.TitleAttribute;
                _paketResim.YeniMi = paketResim.YeniMi;

                PaketResimService.Edit(_paketResim);

                var flag = PaketResimService.Save();

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