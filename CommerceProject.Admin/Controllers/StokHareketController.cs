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

namespace CommerceProject.Admin.Controllers
{
    public class StokHareketController : BaseController
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
        IStokHareketService StokHareketService;
        IStokHareketTipService StokHareketTipService;
        IUrunService UrunService;
        IKategoriService KategoriService;
        IOlcuService OlcuService;
        ISiparisService SiparisService;
        ISiparisDurumTipService SiparisDurumTipService;
        IIadeTalepService IadeTalepService;
        IIadeTalepDurumTipService IadeTalepDurumTipService;
        public StokHareketController(IIcerikAyarService iIcerikAyarService,
                                     IKullaniciService iKullaniciService,
                                     IStokHareketService iStokHareketService,
                                     IStokHareketTipService iStokHareketTipService,
                                     IUrunService iUrunService,
                                     IKategoriService iKategoriService,
                                     IOlcuService iOlcuService,
                                     ISiparisService iSiparisService,
                                     ISiparisDurumTipService iSiparisDurumTipService,
                                     IIadeTalepService iIadeTalepService,
                                     IIadeTalepDurumTipService iIadeTalepDurumTipService) : base(iIcerikAyarService,
                                                                                                 iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            StokHareketService = iStokHareketService;
            StokHareketTipService = iStokHareketTipService;
            UrunService = iUrunService;
            KategoriService = iKategoriService;
            OlcuService = iOlcuService;
            SiparisService = iSiparisService;
            SiparisDurumTipService = iSiparisDurumTipService;
            IadeTalepService = iIadeTalepService;
            IadeTalepDurumTipService = iIadeTalepDurumTipService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.stokhareket_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Stok Hareket", "", "Stok Hareket İşlemleri", "");

            return View();
        }

        [AuthorizeManager]
        public ActionResult Search()
        {
            ViewBag.PageName = PageHelper.Pages.stokhareket_search;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Stok Hareket", "Arama", "Stok Hareket Arama İşlemleri", "");
            ViewBag.StokHareketTipListesi = StokHareketTipService.GetAll(true).ToList().Select(x => new { id = x.StokHareketTipId, text = x.Adi, aktif = x.AktifMi }).ToList();

            return View();
        }
       
