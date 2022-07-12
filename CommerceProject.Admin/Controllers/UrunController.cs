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
    public class UrunController : BaseController
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
        IUrunService UrunService;
        IUrunKategoriService UrunKategoriService;
        IUrunResimService UrunResimService;
        IUrunYorumService UrunYorumService;
        IUrunResimTipService UrunResimTipService;
        IIlgiliUrunService IlgiliUrunService;
        IKategoriService KategoriService;
        IMarkaService MarkaService;
        IVergiService VergiService;
        INitelikGrupService NitelikGrupService;
        INitelikService NitelikService;
        IUrunNitelikService UrunNitelikService;
        IOzellikService OzellikService;
        IUrunOzellikService UrunOzellikService;
        ISiparisService SiparisService;
        ISiparisDetayService SiparisDetayService;
        IIadeTalepService IadeTalepService;
        IIstekListesiService IstekListesiService;
        IStokHareketService StokHareketService;
        public UrunController(IIcerikAyarService iIcerikAyarService,
                              IKullaniciService iKullaniciService, 
                              IUrunService iUrunService,
                              IUrunKategoriService iUrunKategoriService,
                              IUrunResimService iUrunResimService,
                              IUrunYorumService iUrunYorumService,
                              IUrunResimTipService iUrunResimTipService,
                              IIlgiliUrunService iIlgiliUrunService,
                              IKategoriService iKategoriService, 
                              IMarkaService iMarkaService,
                              IVergiService iVergiService,
                              INitelikGrupService iNitelikGrupService,
                              INitelikService iNitelikService,
                              IUrunNitelikService iUrunNitelikService,
                              IOzellikService iOzellikService,
                              IUrunOzellikService iUrunOzellikService,
                              ISiparisService iSiparisService,
                              ISiparisDetayService iSiparisDetayService,
                              IIadeTalepService iIadeTalepService,
                              IIstekListesiService iIstekListesiService,
                              IStokHareketService iStokHareketService) : base(iIcerikAyarService,
                                                                      iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            UrunService = iUrunService;
            UrunKategoriService = iUrunKategoriService;
            UrunResimService = iUrunResimService;
            UrunYorumService = iUrunYorumService;
            UrunResimTipService = iUrunResimTipService;
            IlgiliUrunService = iIlgiliUrunService;
            KategoriService = iKategoriService;
            MarkaService = iMarkaService;
            VergiService = iVergiService;
            NitelikGrupService = iNitelikGrupService;
            NitelikService = iNitelikService;
            UrunNitelikService = iUrunNitelikService;
            OzellikService = iOzellikService;
            UrunOzellikService = iUrunOzellikService;
            SiparisService = iSiparisService;
            SiparisDetayService = iSiparisDetayService;
            IadeTalepService = iIadeTalepService;
            IstekListesiService = iIstekListesiService;
            StokHareketService = iStokHareketService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.urun_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Ürün", "", "Ürün İşlemleri", "");

            return View();
        }

        [AuthorizeManager]
        public ActionResult Search()
        {
            ViewBag.PageName = PageHelper.Pages.urun_search;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Ürün", "Arama", "Ürün Arama İşlemleri", "");

            return View();
        }

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.urun_save;
            ViewBag.VergiListesi = VergiService.GetAll(true).ToList().Select(x => new { id = x.VergiId, text = x.Adi + " (" + x.VergiOrani + ")", aktif = x.AktifMi }).ToList();
            ViewBag.ResimTipListesi = UrunResimTipService.GetAll(true).ToList().Select(x => new { id = x.UrunResimTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.NitelikGrupListesi = NitelikGrupService.GetAll(true).ToList().Select(x => new { id = x.NitelikGrupId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.OzellikListesi = OzellikService.FindBy(x => x.OzellikTipId == 1).ToList();

            if (id == null)
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Ürün", "Kayıt", "Ürün Kayıt İşlemleri", "");
                ViewBag.Urun = new Urun();
                ViewBag.UrunKategoriler = new List<UrunKategori>();
                ViewBag.UrunNitelikler = new List<UrunNitelik>();
                ViewBag.UrunResimler = new List<UrunResim>();
                ViewBag.IlgiliUrunler = new List<IlgiliUrun>();
                ViewBag.UrunYorumlar = new List<UrunYorum>();
                ViewBag.UrunSiparisler = new List<Siparis>();
                ViewBag.UrunOzellikler = new List<UrunOzellik>();
            }
            else
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Ürün", "Güncelleme", "Ürün Güncelleme İşlemleri", "");
                ViewBag.Urun = UrunService.FindBy(x => x.UrunId == id, true, new string[] { "Marka" }).SingleOrDefault();
                ViewBag.UrunKategoriler = UrunKategoriService.FindBy(x => x.UrunId == id, true, new string[] { "Kategori" }).ToList();
                ViewBag.UrunNitelikler = UrunNitelikService.FindBy(x => x.UrunId == id, true, new string[] { "Nitelik" }).ToList();
                ViewBag.UrunResimler = UrunResimService.FindBy(x => x.UrunId == id, true, new string[] { "UrunResimTip" }).ToList();
                ViewBag.IlgiliUrunler = IlgiliUrunService.FindBy(x => x.UrunId1 == id, true, new string[] { "Urun1" }).ToList();
                ViewBag.UrunYorumlar = UrunYorumService.FindBy(x => x.UrunId == id, true, new string[] { "Kullanici", "Kullanici.KullaniciDetay" }).ToList();
                ViewBag.UrunSiparisler = SiparisService.GetAll(true, new string[] { "SiparisOdemeTip", "SiparisDurumTip", "OdemeDurumTip", "SiparisDetay" }).ToList().
                    Where(x => x.SiparisDetay.Any(u => u.UrunId == id)).ToList();
                ViewBag.UrunOzellikler = UrunOzellikService.FindBy(x => x.UrunId == id).ToList();
            }

            return View();
        }

        [AuthorizeManager]
        public ActionResult Card(int? id)
        {
            ViewBag.PageName = PageHelper.Pages.urun_card;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Ürün", "Ürün Kart", "Ürün Kartı İşlemleri", "");

            ViewBag.UrunId = id;

            return View();
        }

        public ActionResult ExcelRaporuAl(string adi, string aciklama, string urunKodu, string barkod, int kategoriId, int markaId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int aktiflik)
        {
            var sonucListesi = UrunService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(aciklama) || x.UzunAciklama.Contains(aciklama)) &&
            (string.IsNullOrEmpty(urunKodu) || x.UrunKod == urunKodu) &&
            (string.IsNullOrEmpty(barkod) || x.Barkod == barkod) &&
            (kategoriId == 0 || x.UrunKategori.Any(u => u.KategoriId == kategoriId)) &&
            (markaId == 0 || x.Marka.MarkaId == markaId) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= x.BaslangicTarihi && baslangicTarihi <= x.BitisTarihi && bitisTarihi >= x.BaslangicTarihi && bitisTarihi >= x.BitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= x.BaslangicTarihi && baslangicTarihi <= x.BitisTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= x.BaslangicTarihi && bitisTarihi >= x.BitisTarihi)))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "Marka", "UrunResim", "UrunKategori", "UrunKategori.Kategori" }).ToList().Select(x => new
            {
                UrunKodu = x.UrunKod,
                Barkod = x.Barkod,
                UrunAdi = x.Adi,
                KisaAciklama = x.KisaAciklama,
                Marka = x.Marka != null ? x.Marka.Adi : "",
                Kategori = x.UrunKategori.Count > 0 ? string.Join(", ", x.UrunKategori.Select(u => u.Kategori.Adi)) : "",
                Fiyat = x.Fiyat,
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }

        public ActionResult UrunResimPartial(int id)
        {
            ViewBag.UrunResimler = UrunResimService.FindBy(x => x.UrunId == id).ToList();

            return PartialView("~/Views/Urun/Partials/UrunResimPartial.cshtml");
        }

        public ActionResult UrunDetayPartial(int id)
        {
            ViewBag.Urun = UrunService.FindBy(x => x.UrunId == id, true, new string[] { "Marka", "Vergi", "UrunKategori", "UrunKategori.Kategori", "UrunNitelik", "UrunNitelik.Nitelik", "UrunResim" }).ToList();

            return PartialView("~/Views/Urun/Partials/UrunDetayPartial.cshtml");
        }

        public ActionResult UrunCardPartial(int id)
        {
            ViewBag.Urun = UrunService.FindBy(x => x.UrunId == id, true, new string[] { "Marka", "Vergi", "UrunKategori", "UrunKategori.Kategori", "UrunNitelik", "UrunNitelik.Nitelik", "UrunResim" }).SingleOrDefault();

            var siparisler = SiparisDetayService.FindBy(x => x.UrunId == id && x.AktifMi == true && x.Siparis.AktifMi == true && x.Siparis.SiparisDurumTipId != 8 && x.Siparis.SiparisDurumTipId != 9
            , true, new string[] { "Siparis", "Urun" }).ToList(); //.Sum(x => x.Adet);
            ViewBag.SiparisSayisi = siparisler.Count > 0 ? siparisler.Sum(x => x.Adet) : 0;

            var iadeTalepler = IadeTalepService.FindBy(x => x.SiparisDetay.UrunId == id && x.AktifMi == true && x.SiparisDetay.AktifMi == true
            , true, new string[] { "SiparisDetay" }).ToList();//.Sum(x => x.Adet);
            ViewBag.IadeSayisi = iadeTalepler.Count > 0 ? iadeTalepler.Sum(x => x.Adet) : 0;

            ViewBag.IstekSayisi = IstekListesiService.FindBy(x => x.UrunId == id && x.AktifMi == true).Count();

            var stokGiris = StokHareketService.FindBy(x => x.AktifMi == true && x.UrunId == id && x.StokHareketTipId == 1).Count() > 0 ? 
                StokHareketService.FindBy(x => x.AktifMi == true && x.UrunId == id && x.StokHareketTipId == 1).Sum(x => x.Miktar) : 0;
            var stokCikis = StokHareketService.FindBy(x => x.AktifMi == true && x.UrunId == id && x.StokHareketTipId == 2).Count() > 0 ? 
                StokHareketService.FindBy(x => x.AktifMi == true && x.UrunId == id && x.StokHareketTipId == 2).Sum(x => x.Miktar) : 0;

            ViewBag.StokMiktari = (stokGiris - stokCikis).ToString("#.##");

            return PartialView("~/Views/Urun/Partials/UrunCardPartial.cshtml");
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult Ara(string adi, string aciklama, string urunKodu, string barkod, int kategoriId, int markaId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = UrunService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(aciklama) || x.UzunAciklama.Contains(aciklama)) &&
            (string.IsNullOrEmpty(urunKodu) || x.UrunKod == urunKodu) &&
            (string.IsNullOrEmpty(barkod) || x.Barkod == barkod) &&
            (kategoriId == 0 || x.UrunKategori.Any(u => u.KategoriId == kategoriId)) &&
            (markaId == 0 || x.Marka.MarkaId == markaId) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= x.BaslangicTarihi && baslangicTarihi <= x.BitisTarihi && bitisTarihi >= x.BaslangicTarihi && bitisTarihi >= x.BitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= x.BaslangicTarihi && baslangicTarihi <= x.BitisTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= x.BaslangicTarihi && bitisTarihi >= x.BitisTarihi)))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "Marka", "UrunResim", "UrunKategori", "UrunKategori.Kategori" });

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.UrunId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    UrunId = x.UrunId,
                    UrunAdi = x.Adi,
                    KisaAciklama = x.KisaAciklama,
                    UzunAciklama = x.UzunAciklama,
                    Marka = x.Marka != null ? x.Marka.Adi : "",
                    Kategori = x.UrunKategori.Count > 0 ? string.Join(", ", x.UrunKategori.Select(u => u.Kategori.Adi)) : "",
                    UrunKodu = x.UrunKod,
                    Barkod = x.Barkod,
                    ResimSayisi = x.UrunResim.Count,
                    Fiyat = x.Fiyat,
                    AktifMi = x.AktifMi
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

                var _urun = UrunService.FindBy(x => x.UrunId == id).SingleOrDefault();

                if (_urun == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _urun.AktifMi = false;

                UrunService.Edit(_urun);

                var flag = UrunService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult UrunBilgiKaydetGuncelle(Urun urun)
        {
            try
            {
                Urun _urun;
                var tempUrunNitelikGrupId = 0;

                if(urun.UrunId == 0)
                {
                    _urun = new Urun();

                    _urun.OlusturmaTarihi = DateTime.Now;
                    _urun.GuncellemeTarihi = DateTime.Now;
                }
                else
                {
                    _urun = UrunService.FindBy(x => x.UrunId == urun.UrunId).SingleOrDefault();

                    tempUrunNitelikGrupId = _urun.NitelikGrupId;

                    if (_urun == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _urun.GuncellemeTarihi = DateTime.Now;
                }

                _urun.Adi = urun.Adi;
                _urun.AktifMi = urun.AktifMi;
                _urun.Barkod = urun.Barkod;
                _urun.BaslangicTarihi = urun.BaslangicTarihi;
                _urun.BitisTarihi = urun.BitisTarihi;
                _urun.EskiFiyat = urun.EskiFiyat;
                _urun.Fiyat = urun.Fiyat;
                _urun.KisaAciklama = urun.KisaAciklama;
                _urun.MaksimumSiparisMiktari = urun.MaksimumSiparisMiktari;
                _urun.Maliyet = urun.Maliyet;
                _urun.MarkaId = urun.MarkaId;
                _urun.MetaDescription = urun.MetaDescription;
                _urun.MetaKeywords = urun.MetaKeywords;
                _urun.MetaTitle = urun.MetaTitle;
                _urun.MinimumSiparisMiktari = urun.MinimumSiparisMiktari;
                _urun.NitelikGrupId = urun.NitelikGrupId;
                _urun.OnSiparisAktifMi = urun.OnSiparisAktifMi;
                _urun.OrnekDosyaYolu = urun.OrnekDosyaYolu;
                _urun.OzelFiyatAktifMi = urun.OzelFiyatAktifMi;
                _urun.OzelFiyat = urun.OzelFiyat;
                _urun.OzelFiyatBaslangicTarihi = urun.OzelFiyatBaslangicTarihi;
                _urun.OzelFiyatBitisTarihi = urun.OzelFiyatBitisTarihi;
                _urun.SatinAlAktifMi = urun.SatinAlAktifMi;
                _urun.Sira = urun.Sira;
                _urun.StokAlarmSeviyesi = urun.StokAlarmSeviyesi;
                _urun.StokMiktar = urun.StokMiktar;
                _urun.StokMiktariGosterilsinMi = urun.StokMiktariGosterilsinMi;
                _urun.StoktaVarMi = urun.StoktaVarMi;
                _urun.Tags = urun.Tags;
                _urun.UrunKod = urun.UrunKod;
                _urun.UzunAciklama = urun.UzunAciklama;
                _urun.VergiId = urun.VergiId;
                _urun.YorumYapilabilsinMi = urun.YorumYapilabilsinMi;

                if(urun.UrunId == 0)
                {
                    UrunService.Add(_urun);
                }
                else
                {
                    UrunService.Edit(_urun);
                }

                var flag = UrunService.Save();

                if(!flag)
                    return Json(0, JsonRequestBehavior.DenyGet);

                var urunId = _urun.UrunId;

                // Ürün nitelikleri
                UrunNitelik _urunNitelik;
                if (urun.UrunId > 0)
                {
                    var urunNitelikListesi = UrunNitelikService.FindBy(x => x.UrunId == urun.UrunId).ToList();
                    foreach (var _un in urunNitelikListesi)
                    {
                        UrunNitelikService.Delete(_un);
                    }
                }

                foreach (var un in urun.UrunNitelik)
                {
                    _urunNitelik = new UrunNitelik();
                    _urunNitelik.AktifMi = un.AktifMi;
                    _urunNitelik.NitelikDegeri = un.NitelikDegeri;
                    _urunNitelik.NitelikId = un.NitelikId;
                    _urunNitelik.Sira = un.Sira;
                    _urunNitelik.UrunId = urunId;

                    UrunNitelikService.Add(_urunNitelik);
                }

                UrunNitelikService.Save();

                // Ürün kategorileri
                if (urun.UrunId > 0)
                {
                    UrunKategori _urunKategori;
                    foreach (var kategori in urun.UrunKategori)
                    {
                        if (kategori.UrunKategoriId == 0)
                        {
                            _urunKategori = new UrunKategori();
                            _urunKategori.AktifMi = kategori.AktifMi;
                            _urunKategori.KategoriId = kategori.KategoriId;
                            _urunKategori.UrunId = urun.UrunId;
                            _urunKategori.Sira = kategori.Sira;

                            UrunKategoriService.Add(_urunKategori);
                        }
                        else
                        {
                            _urunKategori = UrunKategoriService.FindBy(x => x.UrunKategoriId == kategori.UrunKategoriId).SingleOrDefault();

                            if (_urunKategori != null)
                            {
                                _urunKategori.AktifMi = kategori.AktifMi;
                                _urunKategori.Sira = kategori.Sira;

                                UrunKategoriService.Edit(_urunKategori);
                            }
                        }
                    }
                    UrunKategoriService.Save();
                }

                // İlgili ürünler
                if(urun.UrunId > 0)
                {
                    IlgiliUrun _ilgiliUrun;
                    foreach (var ilgiliUrun in urun.IlgiliUrun)
                    {
                        if(ilgiliUrun.IlgiliUrunId == 0)
                        {
                            _ilgiliUrun = new IlgiliUrun();
                            _ilgiliUrun.AktifMi = ilgiliUrun.AktifMi;
                            _ilgiliUrun.Sira = ilgiliUrun.Sira;
                            _ilgiliUrun.UrunId1 = urun.UrunId;
                            _ilgiliUrun.UrunId2 = ilgiliUrun.UrunId2;

                            IlgiliUrunService.Add(_ilgiliUrun);
                        }
                        else
                        {
                            _ilgiliUrun = IlgiliUrunService.FindBy(x => x.IlgiliUrunId == ilgiliUrun.IlgiliUrunId).SingleOrDefault();

                            if(_ilgiliUrun != null)
                            {
                                _ilgiliUrun.AktifMi = ilgiliUrun.AktifMi;
                                _ilgiliUrun.Sira = ilgiliUrun.Sira;

                                IlgiliUrunService.Edit(_ilgiliUrun);
                            }
                        }
                    }
                    IlgiliUrunService.Save();
                }

                // Ürün özellikleri
                if(urun.UrunId > 0)
                {
                    var urunOzellikler = UrunOzellikService.FindBy(x => x.UrunId == urun.UrunId).ToList();
                    foreach (var uo in urunOzellikler)
                    {
                        uo.AktifMi = false;

                        UrunOzellikService.Edit(uo);
                    }
                    UrunOzellikService.Save();

                    UrunOzellik _urunOzellik;
                    foreach (var urunOzellik in urun.UrunOzellik)
                    {
                        _urunOzellik = UrunOzellikService.FindBy(x => x.UrunId == urun.UrunId && x.OzellikId == urunOzellik.OzellikId).SingleOrDefault();

                        if(_urunOzellik != null)
                        {
                            _urunOzellik.AktifMi = true;

                            UrunOzellikService.Edit(_urunOzellik);
                        }
                        else
                        {
                            _urunOzellik = new UrunOzellik();
                            _urunOzellik.AktifMi = true;
                            _urunOzellik.OzellikId = urunOzellik.OzellikId;
                            _urunOzellik.UrunId = urun.UrunId;

                            UrunOzellikService.Add(_urunOzellik);
                        }
                    }
                    UrunOzellikService.Save();
                }

                if (flag)
                    return Json(_urun.UrunId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
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

                    var savePath = Path.Combine(Server.MapPath("~/Uploads/Product/"), guid + dosyaUzantisi);
                    dosyaPath = string.Format("/Uploads/Product/{0}",
                                        guid + dosyaUzantisi);

                    Request.Files[0].SaveAs(savePath);

                    // save db
                    var _urunResim = new UrunResim();
                    _urunResim.AktifMi = true;
                    _urunResim.UrunId = key;
                    _urunResim.ResimYolu = dosyaPath;
                    _urunResim.Tarih = DateTime.Now;
                    _urunResim.YeniMi = true;
                    _urunResim.Sira = 1;
                    _urunResim.UrunResimTipId = 1;

                    UrunResimService.Add(_urunResim);
                    var flag = UrunResimService.Save();

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
        public JsonResult ResimKaydetGuncelle(UrunResim urunResim)
        {
            try
            {
                if (urunResim == null || urunResim.UrunResimId == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _urunResim = UrunResimService.FindBy(x => x.UrunResimId == urunResim.UrunResimId).SingleOrDefault();

                if (_urunResim == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _urunResim.Aciklama = urunResim.Aciklama;
                _urunResim.AktifMi = urunResim.AktifMi;
                _urunResim.AltAttribute = urunResim.AltAttribute;
                _urunResim.Tarih = urunResim.Tarih;
                _urunResim.TitleAttribute = urunResim.TitleAttribute;
                _urunResim.YeniMi = urunResim.YeniMi;
                _urunResim.Sira = urunResim.Sira;
                _urunResim.UrunResimTipId = urunResim.UrunResimTipId;

                UrunResimService.Edit(_urunResim);

                var flag = UrunResimService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult UrunYorumGuncelle(int urunYorumId, bool gosterilsinMi, bool aktifMi)
        {
            try
            {
                if (urunYorumId == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _urunYorum = UrunYorumService.FindBy(x => x.UrunYorumId == urunYorumId).SingleOrDefault();

                if (_urunYorum == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _urunYorum.GosterilsinMi = gosterilsinMi;
                _urunYorum.AktifMi = aktifMi;

                UrunYorumService.Edit(_urunYorum);

                var flag = UrunYorumService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult OrnekDosyaYoluKaydet(int urunId, string dosyaYolu)
        {
            if(urunId == 0)
                return Json(0, JsonRequestBehavior.DenyGet);

            try
            {
                // Dosya kaydet
                string dosyaPath = null;
                if (!string.IsNullOrEmpty(dosyaYolu))
                {
                    dosyaPath = dosyaYolu;
                }
                else
                {
                    if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                    {
                        var guid = Guid.NewGuid();

                        var dosyaUzantisi = new FileInfo(Request.Files[0].FileName).Extension;

                        var savePath = Path.Combine(Server.MapPath("~/Uploads/Product/"), guid + dosyaUzantisi);
                        dosyaPath = string.Format("/Uploads/Product/{0}",
                                            guid + dosyaUzantisi);

                        Request.Files[0].SaveAs(savePath);
                    }
                }

                var _urun = UrunService.FindBy(x => x.UrunId == urunId).SingleOrDefault();

                if(_urun == null)
                    return Json(0, JsonRequestBehavior.DenyGet);

                _urun.OrnekDosyaYolu = dosyaPath;

                UrunService.Edit(_urun);

                var flag = UrunService.Save();

                if (flag)
                    return Json(_urun.UrunId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(0, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult UrunAra(string term)
        {
            return Json(
                UrunService.FindBy(x => 
                (x.Adi.ToLower().StartsWith(term.ToLower().Trim())) ||
                (x.UrunKod.ToLower().StartsWith(term.ToLower().Trim())) ||
                (x.Barkod.ToLower().StartsWith(term.ToLower().Trim()))
                ).Select(x => new { id = x.UrunId, text = x.Adi, kod = x.UrunKod, barkod = x.Barkod }).ToList(), 
                JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}