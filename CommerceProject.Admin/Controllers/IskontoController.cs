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
using static CommerceProject.Admin.Helper.PageHelper;

namespace CommerceProject.Admin.Controllers
{
    public class IskontoController : BaseController
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
        IIskontoService IskontoService;
        IIskontoTipService IskontoTipService;
        IIskontoKategoriService IskontoKategoriService;
        IIskontoKullanimHareketService IskontoKullanimHareketService;
        IIskontoMarkaService IskontoMarkaService;
        IIskontoRolService IskontoRolService;
        IIskontoSirketService IskontoSirketService;
        IIskontoUrunService IskontoUrunService;
        IKategoriService KategoriService;
        ISiparisService SiparisService;
        IMarkaService MarkaService;
        IRolService RolService;
        ISirketService SirketService;
        IUrunService UrunService;
        public IskontoController(IIcerikAyarService iIcerikAyarService,
                                 IKullaniciService iKullaniciService,
                                 IIskontoService iIskontoService,
                                 IIskontoTipService iIskontoTipService,
                                 IIskontoKategoriService iIskontoKategoriService,
                                 IIskontoKullanimHareketService iIskontoKullanimHareketService,
                                 IIskontoMarkaService iIskontoMarkaService,
                                 IIskontoRolService iIskontoRolService,
                                 IIskontoSirketService iIskontoSirketService,
                                 IIskontoUrunService iIskontoUrunService,
                                 IKategoriService iKategoriService,
                                 ISiparisService iSiparisService,
                                 IMarkaService iMarkaService,
                                 IRolService iRolService,
                                 ISirketService iSirketService,
                                 IUrunService iUrunService) : base(iIcerikAyarService,
                                                                   iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            IskontoService = iIskontoService;
            IskontoTipService = iIskontoTipService;
            IskontoKategoriService = iIskontoKategoriService;
            IskontoKullanimHareketService = iIskontoKullanimHareketService;
            IskontoMarkaService = iIskontoMarkaService;
            IskontoRolService = iIskontoRolService;
            IskontoSirketService = iIskontoSirketService;
            IskontoUrunService = iIskontoUrunService;
            KategoriService = iKategoriService;
            SiparisService = iSiparisService;
            MarkaService = iMarkaService;
            RolService = iRolService;
            SirketService = iSirketService;
            UrunService = iUrunService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = Pages.iskonto_index;
            ViewBag.PageProperties = PageProperties.SetPageProperties("İskonto", "Arama", "İskonto Arama İşlemleri", "");
            ViewBag.IskontoTipListesi = IskontoTipService.GetAll(true).ToList().Select(x => new { id = x.IskontoTipId, text = x.Adi, aktif = x.AktifMi }).ToList();

            return View();
        }    

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = Pages.iskonto_save;
            ViewBag.IskontoTipListesi = IskontoTipService.GetAll(true).ToList().Select(x => new { id = x.IskontoTipId, text = x.Adi, aktif = x.AktifMi }).ToList();

            var rolListesi = RolService.GetAll(true).ToList();
            ViewBag.RolListesi = rolListesi.Select(x => new { id = x.RolId, text = x.Adi, aktif = x.AktifMi }).ToList();

            var sirketListesi = SirketService.GetAll(true).ToList();
            ViewBag.SirketListesi = sirketListesi.Select(x => new { id = x.SirketId, text = x.Adi, aktif = x.AktifMi }).ToList();

            if (id == null)
            {
                ViewBag.PageProperties = PageProperties.SetPageProperties("İskonto", "Kayıt", "İskonto Kayıt İşlemleri", "");
                ViewBag.Iskonto = new Iskonto();

                ViewBag.IskontoUrunler = new List<IskontoUrun>();
                ViewBag.IskontoKategoriler = new List<IskontoKategori>();
                ViewBag.IskontoMarkalar = new List<IskontoMarka>();
                ViewBag.IskontoRoller = new List<IskontoRol>();
                ViewBag.IskontoSirketler = new List<IskontoSirket>();
            }
            else
            {
                ViewBag.PageProperties = PageProperties.SetPageProperties("İskonto", "Güncelleme", "İskonto Güncelleme İşlemleri", "");
                ViewBag.Iskonto = IskontoService.FindBy(x => x.IskontoId == id).SingleOrDefault();

                ViewBag.IskontoUrunler = IskontoUrunService.FindBy(x => x.IskontoId == id, true, new string[] { "Urun" }).ToList();
                ViewBag.IskontoKategoriler = IskontoKategoriService.FindBy(x => x.IskontoId == id, true, new string[] { "Kategori" }).ToList();
                ViewBag.IskontoMarkalar = IskontoMarkaService.FindBy(x => x.IskontoId == id, true, new string[] { "Marka" }).ToList();
                ViewBag.IskontoRoller = IskontoRolService.FindBy(x => x.IskontoId == id, true, new string[] { "Rol" }).ToList();
                ViewBag.IskontoSirketler = IskontoSirketService.FindBy(x => x.IskontoId == id, true, new string[] { "Sirket" }).ToList();
            }