        [AuthorizeManager]
        public ActionResult SaveStockIn()
        {
            ViewBag.PageName = PageHelper.Pages.stokhareket_savestockin;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Stok Hareket", "Giriş Kayıt", "Stok Girişi Kayıt İşlemleri", "");
            ViewBag.KullaniciListesi = KullaniciService.GetAll(true, new string[] { "KullaniciDetay" }).ToList().Select(x => new { id = x.KullaniciId, text = x.KullaniciDetay.Ad + " " + x.KullaniciDetay.Soyad, aktif = x.AktifMi }).ToList();
            ViewBag.OlcuListesi = OlcuService.GetAll(true).Where(x => x.OlcuTipId == 1).ToList().Select(x => new { id = x.OlcuId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.IadeTalepDurumTipListesi = IadeTalepDurumTipService.GetAll(true).ToList().Select(x => new { id = x.IadeTalepDurumTipId, text = x.Adi, aktif = x.AktifMi }).ToList();

            return View();
        }

        [AuthorizeManager]
        public ActionResult SaveStockOut()
        {
            ViewBag.PageName = PageHelper.Pages.stokhareket_savestockout;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Stok Hareket", "Çıkış Kayıt", "Stok Çıkışı Kayıt İşlemleri", "");
            ViewBag.KullaniciListesi = KullaniciService.GetAll(true, new string[] { "KullaniciDetay" }).ToList().Select(x => new { id = x.KullaniciId, text = x.KullaniciDetay.Ad + " " + x.KullaniciDetay.Soyad, aktif = x.AktifMi }).ToList();
            ViewBag.OlcuListesi = OlcuService.GetAll(true).Where(x => x.OlcuTipId == 1).ToList().Select(x => new { id = x.OlcuId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.SiparisDurumTipListesi = SiparisDurumTipService.GetAll(true).ToList().Select(x => new { id = x.SiparisDurumTipId, text = x.Adi, aktif = x.AktifMi }).ToList();

            return View();
        }

        public ActionResult ExcelRaporuAl(string urunKodBarkod, int urunId, int kategoriId, int stokHareketTipId, int siparisNo, DateTime? baslangicTarihi, DateTime? bitisTarihi, int aktiflik)
        {
            var kullanicilar = KullaniciService.GetAll(true, new string[] { "KullaniciDetay" }).ToList();

            var sonucListesi = StokHareketService.GetAll(true, new string[] { "Urun", "StokHareketTip", "Urun.UrunKategori", "Urun.UrunKategori.Kategori" }).ToList().Where(x =>
            (string.IsNullOrEmpty(urunKodBarkod) ||
                ((!(string.IsNullOrEmpty(x.Urun.UrunKod)) ? x.Urun.UrunKod.ToLower() == urunKodBarkod.ToLower() : false) || (!(string.IsNullOrEmpty(x.Urun.Barkod)) ? x.Urun.Barkod.ToLower() == urunKodBarkod.ToLower() : false))
            ) &&
            (urunId == 0 || x.UrunId == urunId) &&
            (kategoriId == 0 || x.Urun.UrunKategori.Any(u => u.KategoriId == kategoriId)) &&
            (stokHareketTipId == 0 || x.StokHareketTipId == stokHareketTipId) &&
            (siparisNo == 0 || x.SiparisId == siparisNo) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (x.KayitTarih >= baslangicTarihi && x.KayitTarih <= bitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (x.KayitTarih >= baslangicTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (x.KayitTarih <= bitisTarihi)))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList().Select(x => new
            {
                StokHareketTipi = x.StokHareketTip.Adi,
                KayitTarih = x.KayitTarih.ToString("dd.MM.yyyy HH:mm"),
                Urun = x.Urun.Adi,
                Kategori = x.Urun.UrunKategori.Count > 0 ? string.Join(", ", x.Urun.UrunKategori.Select(u => u.Kategori.Adi)) : "",
                Miktar = x.Miktar,
                OlcuTipi = "Üründe Ölçü Tipi Yok!",
                BirimFiyat = x.Fiyat,
                Aciklama = x.Aciklama,
                Personel = kullanicilar.Any(u => u.KullaniciId == x.KayitKullaniciKey) ? (kullanicilar.First(u => u.KullaniciId == x.KayitKullaniciKey).KullaniciDetay.Ad + " " + kullanicilar.First(u => u.KullaniciId == x.KayitKullaniciKey).KullaniciDetay.Soyad) : "-",
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult Ara(string urunKodBarkod, int urunId, int kategoriId, int stokHareketTipId, int siparisNo, DateTime? baslangicTarihi, DateTime? bitisTarihi, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var kullanicilar = KullaniciService.GetAll(true, new string[] { "KullaniciDetay" }).ToList();

            var tempList = StokHareketService.GetAll(true, new string[] { "Urun", "StokHareketTip", "Urun.UrunKategori", "Urun.UrunKategori.Kategori" }).ToList().Where(x =>
            (string.IsNullOrEmpty(urunKodBarkod) ||
                ((!(string.IsNullOrEmpty(x.Urun.UrunKod)) ? x.Urun.UrunKod.ToLower() == urunKodBarkod.ToLower() : false) || (!(string.IsNullOrEmpty(x.Urun.Barkod)) ? x.Urun.Barkod.ToLower() == urunKodBarkod.ToLower() : false))
            ) &&
            (urunId == 0 || x.UrunId == urunId) &&
            (kategoriId == 0 || x.Urun.UrunKategori.Any(u => u.KategoriId == kategoriId)) &&
            (stokHareketTipId == 0 || x.StokHareketTipId == stokHareketTipId) &&
            (siparisNo == 0 || x.SiparisId == siparisNo) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (x.KayitTarih >= baslangicTarihi && x.KayitTarih <= bitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (x.KayitTarih >= baslangicTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (x.KayitTarih <= bitisTarihi)))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.StokHareketId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    StokHareketId = x.StokHareketId,
                    StokHareketTip = x.StokHareketTip.Adi,
                    KayitTarih = x.KayitTarih.ToString("dd.MM.yyyy HH:mm"),
                    Urun = x.Urun.Adi,
                    Kategori = x.Urun.UrunKategori.Count > 0 ? string.Join(", ", x.Urun.UrunKategori.Select(u => u.Kategori.Adi)) : "",
                    Miktar = x.Miktar,
                    OlcuTip = "Üründe Ölçü Tipi Yok!",
                    BirimFiyat = x.Fiyat,
                    Aciklama = x.Aciklama,
                    Personel = kullanicilar.Any(u => u.KullaniciId == x.KayitKullaniciKey) ? (kullanicilar.First(u => u.KullaniciId == x.KayitKullaniciKey).KullaniciDetay.Ad + " " + kullanicilar.First(u => u.KullaniciId == x.KayitKullaniciKey).KullaniciDetay.Soyad) : "-",
                    AktifMi = x.AktifMi
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult StokHareketKaydet(List<StokHareket> stokHareketListesi)
        {
            try
            {
                StokHareket _stokHareket;
                foreach (var stokHareket in stokHareketListesi)
                {
                    _stokHareket = new StokHareket();
                    _stokHareket.Aciklama = stokHareket.Aciklama;
                    _stokHareket.AktifMi = stokHareket.AktifMi;
                    _stokHareket.Fiyat = stokHareket.Fiyat;
                    _stokHareket.IadeTalepId = stokHareket.IadeTalepId;
                    _stokHareket.KayitKullaniciKey = stokHareket.KayitKullaniciKey;
                    _stokHareket.KayitTarih = DateTime.Now;
                    _stokHareket.Miktar = stokHareket.Miktar;
                    _stokHareket.OlcuId = stokHareket.OlcuId;
                    _stokHareket.SiparisId = stokHareket.SiparisId;
                    _stokHareket.StokHareketTipId = stokHareket.StokHareketTipId;
                    _stokHareket.UrunId = stokHareket.UrunId;

                    StokHareketService.Add(_stokHareket);
                }

                var flag = StokHareketService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
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

                var _stokHareket = StokHareketService.FindBy(x => x.StokHareketId == id).SingleOrDefault();

                if (_stokHareket == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _stokHareket.AktifMi = false;

                StokHareketService.Edit(_stokHareket);

                var flag = StokHareketService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult SiparisGetir(int siparisDurumTipId)
        {
            if (siparisDurumTipId == 0)
                return Json(null, JsonRequestBehavior.AllowGet);

            var siparisler = SiparisService.FindBy(x => x.SiparisDurumTipId == siparisDurumTipId && x.AktifMi == true).ToList().Select(x => new
            {
                id = x.SiparisId,
                text = "#" + x.SiparisId,
                aktif = x.AktifMi
            }).ToList();

            return Json(siparisler, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IadeTalepGetir(int iadeTalepDurumTipId)
        {
            if (iadeTalepDurumTipId == 0)
                return Json(null, JsonRequestBehavior.AllowGet);

            var iadeTalepler = IadeTalepService.FindBy(x => x.IadeTalepDurumTipId == iadeTalepDurumTipId && x.AktifMi == true).ToList().Select(x => new
            {
                id = x.IadeTalepId,
                text = "#" + x.IadeTalepId,
                aktif = x.AktifMi
            }).ToList();

            return Json(iadeTalepler, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}