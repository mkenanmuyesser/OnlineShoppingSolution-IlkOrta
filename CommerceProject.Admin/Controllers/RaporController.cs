using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Admin.Helper;
using CommerceProject.Business.BusinessContracts;
using static CommerceProject.Admin.Helper.PageHelper;
using System.Data;
using CommerceProject.Business.Entities;
using System.Data.Entity;
using CommerceProject.Admin.Models;

namespace CommerceProject.Admin.Controllers
{
    public class RaporController : BaseController
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
        ISepetService SepetService;
        ISiparisService SiparisService;
        ISiparisDetayService SiparisDetayService;
        ISiparisGonderimService SiparisGonderimService;
        IStokHareketService StokHareketService;
        IGonderimService GonderimService;
        ITeslimZamaniService TeslimZamaniService;
        IIstekListesiService IstekListesiService;
        public RaporController(IIcerikAyarService iIcerikAyarService,
                               IKullaniciService iKullaniciService,
                               IUrunService iUrunService,
                               ISepetService iSepetService,
                               ISiparisService iSiparisService,
                               ISiparisDetayService iSiparisDetayService,
                               ISiparisGonderimService iSiparisGonderimService,
                               IStokHareketService iStokHareketService,
                               IGonderimService iGonderimService,
                               ITeslimZamaniService iTeslimZamaniService,
                               IIstekListesiService iIstekListesiService) : base(iIcerikAyarService,
                                                                           iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;
            UrunService = iUrunService;
            SepetService = iSepetService;
            SiparisService = iSiparisService;
            SiparisDetayService = iSiparisDetayService;
            SiparisGonderimService = iSiparisGonderimService;
            StokHareketService = iStokHareketService;
            GonderimService = iGonderimService;
            TeslimZamaniService = iTeslimZamaniService;
            IstekListesiService = iIstekListesiService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.rapor_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Rapor", "", "Raporlar", "");

            return View();
        }

        #region RaporUrunStokAnaliz
        [AuthorizeManager]
        public ActionResult RaporUrunStokAnaliz()
        {
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Rapor", "", "Ürün Stok Hareket Analizi", "");

            var yillar = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                yillar.Add(DateTime.Now.AddYears(-1 * i).Year);
            }

            ViewBag.Yillar = yillar.Select(x => new { id = x, text = x }).ToList();