            return View();
        }

        public ActionResult ExcelRaporuAl(string adi, string aciklama, int iskontoTipId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int hediyeAktiflik, int aktiflik)
        {
            var iskontoTipListesi = IskontoTipService.GetAll(true).ToList();

            var sonucListesi = IskontoService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (iskontoTipId == 0 || x.IskontoTipId == iskontoTipId) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= x.BaslangicTarih && baslangicTarihi <= x.BitisTarih && bitisTarihi >= x.BaslangicTarih && bitisTarihi >= x.BitisTarih))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= x.BaslangicTarih && baslangicTarihi <= x.BitisTarih))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= x.BaslangicTarih && bitisTarihi >= x.BitisTarih)))
            ) &&
            (hediyeAktiflik == 2 || ((hediyeAktiflik == 0 && x.HediyeKartAktifMi == false) || (hediyeAktiflik == 1 && x.HediyeKartAktifMi == true))) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList().Select(x => new
            {
                Adi = x.Adi,
                Aciklama = x.Aciklama,
                IskontoTipi = iskontoTipListesi.Any(u => u.IskontoTipId == x.IskontoTipId) ? iskontoTipListesi.First(u => u.IskontoTipId == x.IskontoTipId).Adi : "?",
                IskontoMiktari = x.IskontoMiktari,
                MinimumFiyat = x.MinimumFiyat,
                MaksimumFiyat = x.MaksimumFiyat,
                HediyeKartAktifMi = x.HediyeKartAktifMi ? "Aktif" : "Pasif",
                BaslangicTarihi = x.BaslangicTarih.HasValue ? x.BaslangicTarih.Value.ToString("dd.MM.yyyy") : "?",
                BitisTarihi = x.BitisTarih.HasValue ? x.BitisTarih.Value.ToString("dd.MM.yyyy") : "?",
                Sira = x.Sira,
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }

        public ActionResult KullanimHareketExcelRaporuAl(int iskontoId, DateTime? baslangicTarihi, DateTime? bitisTarihi)
        {
            var iskontoTipListesi = IskontoTipService.GetAll(true).ToList();

            var sonucListesi = IskontoKullanimHareketService.FindBy(x =>
            x.IskontoId == iskontoId &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (x.Tarih >= baslangicTarihi && x.Tarih <= bitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (x.Tarih >= baslangicTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (x.Tarih <= bitisTarihi)))
            ), true, new string[] { "Siparis", "Siparis.Kullanici" }).ToList().Select(x => new
            {
                KullaniciEposta = x.Siparis.Kullanici.Eposta,
                SiparisNo = x.SiparisId,
                SiparisToplam = x.Siparis.OdenecekTutar,
                SiparisIskonto = x.Siparis.ToplamIskonto,
                KullanimTarihi = x.Tarih.ToString("dd.MM.yyyy HH:mm"),
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }

        public ActionResult IskontoDetayPartial(int id)
        {
            ViewBag.IskontoUrunler = IskontoUrunService.FindBy(x => x.IskontoId == id, true, new string[] { "Urun" }).ToList().Select(x => new IskontoUrun()
            {
                AktifMi = x.AktifMi,
                IskontoId = x.IskontoId,
                IskontoUrunId = x.IskontoUrunId,
                UrunId = x.UrunId,
                Urun = x.Urun
            }).ToList();

            ViewBag.IskontoKategoriler = IskontoKategoriService.FindBy(x => x.IskontoId == id, true, new string[] { "Kategori" }).ToList().Select(x => new IskontoKategori()
            {
                AktifMi = x.AktifMi,
                AltKategorilerdeGecerliMi = x.AltKategorilerdeGecerliMi,
                IskontoId = x.IskontoId,
                IskontoKategoriId = x.IskontoKategoriId,
                KategoriId = x.KategoriId,
                Kategori = x.Kategori
            }).ToList();

            ViewBag.IskontoMarkalar = IskontoMarkaService.FindBy(x => x.IskontoId == id, true, new string[] { "Marka" }).ToList().Select(x => new IskontoMarka()
            {
                AktifMi = x.AktifMi,
                IskontoId = x.IskontoId,
                IskontoMarkaId = x.IskontoMarkaId,
                MarkaId = x.MarkaId,
                Marka = x.Marka
            }).ToList();

            ViewBag.IskontoRoller = IskontoRolService.FindBy(x => x.IskontoId == id, true, new string[] { "Rol" }).ToList().Select(x => new IskontoRol()
            {
                AktifMi = x.AktifMi,
                IskontoId = x.IskontoId,
                IskontoRolId = x.IskontoRolId,
                RolId = x.RolId,
                Rol = x.Rol
            }).ToList();

            ViewBag.IskontoSirketler = IskontoSirketService.FindBy(x => x.IskontoId == id, true, new string[] { "Sirket" }).ToList().Select(x => new IskontoSirket()
            {
                AktifMi = x.AktifMi,
                IskontoId = x.IskontoId,
                IskontoSirketId = x.IskontoSirketId,
                SirketId = x.SirketId,
                Sirket = x.Sirket
            }).ToList();

            return PartialView("~/Views/Iskonto/Partials/IskontoDetayPartial.cshtml");
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult Ara(string adi, string aciklama, int iskontoTipId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int hediyeAktiflik, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var iskontoTipListesi = IskontoTipService.GetAll(true).ToList();

            var tempList = IskontoService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (iskontoTipId == 0 || x.IskontoTipId == iskontoTipId) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= x.BaslangicTarih && baslangicTarihi <= x.BitisTarih && bitisTarihi >= x.BaslangicTarih && bitisTarihi >= x.BitisTarih))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= x.BaslangicTarih && baslangicTarihi <= x.BitisTarih))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= x.BaslangicTarih && bitisTarihi >= x.BitisTarih)))
            ) &&
            (hediyeAktiflik == 2 || ((hediyeAktiflik == 0 && x.HediyeKartAktifMi == false) || (hediyeAktiflik == 1 && x.HediyeKartAktifMi == true))) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.IskontoId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    Aciklama = x.Aciklama,
                    Adi = x.Adi,
                    AktifMi = x.AktifMi,
                    BaslangicTarih = x.BaslangicTarih.HasValue ? x.BaslangicTarih.Value.ToString("dd.MM.yyyy") : "?",
                    BitisTarih = x.BitisTarih.HasValue ? x.BitisTarih.Value.ToString("dd.MM.yyyy") : "?",
                    HediyeKartAktifMi = x.HediyeKartAktifMi,
                    HediyeKartKuponKod = x.HediyeKartKuponKod,
                    IskontoId = x.IskontoId,
                    IskontoMiktari = x.IskontoMiktari,
                    IskontoTipId = x.IskontoTipId,
                    IskontoTipi = iskontoTipListesi.Any(u => u.IskontoTipId == x.IskontoTipId) ? iskontoTipListesi.First(u => u.IskontoTipId == x.IskontoTipId).Adi : "?",
                    MaksimumFiyat = x.MaksimumFiyat,
                    MinimumFiyat = x.MinimumFiyat,
                    Sira = x.Sira
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Sil(int id)
        {
            try
            {
                if (id == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _iskonto = IskontoService.FindBy(x => x.IskontoId == id).SingleOrDefault();

                if (_iskonto == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _iskonto.AktifMi = false;

                IskontoService.Edit(_iskonto);

                var flag = IskontoService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult IskontoBilgiKaydetGuncelle(IskontoBilgiDataObj iskontoBilgi)
        {
            try
            {
                Iskonto _iskonto;

                if (iskontoBilgi.IskontoId == 0)
                {
                    _iskonto = new Iskonto();

                    _iskonto.Aciklama = iskontoBilgi.Aciklama;
                    _iskonto.Adi = iskontoBilgi.Adi;
                    _iskonto.AktifMi = iskontoBilgi.AktifMi;
                    _iskonto.BaslangicTarih = iskontoBilgi.BaslangicTarih;
                    _iskonto.BitisTarih = iskontoBilgi.BitisTarih;
                    _iskonto.HediyeKartAktifMi = iskontoBilgi.HediyeKartAktifMi;
                    _iskonto.HediyeKartKuponKod = iskontoBilgi.HediyeKartKuponKod;
                    _iskonto.IskontoMiktari = iskontoBilgi.IskontoMiktari;
                    _iskonto.IskontoTipId = iskontoBilgi.IskontoTipId;
                    _iskonto.MaksimumFiyat = iskontoBilgi.MaksimumFiyat;
                    _iskonto.MinimumFiyat = iskontoBilgi.MinimumFiyat;
                    _iskonto.Sira = iskontoBilgi.Sira;

                    IskontoService.Add(_iskonto);
                }
                else
                {
                    _iskonto = IskontoService.FindBy(x => x.IskontoId == iskontoBilgi.IskontoId).SingleOrDefault();

                    if (_iskonto == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _iskonto.Aciklama = iskontoBilgi.Aciklama;
                    _iskonto.Adi = iskontoBilgi.Adi;
                    _iskonto.AktifMi = iskontoBilgi.AktifMi;
                    _iskonto.BaslangicTarih = iskontoBilgi.BaslangicTarih;
                    _iskonto.BitisTarih = iskontoBilgi.BitisTarih;
                    _iskonto.HediyeKartAktifMi = iskontoBilgi.HediyeKartAktifMi;
                    _iskonto.HediyeKartKuponKod = iskontoBilgi.HediyeKartKuponKod;
                    _iskonto.IskontoMiktari = iskontoBilgi.IskontoMiktari;
                    _iskonto.IskontoTipId = iskontoBilgi.IskontoTipId;
                    _iskonto.MaksimumFiyat = iskontoBilgi.MaksimumFiyat;
                    _iskonto.MinimumFiyat = iskontoBilgi.MinimumFiyat;
                    _iskonto.Sira = iskontoBilgi.Sira;

                    IskontoService.Edit(_iskonto);
                }

                var flag = IskontoService.Save();

                if (flag)
                    return Json(_iskonto.IskontoId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult IskontoUrunKaydetGuncelle(IskontoUrunBilgiDataObj iskontoUrun)
        {
            try
            {
                // İskonto Ürün Listesi
                foreach (var iu in (iskontoUrun.IskonUrunListesi != null ? iskontoUrun.IskonUrunListesi : new List<IskontoUrun>()))
                {
                    if (iu.IskontoUrunId == 0)
                    {
                        var _iu = new IskontoUrun();
                        _iu.AktifMi = iu.AktifMi;
                        _iu.IskontoId = iskontoUrun.IskontoId;
                        _iu.UrunId = iu.UrunId;

                        IskontoUrunService.Add(_iu);
                    }
                    else
                    {
                        var _iu = IskontoUrunService.FindBy(x => x.IskontoUrunId == iu.IskontoUrunId).SingleOrDefault();

                        if (_iu != null)
                        {
                            _iu.AktifMi = iu.AktifMi;
                            _iu.IskontoId = iskontoUrun.IskontoId;
                            _iu.UrunId = iu.UrunId;

                            IskontoUrunService.Edit(_iu);
                        }
                    }
                    IskontoUrunService.Save();
                }

                // İskonto Kategori Listesi
                foreach (var ik in (iskontoUrun.IskontoKategoriListesi != null ? iskontoUrun.IskontoKategoriListesi : new List<IskontoKategori>()))
                {
                    if (ik.IskontoKategoriId == 0)
                    {
                        var _ik = new IskontoKategori();
                        _ik.AktifMi = ik.AktifMi;
                        _ik.AltKategorilerdeGecerliMi = ik.AltKategorilerdeGecerliMi;
                        _ik.IskontoId = iskontoUrun.IskontoId;
                        _ik.KategoriId = ik.KategoriId;

                        IskontoKategoriService.Add(_ik);
                    }
                    else
                    {
                        var _ik = IskontoKategoriService.FindBy(x => x.IskontoKategoriId == ik.IskontoKategoriId).SingleOrDefault();

                        if (_ik != null)
                        {
                            _ik.AktifMi = ik.AktifMi;
                            _ik.AltKategorilerdeGecerliMi = ik.AltKategorilerdeGecerliMi;
                            _ik.IskontoId = iskontoUrun.IskontoId;
                            _ik.KategoriId = ik.KategoriId;

                            IskontoKategoriService.Edit(_ik);
                        }
                    }
                    IskontoKategoriService.Save();
                }

                // İskonto Marka Listesi
                foreach (var im in (iskontoUrun.IskontoMarkaListesi != null ? iskontoUrun.IskontoMarkaListesi : new List<IskontoMarka>()))
                {
                    if (im.IskontoMarkaId == 0)
                    {
                        var _im = new IskontoMarka();
                        _im.AktifMi = im.AktifMi;
                        _im.IskontoId = iskontoUrun.IskontoId;
                        _im.MarkaId = im.MarkaId;

                        IskontoMarkaService.Add(_im);
                    }
                    else
                    {
                        var _im = IskontoMarkaService.FindBy(x => x.IskontoMarkaId == im.IskontoMarkaId).SingleOrDefault();

                        if (_im != null)
                        {
                            _im.AktifMi = im.AktifMi;
                            _im.IskontoId = iskontoUrun.IskontoId;
                            _im.MarkaId = im.MarkaId;

                            IskontoMarkaService.Edit(_im);
                        }
                    }
                    IskontoMarkaService.Save();
                }

                return Json(true, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult IskontoSepetKaydetGuncelle(IskontoSepetBilgiDataObj iskontoSepet)
        {
            try
            {
                // İskonto Rol Listesi
                foreach (var ir in (iskontoSepet.IskontoRolListesi != null ? iskontoSepet.IskontoRolListesi : new List<IskontoRol>()))
                {
                    if (ir.IskontoRolId == 0)
                    {
                        var _ir = new IskontoRol();
                        _ir.AktifMi = ir.AktifMi;
                        _ir.IskontoId = iskontoSepet.IskontoId;
                        _ir.RolId = ir.RolId;

                        IskontoRolService.Add(_ir);
                    }
                    else
                    {
                        var _ir = IskontoRolService.FindBy(x => x.IskontoRolId == ir.IskontoRolId).SingleOrDefault();

                        if (_ir != null)
                        {
                            _ir.AktifMi = ir.AktifMi;
                            _ir.IskontoId = iskontoSepet.IskontoId;
                            _ir.RolId = ir.RolId;

                            IskontoRolService.Edit(_ir);
                        }
                    }
                    IskontoRolService.Save();
                }

                // İskonto Şirket Listesi
                foreach (var iss in (iskontoSepet.IskontoSirketListesi != null ? iskontoSepet.IskontoSirketListesi : new List<IskontoSirket>()))
                {
                    if (iss.IskontoSirketId == 0)
                    {
                        var _iss = new IskontoSirket();
                        _iss.AktifMi = iss.AktifMi;
                        _iss.IskontoId = iskontoSepet.IskontoId;
                        _iss.SirketId = iss.SirketId;

                        IskontoSirketService.Add(_iss);
                    }
                    else
                    {
                        var _iss = IskontoSirketService.FindBy(x => x.IskontoSirketId == iss.IskontoSirketId).SingleOrDefault();

                        if (_iss != null)
                        {
                            _iss.AktifMi = iss.AktifMi;
                            _iss.IskontoId = iskontoSepet.IskontoId;
                            _iss.SirketId = iss.SirketId;

                            IskontoSirketService.Edit(_iss);
                        }
                    }
                    IskontoSirketService.Save();
                }

                return Json(true, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult KullanimHareketAra(int iskontoId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = IskontoKullanimHareketService.FindBy(x =>
            x.IskontoId == iskontoId &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (x.Tarih >= baslangicTarihi && x.Tarih <= bitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (x.Tarih >= baslangicTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (x.Tarih <= bitisTarihi)))
            ), true, new string[] { "Siparis", "Siparis.Kullanici" });

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.IskontoKullanimId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    IskontoKullanimId = x.IskontoKullanimId,
                    SiparisId = x.SiparisId,
                    Tarih = x.Tarih.ToString("dd.MM.yyyy HH:mm"),
                    SiparisToplam = x.Siparis.OdenecekTutar,
                    SiparisIskonto = x.Siparis.ToplamIskonto,
                    KullaniciEposta = x.Siparis.Kullanici.Eposta
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}