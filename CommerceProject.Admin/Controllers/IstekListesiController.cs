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
    public class IstekListesiController : BaseController
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
        IIstekListesiService IstekListesiService;
        IUrunService UrunService;
        IKategoriService KategoriService;
        IUrunKategoriService UrunKategoriService;
        public IstekListesiController(IIcerikAyarService iIcerikAyarService,
                                      IKullaniciService iKullaniciService,
                                      IIstekListesiService iIstekListesiService,
                                      IUrunService iUrunService,
                                      IKategoriService iKategoriService,
                                      IUrunKategoriService iUrunKategoriService) : base(iIcerikAyarService,
                                                                                        iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            IstekListesiService = iIstekListesiService;
            UrunService = iUrunService;
            KategoriService = iKategoriService;
            UrunKategoriService = iUrunKategoriService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.isteklistesi_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("İstek Listesi", "Arama", "İstek Listesi Arama İşlemleri", "");

            return View();
        }      

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.isteklistesi_save;

            if (id == null)
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("İstek Listesi", "Kayıt", "İstek Listesi Kayıt İşlemleri", "");
            else
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("İstek Listesi", "Güncelleme", "İstek Listesi Güncelleme İşlemleri", "");

            return View();
        }

        public ActionResult ExcelRaporuAl(string barkodNo, int kategoriId, int urunId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int aktiflik)
        {
            var istekListesi = IstekListesiService.GetAll(true).ToList();
            var urunListesi = UrunService.GetAll(true).ToList();
            var kategoriListesi = KategoriService.GetAll(true).ToList();
            var urunKategoriListesi = UrunKategoriService.GetAll(true).ToList();

            var sonucListesi = (from istek in istekListesi
                                join urun in urunListesi on istek.UrunId equals urun.UrunId
                                join uk in urunKategoriListesi on urun.UrunId equals uk.UrunId
                                join k in kategoriListesi on uk.KategoriId equals k.KategoriId
                                where
                                (
                                (string.IsNullOrEmpty(barkodNo) || urun.UrunKod == barkodNo) &&
                                (kategoriId == 0 || k.KategoriId == kategoriId) &&
                                (urunId == 0 || istek.UrunId == urunId) &&
                                (
                                 (baslangicTarihi == null && bitisTarihi == null) ||
                                 (((baslangicTarihi != null && bitisTarihi != null) && (istek.Tarih >= baslangicTarihi && istek.Tarih <= bitisTarihi))) ||
                                 (((baslangicTarihi != null && bitisTarihi == null) && (istek.Tarih >= baslangicTarihi))) ||
                                 (((baslangicTarihi == null && bitisTarihi != null) && (istek.Tarih <= bitisTarihi)))
                                ) &&
                                (aktiflik == 2 || ((aktiflik == 0 && istek.AktifMi == false) || (aktiflik == 1 && istek.AktifMi == true)))
                                )
                                select istek).ToList().GroupBy(x => x.UrunId).ToList()
                                .Select(x => new
                                {
                                    UrunAdi = urunListesi.Any(u => u.UrunId == x.Key) ? urunListesi.Single(u => u.UrunId == x.Key).Adi : "?",
                                    KategoriAdiListesi = (from uk in urunKategoriListesi
                                                          join k in kategoriListesi on uk.KategoriId equals k.KategoriId
                                                          where uk.UrunId == x.Key
                                                          select new { KategoriAdi = k.Adi }).ToList(),
                                    IstekSayisi = istekListesi.Where(u => u.UrunId == x.Key &&
                                    (
                                        (baslangicTarihi == null && bitisTarihi == null) ||
                                        (((baslangicTarihi != null && bitisTarihi != null) && (u.Tarih >= baslangicTarihi && u.Tarih <= bitisTarihi))) ||
                                        (((baslangicTarihi != null && bitisTarihi == null) && (u.Tarih >= baslangicTarihi))) ||
                                        (((baslangicTarihi == null && bitisTarihi != null) && (u.Tarih <= bitisTarihi)))
                                    )).Count()
                                }).ToList();

            var list = new List<ExcelDto>();
            var str = "";
            foreach (var s in sonucListesi)
            {
                for (int i = 0; i < s.KategoriAdiListesi.Count; i++)
                {
                    if (i != s.KategoriAdiListesi.Count - 1)
                    {
                        str += s.KategoriAdiListesi[i].KategoriAdi + ", ";
                    }
                    else
                    {
                        str += s.KategoriAdiListesi[i].KategoriAdi;
                    }
                }
                list.Add(new ExcelDto() { UrunAdi = s.UrunAdi, Kategori = str, IstekSayisi = s.IstekSayisi });
            }

            var dt = new PageHelper().ToDataTable(list);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult Ara(string barkodNo, int kategoriId, int urunId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var istekListesi = IstekListesiService.GetAll(true).ToList();
            var urunListesi = UrunService.GetAll(true).ToList();
            var kategoriListesi = KategoriService.GetAll(true).ToList();
            var urunKategoriListesi = UrunKategoriService.GetAll(true).ToList();

            var sonucListesi = (from istek in istekListesi
                                join urun in urunListesi on istek.UrunId equals urun.UrunId
                                join uk in urunKategoriListesi on urun.UrunId equals uk.UrunId
                                join k in kategoriListesi on uk.KategoriId equals k.KategoriId
                                where
                                (
                                (string.IsNullOrEmpty(barkodNo) || urun.UrunKod == barkodNo) &&
                                (kategoriId == 0 || k.KategoriId == kategoriId) &&
                                (urunId == 0 || istek.UrunId == urunId) &&
                                (
                                 (baslangicTarihi == null && bitisTarihi == null) ||
                                 (((baslangicTarihi != null && bitisTarihi != null) && (istek.Tarih >= baslangicTarihi && istek.Tarih <= bitisTarihi))) ||
                                 (((baslangicTarihi != null && bitisTarihi == null) && (istek.Tarih >= baslangicTarihi))) ||
                                 (((baslangicTarihi == null && bitisTarihi != null) && (istek.Tarih <= bitisTarihi)))
                                ) &&
                                (aktiflik == 2 || ((aktiflik == 0 && istek.AktifMi == false) || (aktiflik == 1 && istek.AktifMi == true)))
                                )
                                select istek).ToList();

            var sonuclar = sonucListesi.GroupBy(x => x.UrunId).ToList();
            var tempSonuclar = sonuclar.Select(x => new
            {
                UrunId = x.Key,
                UrunAdi = urunListesi.Any(u => u.UrunId == x.Key) ? urunListesi.Single(u => u.UrunId == x.Key).Adi : "?",
                IstekSayisi = istekListesi.Where(u => u.UrunId == x.Key &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    (((baslangicTarihi != null && bitisTarihi != null) && (u.Tarih >= baslangicTarihi && u.Tarih <= bitisTarihi))) ||
                    (((baslangicTarihi != null && bitisTarihi == null) && (u.Tarih >= baslangicTarihi))) ||
                    (((baslangicTarihi == null && bitisTarihi != null) && (u.Tarih <= bitisTarihi)))
                )).Count(),
                KullaniciListesi = sonucListesi.Where(u => u.UrunId == x.Key).ToList().Select(u => new { KullaniciId = u.KullaniciId }).Distinct().ToList(),
                KategoriAdiListesi = (from uk in urunKategoriListesi
                                      join k in kategoriListesi on uk.KategoriId equals k.KategoriId
                                      where uk.UrunId == x.Key
                                      select new { KategoriAdi = k.Adi }).ToList(),
            }).ToList();

            var count = tempSonuclar.Count;

            tempSonuclar = tempSonuclar.Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = tempSonuclar,
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult KullanicilariGetir(List<Guid> kullanicilar)
        {
            if (kullanicilar == null)
                return Json(false, JsonRequestBehavior.AllowGet);

            var kullaniciListesi = new List<Kullanici>();

            foreach (var id in kullanicilar)
            {
                var kul = KullaniciService.FindBy(x => x.KullaniciId == id, true, new string[] { "KullaniciDetay" }).SingleOrDefault();

                if (kul != null)
                    kullaniciListesi.Add(kul);
            }

            return Json(new
            {
                kullanicilar = kullaniciListesi.Select(x => new
                {
                    Eposta = x.Eposta,
                    UyelikTarihi = x.UyelikTarihi.ToString("dd.MM.yyyy HH:mm"),
                    SonGirisTarihi = x.SonGirisTarihi.ToString("dd.MM.yyyy HH:mm"),
                    Ad = x.KullaniciDetay.Ad,
                    Soyad = x.KullaniciDetay.Soyad,
                    AktifMi = x.AktifMi
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }

    public class ExcelDto
    {
        public string UrunAdi { get; set; }
        public string Kategori { get; set; }
        public int IstekSayisi { get; set; }
    }
}