            return View();
        }

        public ActionResult RaporUrunStokAnaliz_ChartPartial(int tarihYil, int urunId)
        {
            var analizler = new List<RaporUrunStokHareketAnalizDataObj>();

            var stokHareketler = StokHareketService.FindBy(x => x.AktifMi == true &&
            (urunId == 0 || x.UrunId == urunId) &&
            (x.KayitTarih.Year == tarihYil)).ToList();

            var stokGirisArray = stokHareketler.Where(x => x.StokHareketTipId == 1).ToList().Select(x => new
            {
                Miktar = x.Miktar,
                IslemTarihi = new DateTime(x.KayitTarih.Year, x.KayitTarih.Month, 1)
            }).ToList().GroupBy(x => x.IslemTarihi).Select(x => new
            {
                IslemTarihi = x.Key,
                ToplamMiktar = x.Sum(u => u.Miktar)
            }).ToList();

            var stokCikisArray = stokHareketler.Where(x => x.StokHareketTipId == 2).ToList().Select(x => new
            {
                Miktar = x.Miktar,
                IslemTarihi = new DateTime(x.KayitTarih.Year, x.KayitTarih.Month, 1)
            }).ToList().GroupBy(x => x.IslemTarihi).Select(x => new
            {
                IslemTarihi = x.Key,
                ToplamMiktar = x.Sum(u => u.Miktar)
            }).ToList();

            for (int ay = 1; ay < 13; ay++)
            {
                var analiz = new RaporUrunStokHareketAnalizDataObj()
                {
                    Ay = tarihYil + "-" + ay.ToString("D2")
                };

                var sonucGiris = stokGirisArray.SingleOrDefault(x => x.IslemTarihi.Month == ay);
                if (sonucGiris == null)
                    analiz.StokGirisToplamMiktar = 0;
                else
                    analiz.StokGirisToplamMiktar = sonucGiris.ToplamMiktar;

                var sonucCikis = stokCikisArray.SingleOrDefault(x => x.IslemTarihi.Month == ay);
                if (sonucCikis == null)
                    analiz.StokCikisToplamMiktar = 0;
                else
                    analiz.StokCikisToplamMiktar = sonucCikis.ToplamMiktar;

                analizler.Add(analiz);
            }

            ViewBag.ChartData = analizler;

            return PartialView("~/Views/Rapor/Partials/RaporUrunStokAnaliz_ChartPartial.cshtml");
        }
        #endregion

        #region RaporUrunSatis
        [AuthorizeManager]
        public ActionResult RaporUrunSatis()
        {
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Rapor", "", "Ürün Satış Raporu", "");

            return View();
        }

        [AuthorizeManager]
        public ActionResult ExcelRaporUrunSatis(string baslangicTarihiStr, string bitisTarihiStr, int raporTipi)
        {
            DateTime? baslangicTarihi = null;
            DateTime? bitisTarihi = null;

            if (!string.IsNullOrEmpty(baslangicTarihiStr))
            {
                baslangicTarihi = new DateTime(Convert.ToInt32(baslangicTarihiStr.Split('.')[2]), Convert.ToInt32(baslangicTarihiStr.Split('.')[1]), Convert.ToInt32(baslangicTarihiStr.Split('.')[0]));
            }
            if (!string.IsNullOrEmpty(bitisTarihiStr))
            {
                bitisTarihi = new DateTime(Convert.ToInt32(bitisTarihiStr.Split('.')[2]), Convert.ToInt32(bitisTarihiStr.Split('.')[1]), Convert.ToInt32(bitisTarihiStr.Split('.')[0]));
            }

            var urunListesi = UrunService.FindBy(x => x.AktifMi == true, true, new string[] { "Marka", "UrunResim" }).ToList();

            var urunSatisRaporuDictionary = new Dictionary<int, int>();

            // Asc or Desc?
            if (raporTipi == 1)
            {
                urunSatisRaporuDictionary = SiparisDetayService.FindBy(x =>
                (x.AktifMi == true && x.Siparis.AktifMi == true) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi) && bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi)))
                )
                , true, new string[] { "Siparis", "Urun" })
                .GroupBy(x => x.UrunId).Select(x => new { UrunId = x.Key, ToplamSatisAdedi = x.Sum(u => u.Adet) }).ToDictionary(x => x.UrunId, x => x.ToplamSatisAdedi).OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                urunSatisRaporuDictionary = SiparisDetayService.FindBy(x =>
                (x.AktifMi == true && x.Siparis.AktifMi == true) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi) && bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi)))
                )
                , true, new string[] { "Siparis", "Urun" })
                .GroupBy(x => x.UrunId).Select(x => new { UrunId = x.Key, ToplamSatisAdedi = x.Sum(u => u.Adet) }).ToDictionary(x => x.UrunId, x => x.ToplamSatisAdedi).OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }

            var sonucUrunListesi = new List<Urun>();
            Urun urun;
            foreach (var keyValue in urunSatisRaporuDictionary)
            {
                urun = urunListesi.SingleOrDefault(x => x.UrunId == keyValue.Key);

                if (urun != null)
                {
                    sonucUrunListesi.Add(urun);
                }
            }

            var sonucListesi = sonucUrunListesi.Select(x => new
            {
                UrunKodu = x.UrunKod,
                Barkod = x.Barkod,
                UrunAdi = x.Adi,
                Marka = x.Marka != null ? x.Marka.Adi : "",
                Fiyat = x.Fiyat.ToString("##"),
                ToplamSatisAdedi = urunSatisRaporuDictionary.ContainsKey(x.UrunId) ? urunSatisRaporuDictionary[x.UrunId] : 0
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }

        public ActionResult RaporUrunSatis_ListePartial(string baslangicTarihiStr, string bitisTarihiStr, int raporTipi)
        {
            ViewBag.BaslangicTarihi = baslangicTarihiStr;
            ViewBag.BitisTarihi = bitisTarihiStr;
            ViewBag.RaporTipi = raporTipi;

            return PartialView("~/Views/Rapor/Partials/RaporUrunSatis_ListePartial.cshtml");
        }

        public ActionResult RaporUrunSatis_ChartPartial(string baslangicTarihiStr, string bitisTarihiStr, int raporTipi)
        {
            DateTime? baslangicTarihi = null;
            DateTime? bitisTarihi = null;

            if (!string.IsNullOrEmpty(baslangicTarihiStr))
            {
                baslangicTarihi = new DateTime(Convert.ToInt32(baslangicTarihiStr.Split('.')[2]), Convert.ToInt32(baslangicTarihiStr.Split('.')[1]), Convert.ToInt32(baslangicTarihiStr.Split('.')[0]));
            }
            if (!string.IsNullOrEmpty(bitisTarihiStr))
            {
                bitisTarihi = new DateTime(Convert.ToInt32(bitisTarihiStr.Split('.')[2]), Convert.ToInt32(bitisTarihiStr.Split('.')[1]), Convert.ToInt32(bitisTarihiStr.Split('.')[0]));
            }

            var urunListesi = UrunService.FindBy(x => x.AktifMi == true).ToList();

            var urunSatisRaporuDictionary = new Dictionary<int, int>();

            // Asc or Desc?
            if (raporTipi == 1)
            {
                urunSatisRaporuDictionary = SiparisDetayService.FindBy(x =>
                (x.AktifMi == true && x.Siparis.AktifMi == true) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi) && bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi)))
                )
                , true, new string[] { "Siparis", "Urun" })
                .GroupBy(x => x.UrunId)
                .Select(x => new { UrunId = x.Key, ToplamSatisAdedi = x.Sum(u => u.Adet) })
                .OrderByDescending(x => x.ToplamSatisAdedi).Take(20)
                .ToDictionary(x => x.UrunId, x => x.ToplamSatisAdedi);
            }
            else
            {
                urunSatisRaporuDictionary = SiparisDetayService.FindBy(x =>
                (x.AktifMi == true && x.Siparis.AktifMi == true) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi) && bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi)))
                )
                , true, new string[] { "Siparis", "Urun" })
                .GroupBy(x => x.UrunId)
                .Select(x => new { UrunId = x.Key, ToplamSatisAdedi = x.Sum(u => u.Adet) })
                .OrderBy(x => x.ToplamSatisAdedi).Take(20)
                .ToDictionary(x => x.UrunId, x => x.ToplamSatisAdedi);
            }

            var colorList = new List<string>()
            { "#FF0F00", "#FF6600", "#FF9E01", "#FCD202", "#F8FF01", "#B0DE09", "#04D215", "#0D8ECF", "#0D52D1", "#2A0CD0", "#8A0CCF", "#CD0D74", "#754DEB", "#DDDDDD", "#999999", "#333333", "#000000", "#FF0F00", "#FF6600", "#FF9E01" };

            ViewBag.ChartData = urunSatisRaporuDictionary.Select((x, i) => new
            {
                urun = urunListesi.First(u => u.UrunId == x.Key).Adi,
                adet = x.Value,
                color = colorList[i]
            }).ToList();

            return PartialView("~/Views/Rapor/Partials/RaporUrunSatis_ChartPartial.cshtml");
        }
        #endregion

        #region RaporIslemGormeyen
        [AuthorizeManager]
        public ActionResult RaporIslemGormeyen()
        {
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Rapor", "", "İşlem Görmeyenler", "");

            return View();
        }

        [AuthorizeManager]
        public ActionResult ExcelRaporIslemGormeyen(DateTime? baslangicTarihi, DateTime? bitisTarihi, int stoktaAransinMi)
        {
            var urunListesi = UrunService.FindBy(x => x.AktifMi == true, true, new string[] { "Marka" }).ToList();
            var stokHareketListesi = StokHareketService.FindBy(x => x.AktifMi == true).ToList();

            var siparisDetayListesi = SiparisDetayService.FindBy(x => x.AktifMi == true && x.Siparis.AktifMi == true &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= x.Siparis.SiparisTarihi && bitisTarihi >= x.Siparis.SiparisTarihi)) ||
                ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= x.Siparis.SiparisTarihi)) ||
                ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= x.Siparis.SiparisTarihi))
            )
            , true, new string[] { "Siparis", "Urun", "Urun.Marka", "Urun.UrunResim" }).ToList();

            var islemGormeyenUrunListesi = new List<Urun>();
            decimal stokGiris = 0;
            decimal stokCikis = 0;

            foreach (var urun in urunListesi)
            {
                // Stok miktarları
                stokGiris = stokHareketListesi.Where(x => x.UrunId == urun.UrunId && x.StokHareketTipId == 1).Sum(x => x.Miktar);
                stokCikis = stokHareketListesi.Where(x => x.UrunId == urun.UrunId && x.StokHareketTipId == 2).Sum(x => x.Miktar);

                // Stokta Aransın mı??
                if (stoktaAransinMi == 1)
                {
                    if (stokGiris - stokCikis > 0)
                    {
                        if (siparisDetayListesi.Where(x => x.UrunId == urun.UrunId).Count() == 0)
                        {
                            islemGormeyenUrunListesi.Add(urun);
                        }
                    }
                }
                else
                {
                    if (siparisDetayListesi.Where(x => x.UrunId == urun.UrunId).Count() == 0)
                    {
                        islemGormeyenUrunListesi.Add(urun);
                    }
                }
            }

            var sonucListesi = islemGormeyenUrunListesi.Select(x => new
            {
                UrunAdi = x.Adi,
                Marka = x.Marka != null ? x.Marka.Adi : "",
                UrunKodu = x.UrunKod,
                Barkod = x.Barkod,
                Fiyat = x.Fiyat.ToString("##")
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region RaporSepetUrunKullanici
        [AuthorizeManager]
        public ActionResult RaporSepetUrunKullanici()
        {
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Rapor", "", "Sepetinde Ürün Bulunan Kullanıcılar", "");
            ViewBag.KullaniciListesi = KullaniciService.FindBy(x => x.AktifMi == true, true, new string[] { "KullaniciDetay" }).ToList().
                Select(x => new { id = x.KullaniciId, text = x.Eposta, ad = x.KullaniciDetay.Ad, soyad = x.KullaniciDetay.Soyad }).ToList();

            return View();
        }

        [AuthorizeManager]
        public ActionResult ExcelRaporSepetUrunKullanici(int urunId, Guid? kullaniciId)
        {
            var sonucListesi = SepetService.FindBy(x =>
            (x.AktifMi == true && x.SepetTipId == 1) &&
            (urunId == 0 || (x.UrunId == urunId && x.Urun.AktifMi == true)) &&
            (kullaniciId == null || (x.KullaniciId == kullaniciId && x.Kullanici.AktifMi == true)),
            true, new string[] { "Kullanici", "Kullanici.KullaniciDetay", "Urun" }).ToList().Select(x => new
            {
                Kullanici_Eposta = x.Kullanici.Eposta,
                Kullanici_Adi = x.Kullanici.KullaniciDetay.Ad,
                Kullanici_Soyad = x.Kullanici.KullaniciDetay.Soyad,
                Urun_Kodu = x.Urun.UrunKod,
                Urun_Barkod = x.Urun.Barkod,
                Urun_Adi = x.Urun.Adi,
                Urun_Adet = x.Adet,
                Urun_Fiyat = x.Urun.Fiyat.ToString("##")
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region RaporEnCokIstek
        [AuthorizeManager]
        public ActionResult RaporEnCokIstek()
        {
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Rapor", "", "En Çok İstek Listesine Eklenen Ürünler", "");

            return View();
        }

        [AuthorizeManager]
        public ActionResult ExcelRaporEnCokIstek(string baslangicTarihiStr, string bitisTarihiStr, int raporTipi)
        {
            DateTime? baslangicTarihi = null;
            DateTime? bitisTarihi = null;

            if (!string.IsNullOrEmpty(baslangicTarihiStr))
            {
                baslangicTarihi = new DateTime(Convert.ToInt32(baslangicTarihiStr.Split('.')[2]), Convert.ToInt32(baslangicTarihiStr.Split('.')[1]), Convert.ToInt32(baslangicTarihiStr.Split('.')[0]));
            }
            if (!string.IsNullOrEmpty(bitisTarihiStr))
            {
                bitisTarihi = new DateTime(Convert.ToInt32(bitisTarihiStr.Split('.')[2]), Convert.ToInt32(bitisTarihiStr.Split('.')[1]), Convert.ToInt32(bitisTarihiStr.Split('.')[0]));
            }

            var urunListesi = UrunService.FindBy(x => x.AktifMi == true, true, new string[] { "Marka", "UrunResim" }).ToList();

            var enCokIstekRaporuDictionary = new Dictionary<int, int>();

            // Asc or Desc?
            if (raporTipi == 1)
            {
                enCokIstekRaporuDictionary = IstekListesiService.FindBy(x =>
                (x.AktifMi == true && x.Urun.AktifMi == true) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Tarih) && bitisTarihi >= DbFunctions.TruncateTime(x.Tarih))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Tarih))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Tarih)))
                )
                , true, new string[] { "Urun" })
                .GroupBy(x => x.UrunId)
                .Select(x => new { UrunId = x.Key, ToplamIstek = x.Count() }).
                ToDictionary(x => x.UrunId, x => x.ToplamIstek).
                OrderByDescending(x => x.Value).
                ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                enCokIstekRaporuDictionary = IstekListesiService.FindBy(x =>
                (x.AktifMi == true && x.Urun.AktifMi == true) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Tarih) && bitisTarihi >= DbFunctions.TruncateTime(x.Tarih))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Tarih))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Tarih)))
                )
                , true, new string[] { "Urun" })
                .GroupBy(x => x.UrunId)
                .Select(x => new { UrunId = x.Key, ToplamIstek = x.Count() }).
                ToDictionary(x => x.UrunId, x => x.ToplamIstek).
                OrderBy(x => x.Value).
                ToDictionary(x => x.Key, x => x.Value);
            }

            var sonucUrunListesi = new List<Urun>();
            Urun urun;
            foreach (var keyValue in enCokIstekRaporuDictionary)
            {
                urun = urunListesi.SingleOrDefault(x => x.UrunId == keyValue.Key);

                if (urun != null)
                {
                    sonucUrunListesi.Add(urun);
                }
            }

            var sonucListesi = sonucUrunListesi.Select(x => new
            {
                UrunKodu = x.UrunKod,
                Barkod = x.Barkod,
                UrunAdi = x.Adi,
                Marka = x.Marka != null ? x.Marka.Adi : "",
                Fiyat = x.Fiyat.ToString("##"),
                ToplamIstek = enCokIstekRaporuDictionary.ContainsKey(x.UrunId) ? enCokIstekRaporuDictionary[x.UrunId] : 0
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }

        public ActionResult RaporEnCokIstek_ListePartial(string baslangicTarihiStr, string bitisTarihiStr, int raporTipi)
        {
            ViewBag.BaslangicTarihi = baslangicTarihiStr;
            ViewBag.BitisTarihi = bitisTarihiStr;
            ViewBag.RaporTipi = raporTipi;

            return PartialView("~/Views/Rapor/Partials/RaporEnCokIstek_ListePartial.cshtml");
        }

        public ActionResult RaporEnCokIstek_ChartPartial(string baslangicTarihiStr, string bitisTarihiStr, int raporTipi)
        {
            DateTime? baslangicTarihi = null;
            DateTime? bitisTarihi = null;

            if (!string.IsNullOrEmpty(baslangicTarihiStr))
            {
                baslangicTarihi = new DateTime(Convert.ToInt32(baslangicTarihiStr.Split('.')[2]), Convert.ToInt32(baslangicTarihiStr.Split('.')[1]), Convert.ToInt32(baslangicTarihiStr.Split('.')[0]));
            }
            if (!string.IsNullOrEmpty(bitisTarihiStr))
            {
                bitisTarihi = new DateTime(Convert.ToInt32(bitisTarihiStr.Split('.')[2]), Convert.ToInt32(bitisTarihiStr.Split('.')[1]), Convert.ToInt32(bitisTarihiStr.Split('.')[0]));
            }

            var urunListesi = UrunService.FindBy(x => x.AktifMi == true).ToList();

            var enCokIstekRaporuDictionary = new Dictionary<int, int>();

            // Asc or Desc?
            if (raporTipi == 1)
            {
                enCokIstekRaporuDictionary = IstekListesiService.FindBy(x =>
                (x.AktifMi == true && x.Urun.AktifMi == true) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Tarih) && bitisTarihi >= DbFunctions.TruncateTime(x.Tarih))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Tarih))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Tarih)))
                )
                , true, new string[] { "Urun" })
                .GroupBy(x => x.UrunId)
                .Select(x => new { UrunId = x.Key, ToplamIstek = x.Count() })
                .OrderByDescending(x => x.ToplamIstek).Take(20)
                .ToDictionary(x => x.UrunId, x => x.ToplamIstek);
            }
            else
            {
                enCokIstekRaporuDictionary = IstekListesiService.FindBy(x =>
                (x.AktifMi == true && x.Urun.AktifMi == true) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Tarih) && bitisTarihi >= DbFunctions.TruncateTime(x.Tarih))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Tarih))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Tarih)))
                )
                , true, new string[] { "Urun" })
                .GroupBy(x => x.UrunId)
                .Select(x => new { UrunId = x.Key, ToplamIstek = x.Count() })
                .OrderBy(x => x.ToplamIstek).Take(20)
                .ToDictionary(x => x.UrunId, x => x.ToplamIstek);
            }

            var colorList = new List<string>()
            { "#FF0F00", "#FF6600", "#FF9E01", "#FCD202", "#F8FF01", "#B0DE09", "#04D215", "#0D8ECF", "#0D52D1", "#2A0CD0", "#8A0CCF", "#CD0D74", "#754DEB", "#DDDDDD", "#999999", "#333333", "#000000", "#FF0F00", "#FF6600", "#FF9E01" };

            ViewBag.ChartData = enCokIstekRaporuDictionary.Select((x, i) => new
            {
                urun = urunListesi.First(u => u.UrunId == x.Key).Adi,
                adet = x.Value,
                color = colorList[i]
            }).ToList();

            return PartialView("~/Views/Rapor/Partials/RaporEnCokIstek_ChartPartial.cshtml");
        }
        #endregion

        #region RaporAlisverisYapanKullanici
        [AuthorizeManager]
        public ActionResult RaporAlisverisYapanKullanici()
        {
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Rapor", "", "Alışveriş Yapan Kullanıcılar", "");
            ViewBag.KullaniciListesi = KullaniciService.FindBy(x => x.AktifMi == true, true, new string[] { "KullaniciDetay" }).ToList().
                Select(x => new { id = x.KullaniciId, text = x.Eposta, ad = x.KullaniciDetay.Ad, soyad = x.KullaniciDetay.Soyad }).ToList();

            return View();
        }

        [AuthorizeManager]
        public ActionResult ExcelRaporAlisverisYapanKullanici(Guid? kullaniciId, DateTime? baslangicTarihi, DateTime? bitisTarihi)
        {
            var sonucListesi = SiparisDetayService.FindBy(x => x.AktifMi && x.Siparis.AktifMi == true &&
                (kullaniciId == null || x.Siparis.KullaniciId == kullaniciId) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi) && bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi)))
                )
                , true, new string[] { "Siparis", "Siparis.Kullanici", "Siparis.Kullanici.KullaniciDetay", "Urun" }).ToList().Select(x => new
                {
                    SiparisNo = x.SiparisId,
                    Kullanici = x.Siparis.Kullanici.KullaniciDetay.Ad + " " + x.Siparis.Kullanici.KullaniciDetay.Soyad + " (" + x.Siparis.Kullanici.Eposta + ")",
                    UrunId = x.Urun.UrunId,
                    UrunKodu = x.Urun.UrunKod,
                    Barkod = x.Urun.Barkod,
                    UrunAdi = x.Urun.Adi,
                    Adet = x.Adet,
                    HesaplananKDVDahilToplamUcret = x.HesaplananKdvDahilTutar.ToString("##"),
                    SiparisTarihi = x.Siparis.SiparisTarihi.ToString("dd.MM.yyyy HH:mm")
                }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region RaporGonderim
        [AuthorizeManager]
        public ActionResult RaporGonderim()
        {
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Rapor", "", "Gönderim Raporu", "");
            ViewBag.GonderimTipListesi = GonderimService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.GonderimId, text = x.Adi }).ToList();
            ViewBag.TeslimZamaniTipListesi = TeslimZamaniService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.TeslimZamaniId, text = x.Adi }).ToList();

            return View();
        }

        [AuthorizeManager]
        public ActionResult ExcelRaporGonderim(string takipNo, int siparisNo, int urunId, int gonderimTipId, int teslimZamaniTipId, DateTime? siparisBaslangicTarihi, DateTime? siparisBitisTarihi, DateTime? gonderimBaslangicTarihi, DateTime? gonderimBitisTarihi)
        {
            var sonucListesi = SiparisGonderimService.FindBy(x => x.AktifMi == true && x.SiparisDetay.AktifMi == true && x.SiparisDetay.Siparis.AktifMi == true &&
            (string.IsNullOrEmpty(takipNo) || x.TakipNo.Contains(takipNo)) &&
            (siparisNo == 0 || x.SiparisDetay.SiparisId == siparisNo) &&
            (urunId == 0 || x.SiparisDetay.UrunId == urunId) &&
            (gonderimTipId == 0 || x.GonderimId == gonderimTipId) &&
            (teslimZamaniTipId == 0 || x.TeslimZamaniId == teslimZamaniTipId) &&
            (
                (siparisBaslangicTarihi == null && siparisBitisTarihi == null) ||
                ((siparisBaslangicTarihi != null && siparisBitisTarihi != null) && (siparisBaslangicTarihi <= DbFunctions.TruncateTime(x.SiparisDetay.Siparis.SiparisTarihi) && siparisBitisTarihi >= DbFunctions.TruncateTime(x.SiparisDetay.Siparis.SiparisTarihi))) ||
                ((siparisBaslangicTarihi != null && siparisBitisTarihi == null) && (siparisBaslangicTarihi <= DbFunctions.TruncateTime(x.SiparisDetay.Siparis.SiparisTarihi))) ||
                ((siparisBaslangicTarihi == null && siparisBitisTarihi != null) && (siparisBitisTarihi >= DbFunctions.TruncateTime(x.SiparisDetay.Siparis.SiparisTarihi)))
            ) &&
            (
                (gonderimBaslangicTarihi == null && gonderimBitisTarihi == null) ||
                ((gonderimBaslangicTarihi != null && gonderimBitisTarihi != null) && (gonderimBaslangicTarihi <= DbFunctions.TruncateTime(x.GonderimTarihi) && gonderimBitisTarihi >= DbFunctions.TruncateTime(x.GonderimTarihi))) ||
                ((gonderimBaslangicTarihi != null && gonderimBitisTarihi == null) && (gonderimBaslangicTarihi <= DbFunctions.TruncateTime(x.GonderimTarihi))) ||
                ((gonderimBaslangicTarihi == null && gonderimBitisTarihi != null) && (gonderimBitisTarihi >= DbFunctions.TruncateTime(x.GonderimTarihi)))
            )
            , true, new string[] { "SiparisDetay", "Gonderim", "TeslimZamani", "SiparisDetay.Siparis", "SiparisDetay.Urun" }).ToList().Select(x => new
            {
                SiparisNo = x.SiparisDetay.SiparisId,
                TakipNo = x.TakipNo,
                GonderimTipi = x.Gonderim.Adi,
                TeslimZamaniTipi = x.TeslimZamani.Adi,
                UrunKodu = x.SiparisDetay.Urun.UrunKod,
                Barkod = x.SiparisDetay.Urun.Barkod,
                UrunAdi = x.SiparisDetay.Urun.Adi,
                Adet = x.Adet,
                SiparisTarihi = x.SiparisDetay.Siparis.SiparisTarihi.ToString("dd.MM.yyyy HH:mm"),
                GonderimTarihi = x.GonderimTarihi.ToString("dd.MM.yyyy HH:mm"),
                TeslimTarihi = x.TeslimTarihi.HasValue ? x.TeslimTarihi.Value.ToString("dd.MM.yyyy HH:mm") : ""
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        [AuthorizeManager]
        public ActionResult OnlineUsers()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Rapor", "Onlıne Kullanıcılar", "Onlıne Kullanıcılar", "");

            return View();
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult RaporSepetUrunKullaniciAra(int urunId, Guid? kullaniciId, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = SepetService.FindBy(x =>
            (x.AktifMi == true && x.SepetTipId == 1) &&
            (urunId == 0 || (x.UrunId == urunId && x.Urun.AktifMi == true)) &&
            (kullaniciId == null || (x.KullaniciId == kullaniciId && x.Kullanici.AktifMi == true)), 
            true, new string[] { "Kullanici", "Kullanici.KullaniciDetay", "Urun" });

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.SepetId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    Kullanici_Eposta = x.Kullanici.Eposta,
                    Kullanici_Adi = x.Kullanici.KullaniciDetay.Ad,
                    Kullanici_Soyad = x.Kullanici.KullaniciDetay.Soyad,
                    Urun_Id = x.UrunId,
                    Urun_Kodu = x.Urun.UrunKod,
                    Urun_Barkod = x.Urun.Barkod,
                    Urun_Adi = x.Urun.Adi,
                    Urun_Adet = x.Adet,
                    Urun_Fiyat = x.Urun.Fiyat.ToString("##")
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RaporIslemGormeyenAra(DateTime? baslangicTarihi, DateTime? bitisTarihi, int stoktaAransinMi, int sayfaSayisi, int sayfaSirasi)
        {
            var urunListesi = UrunService.FindBy(x => x.AktifMi == true, true, new string[] { "Marka", "UrunResim" }).ToList();
            var stokHareketListesi = StokHareketService.FindBy(x => x.AktifMi == true).ToList();

            var siparisDetayListesi = SiparisDetayService.FindBy(x => x.AktifMi == true && x.Siparis.AktifMi == true &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi) && bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi)))
            )
            , true, new string[] { "Siparis", "Urun" }).ToList();

            var islemGormeyenUrunListesi = new List<Urun>();
            decimal stokGiris = 0;
            decimal stokCikis = 0;

            foreach (var urun in urunListesi)
            {
                // Stok miktarları
                stokGiris = stokHareketListesi.Where(x => x.UrunId == urun.UrunId && x.StokHareketTipId == 1).Sum(x => x.Miktar);
                stokCikis = stokHareketListesi.Where(x => x.UrunId == urun.UrunId && x.StokHareketTipId == 2).Sum(x => x.Miktar);

                // Stokta Aransın mı??
                if (stoktaAransinMi == 1)
                {
                    if(stokGiris - stokCikis > 0)
                    {
                        if (siparisDetayListesi.Where(x => x.UrunId == urun.UrunId).Count() == 0)
                        {
                            islemGormeyenUrunListesi.Add(urun);
                        }
                    }
                }
                else
                {
                    if (siparisDetayListesi.Where(x => x.UrunId == urun.UrunId).Count() == 0)
                    {
                        islemGormeyenUrunListesi.Add(urun);
                    }
                }
            }

            var count = islemGormeyenUrunListesi.Count;

            islemGormeyenUrunListesi = islemGormeyenUrunListesi.Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = islemGormeyenUrunListesi.Select(x => new
                {
                    UrunId = x.UrunId,
                    UrunAdi = x.Adi,
                    Marka = x.Marka != null ? x.Marka.Adi : "",
                    UrunKodu = x.UrunKod,
                    Barkod = x.Barkod,
                    ResimSayisi = x.UrunResim.Count,
                    Fiyat = x.Fiyat.ToString("##")
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RaporUrunSatis_ListeAra(string baslangicTarihiStr, string bitisTarihiStr, int raporTipi, int sayfaSayisi, int sayfaSirasi)
        {
            DateTime? baslangicTarihi = null;
            DateTime? bitisTarihi = null;

            if (!string.IsNullOrEmpty(baslangicTarihiStr))
            {
                baslangicTarihi = new DateTime(Convert.ToInt32(baslangicTarihiStr.Split('.')[2]), Convert.ToInt32(baslangicTarihiStr.Split('.')[1]), Convert.ToInt32(baslangicTarihiStr.Split('.')[0]));
            }
            if (!string.IsNullOrEmpty(bitisTarihiStr))
            {
                bitisTarihi = new DateTime(Convert.ToInt32(bitisTarihiStr.Split('.')[2]), Convert.ToInt32(bitisTarihiStr.Split('.')[1]), Convert.ToInt32(bitisTarihiStr.Split('.')[0]));
            }

            var urunListesi = UrunService.FindBy(x => x.AktifMi == true, true, new string[] { "Marka", "UrunResim" }).ToList();

            var urunSatisRaporuDictionary = new Dictionary<int, int>();

            // Asc or Desc?
            if (raporTipi == 1)
            {
                urunSatisRaporuDictionary = SiparisDetayService.FindBy(x =>
                (x.AktifMi == true && x.Siparis.AktifMi == true) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi) && bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi)))
                )
                , true, new string[] { "Siparis", "Urun" })
                .GroupBy(x => x.UrunId).Select(x => new { UrunId = x.Key, ToplamSatisAdedi = x.Sum(u => u.Adet) }).ToDictionary(x => x.UrunId, x => x.ToplamSatisAdedi).OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                urunSatisRaporuDictionary = SiparisDetayService.FindBy(x =>
                (x.AktifMi == true && x.Siparis.AktifMi == true) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi) && bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi)))
                )
                , true, new string[] { "Siparis", "Urun" })
                .GroupBy(x => x.UrunId).Select(x => new { UrunId = x.Key, ToplamSatisAdedi = x.Sum(u => u.Adet) }).ToDictionary(x => x.UrunId, x => x.ToplamSatisAdedi).OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }

            var count = urunSatisRaporuDictionary.Count;

            var lastDictionary = urunSatisRaporuDictionary.Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            var sonucListesi = new List<object>();
            Urun urun;
            foreach (var keyValue in lastDictionary)
            {
                urun = urunListesi.SingleOrDefault(x => x.UrunId == keyValue.Key);

                if (urun != null)
                {
                    sonucListesi.Add(new
                    {
                        UrunId = urun.UrunId,
                        UrunAdi = urun.Adi,
                        Marka = urun.Marka != null ? urun.Marka.Adi : "",
                        UrunKodu = urun.UrunKod,
                        Barkod = urun.Barkod,
                        ResimSayisi = urun.UrunResim.Count,
                        Fiyat = urun.Fiyat.ToString("##"),
                        ToplamSatisAdedi = keyValue.Value
                    });
                }
            }

            return Json(new
            {
                sonucListesi = sonucListesi,
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RaporGonderimAra(string takipNo, int siparisNo, int urunId, int gonderimTipId, int teslimZamaniTipId, DateTime? siparisBaslangicTarihi, DateTime? siparisBitisTarihi, DateTime? gonderimBaslangicTarihi, DateTime? gonderimBitisTarihi, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = SiparisGonderimService.FindBy(x => x.AktifMi == true && x.SiparisDetay.AktifMi == true && x.SiparisDetay.Siparis.AktifMi == true &&
            (string.IsNullOrEmpty(takipNo) || x.TakipNo.Contains(takipNo)) &&
            (siparisNo == 0 || x.SiparisDetay.SiparisId == siparisNo) &&
            (urunId == 0 || x.SiparisDetay.UrunId == urunId) &&
            (gonderimTipId == 0 || x.GonderimId == gonderimTipId) &&
            (teslimZamaniTipId == 0 || x.TeslimZamaniId == teslimZamaniTipId) &&
            (
                (siparisBaslangicTarihi == null && siparisBitisTarihi == null) ||
                ((siparisBaslangicTarihi != null && siparisBitisTarihi != null) && (siparisBaslangicTarihi <= DbFunctions.TruncateTime(x.SiparisDetay.Siparis.SiparisTarihi) && siparisBitisTarihi >= DbFunctions.TruncateTime(x.SiparisDetay.Siparis.SiparisTarihi))) ||
                ((siparisBaslangicTarihi != null && siparisBitisTarihi == null) && (siparisBaslangicTarihi <= DbFunctions.TruncateTime(x.SiparisDetay .Siparis.SiparisTarihi))) ||
                ((siparisBaslangicTarihi == null && siparisBitisTarihi != null) && (siparisBitisTarihi >= DbFunctions.TruncateTime(x.SiparisDetay.Siparis.SiparisTarihi)))
            ) &&
            (
                (gonderimBaslangicTarihi == null && gonderimBitisTarihi == null) ||
                ((gonderimBaslangicTarihi != null && gonderimBitisTarihi != null) && (gonderimBaslangicTarihi <= DbFunctions.TruncateTime(x.GonderimTarihi) && gonderimBitisTarihi >= DbFunctions.TruncateTime(x.GonderimTarihi))) ||
                ((gonderimBaslangicTarihi != null && gonderimBitisTarihi == null) && (gonderimBaslangicTarihi <= DbFunctions.TruncateTime(x.GonderimTarihi))) ||
                ((gonderimBaslangicTarihi == null && gonderimBitisTarihi != null) && (gonderimBitisTarihi >= DbFunctions.TruncateTime(x.GonderimTarihi)))
            )
            , true, new string[] { "SiparisDetay", "Gonderim", "TeslimZamani", "SiparisDetay.Siparis", "SiparisDetay.Urun" });

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.SiparisGonderimId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).OrderBy(x => x.GonderimTarihi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    SiparisNo = x.SiparisDetay.SiparisId,
                    TakipNo = x.TakipNo,
                    GonderimTipi = x.Gonderim.Adi,
                    TeslimZamaniTipi = x.TeslimZamani.Adi,
                    UrunId = x.SiparisDetay.UrunId,
                    UrunKodu = x.SiparisDetay.Urun.UrunKod,
                    Barkod = x.SiparisDetay.Urun.Barkod,
                    UrunAdi = x.SiparisDetay.Urun.Adi,
                    Adet = x.Adet,
                    SiparisTarihi = x.SiparisDetay.Siparis.SiparisTarihi.ToString("dd.MM.yyyy HH:mm"),
                    GonderimTarihi = x.GonderimTarihi.ToString("dd.MM.yyyy HH:mm"),
                    TeslimTarihi = x.TeslimTarihi.HasValue ? x.TeslimTarihi.Value.ToString("dd.MM.yyyy HH:mm") : ""
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RaporAlisverisYapanKullaniciAra(Guid? kullaniciId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int sayfaSayisi, int sayfaSirasi)
        {
            var sonucListesi = SiparisDetayService.FindBy(x => x.AktifMi && x.Siparis.AktifMi == true &&
                (kullaniciId == null || x.Siparis.KullaniciId == kullaniciId) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi) && bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Siparis.SiparisTarihi)))
                )
                ,true, new string[] { "Siparis", "Siparis.Kullanici", "Siparis.Kullanici.KullaniciDetay", "Urun" }).ToList();

            var count = sonucListesi.Count;

            var siparisToplam = sonucListesi.Sum(u => u.Siparis.OdenecekTutar);
            var siparisIskonto = sonucListesi.Sum(u => u.Siparis.ToplamIskonto);
            var siparisKomisyon = sonucListesi.Sum(u => u.Siparis.ToplamKomisyon);
            var iadeToplam = sonucListesi.Sum(u => u.Siparis.IadeToplam);

            sonucListesi = sonucListesi.Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).OrderBy(x => x.SiparisId).ThenBy(x => x.SiparisDetayId).ToList();

            var siparisOzetToplam = sonucListesi.Sum(u => u.Siparis.OdenecekTutar);
            var siparisOzetIskonto = sonucListesi.Sum(u => u.Siparis.ToplamIskonto);
            var siparisOzetKomisyon = sonucListesi.Sum(u => u.Siparis.ToplamKomisyon);
            var iadeOzetToplam = sonucListesi.Sum(u => u.Siparis.IadeToplam);

            var tempSiparisId = 0;
            var rowNumber = 1;
            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    SiparisNo = x.SiparisId,
                    SiparisDetaySira = x.Siparis.SiparisDetay.Select((u, i) => 
                    {
                        if(tempSiparisId == 0)
                        {
                            tempSiparisId = x.SiparisId;
                            rowNumber = 1;
                        }
                        else
                        {
                            if(tempSiparisId == x.SiparisId)
                            {
                                rowNumber++;
                            }
                            else
                            {
                                tempSiparisId = x.SiparisId;
                                rowNumber = 1;
                            }
                        }
                        return rowNumber.ToString();
                    }),
                    Kullanici = x.Siparis.Kullanici.KullaniciDetay.Ad + " " + x.Siparis.Kullanici.KullaniciDetay.Soyad + " (" + x.Siparis.Kullanici.Eposta + ")",
                    UrunId = x.Urun.UrunId,
                    UrunKodu = x.Urun.UrunKod,
                    Barkod = x.Urun.Barkod,
                    UrunAdi = x.Urun.Adi,
                    Adet = x.Adet,
                    HesaplananKDVDahilToplamUcret = x.HesaplananKdvDahilTutar.ToString("##"),
                    SiparisTarihi = x.Siparis.SiparisTarihi.ToString("dd.MM.yyyy HH:mm")
                }).ToList(),
                siparisOzetToplam = siparisOzetToplam,
                siparisOzetIskonto = siparisOzetIskonto,
                siparisOzetKomisyon = siparisOzetKomisyon,
                iadeOzetToplam = iadeOzetToplam,

                siparisToplam = siparisToplam,
                siparisIskonto = siparisIskonto,
                siparisKomisyon = siparisKomisyon,
                iadeToplam = iadeToplam,

                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RaporEnCokIstek_ListeAra(string baslangicTarihiStr, string bitisTarihiStr, int raporTipi, int sayfaSayisi, int sayfaSirasi)
        {
            DateTime? baslangicTarihi = null;
            DateTime? bitisTarihi = null;

            if (!string.IsNullOrEmpty(baslangicTarihiStr))
            {
                baslangicTarihi = new DateTime(Convert.ToInt32(baslangicTarihiStr.Split('.')[2]), Convert.ToInt32(baslangicTarihiStr.Split('.')[1]), Convert.ToInt32(baslangicTarihiStr.Split('.')[0]));
            }
            if (!string.IsNullOrEmpty(bitisTarihiStr))
            {
                bitisTarihi = new DateTime(Convert.ToInt32(bitisTarihiStr.Split('.')[2]), Convert.ToInt32(bitisTarihiStr.Split('.')[1]), Convert.ToInt32(bitisTarihiStr.Split('.')[0]));
            }

            var urunListesi = UrunService.FindBy(x => x.AktifMi == true, true, new string[] { "Marka", "UrunResim" }).ToList();

            var enCokIstekRaporuDictionary = new Dictionary<int, int>();

            // Asc or Desc?
            if (raporTipi == 1)
            {
                enCokIstekRaporuDictionary = IstekListesiService.FindBy(x =>
                (x.AktifMi == true && x.Urun.AktifMi == true) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Tarih) && bitisTarihi >= DbFunctions.TruncateTime(x.Tarih))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Tarih))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Tarih)))
                )
                , true, new string[] { "Urun" })
                .GroupBy(x => x.UrunId)
                .Select(x => new { UrunId = x.Key, ToplamIstek = x.Count() }).
                ToDictionary(x => x.UrunId, x => x.ToplamIstek).
                OrderByDescending(x => x.Value).
                ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                enCokIstekRaporuDictionary = IstekListesiService.FindBy(x =>
                (x.AktifMi == true && x.Urun.AktifMi == true) &&
                (
                    (baslangicTarihi == null && bitisTarihi == null) ||
                    ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Tarih) && bitisTarihi >= DbFunctions.TruncateTime(x.Tarih))) ||
                    ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= DbFunctions.TruncateTime(x.Tarih))) ||
                    ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= DbFunctions.TruncateTime(x.Tarih)))
                )
                , true, new string[] { "Urun" })
                .GroupBy(x => x.UrunId)
                .Select(x => new { UrunId = x.Key, ToplamIstek = x.Count() }).
                ToDictionary(x => x.UrunId, x => x.ToplamIstek).
                OrderBy(x => x.Value).
                ToDictionary(x => x.Key, x => x.Value);
            }

            var count = enCokIstekRaporuDictionary.Count;

            var lastDictionary = enCokIstekRaporuDictionary.Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            var sonucListesi = new List<object>();
            Urun urun;
            foreach (var keyValue in lastDictionary)
            {
                urun = urunListesi.SingleOrDefault(x => x.UrunId == keyValue.Key);

                if (urun != null)
                {
                    sonucListesi.Add(new
                    {
                        UrunId = urun.UrunId,
                        UrunAdi = urun.Adi,
                        Marka = urun.Marka != null ? urun.Marka.Adi : "",
                        UrunKodu = urun.UrunKod,
                        Barkod = urun.Barkod,
                        ResimSayisi = urun.UrunResim.Count,
                        Fiyat = urun.Fiyat.ToString("##"),
                        ToplamIstek = keyValue.Value
                    });
                }
            }

            return Json(new
            {
                sonucListesi = sonucListesi,
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}