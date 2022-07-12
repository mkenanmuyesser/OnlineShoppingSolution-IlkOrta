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
    public class AnketController : BaseController
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
        IAnketService AnketService;
        IAnketSoruService AnketSoruService;
        IAnketCevapService AnketCevapService;
        public AnketController(IIcerikAyarService iIcerikAyarService,
                               IKullaniciService iKullaniciService,
                               IAnketService iAnketService,
                               IAnketSoruService iAnketSoruService,
                               IAnketCevapService iAnketCevapService) : base(iIcerikAyarService,
                                                                             iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            AnketService = iAnketService;
            AnketSoruService = iAnketSoruService;
            AnketCevapService = iAnketCevapService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.anket_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Anket", "Arama", "Anket Arama İşlemleri", "");

            return View();
        }      

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.anket_save;

            if (id == null)
            {
                ViewBag.Anket = new Anket();
                ViewBag.AnketSorular = new List<AnketSoru>();
                ViewBag.AnketCevaplar = new List<AnketCevap>();

                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Anket", "Kayıt", "Anket Kayıt İşlemleri", "");
            }
            else
            {
                ViewBag.Anket = AnketService.FindBy(x => x.AnketId == id).SingleOrDefault();

                var cevaplar = AnketCevapService.GetAll();
                var kullanicilar = KullaniciService.GetAll();

                var sorular = AnketSoruService.FindBy(x => x.AnketId == id).ToList();

                ViewBag.AnketSorular = sorular.Select(x => new AnketSoru()
                {
                    AktifMi = x.AktifMi,
                    AnketId = x.AnketId,
                    AnketSoruId = x.AnketSoruId,
                    Sira = x.Sira,
                    Soru = x.Soru,
                    AnketCevap = cevaplar.Where(u => u.AnketSoruId == x.AnketSoruId).ToList().Select(u => new AnketCevap()
                    {
                        AnketCevapId = u.AnketCevapId,
                        AnketSoruId = u.AnketSoruId,
                        CevapSonuc = u.CevapSonuc,
                        Kullanici = kullanicilar.Where(z => z.KullaniciId == u.KullaniciId).FirstOrDefault(),
                        KullaniciId = u.KullaniciId,
                        Tarih = u.Tarih
                    }).ToList()
                }).ToList();

                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Anket", "Güncelleme", "Anket Güncelleme İşlemleri", "");
            }

            return View();
        }

        public ActionResult AnketSoruPartial(int id)
        {
            ViewBag.AnketSorular = AnketSoruService.FindBy(x => x.AnketId == id).ToList();

            return PartialView("~/Views/Anket/Partials/AnketSoruPartial.cshtml");
        }

        public ActionResult ExcelRaporuAl(string adi, string aciklama, DateTime? baslangicTarihi, DateTime? bitisTarihi, int ziyaretciCevap, int yayin, int aktiflik)
        {
            var sonucListesi = AnketService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= x.BaslangicTarihi && baslangicTarihi <= x.BitisTarihi && bitisTarihi >= x.BaslangicTarihi && bitisTarihi >= x.BitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= x.BaslangicTarihi && baslangicTarihi <= x.BitisTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= x.BaslangicTarihi && bitisTarihi >= x.BitisTarihi)))
            ) &&
            (ziyaretciCevap == 2 || ((ziyaretciCevap == 0 && x.ZiyaretciCevapAktifMi == false) || (ziyaretciCevap == 1 && x.ZiyaretciCevapAktifMi == true))) &&
            (yayin == 2 || ((yayin == 0 && x.YayindaMi == false) || (yayin == 1 && x.YayindaMi == true))) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList()
            .Select(x => new
            {
                Adi = x.Adi,
                Aciklama = x.Aciklama,
                BaslangicTarihi = x.BaslangicTarihi.ToString("dd.MM.yyyy"),
                BitisTarihi = x.BitisTarihi.ToString("dd.MM.yyyy"),
                YayindaMi = x.YayindaMi ? "Evet" : "Hayır",
                ZiyaretciCevapAktifMi = x.ZiyaretciCevapAktifMi ? "Evet" : "Hayır",
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult KaydetGuncelle(Anket anket)
        {
            try
            {
                Anket _anket;

                if (anket.AnketId == 0)
                {
                    _anket = new Anket();
                    _anket.Aciklama = anket.Aciklama;
                    _anket.Adi = anket.Adi;
                    _anket.AktifMi = anket.AktifMi;
                    _anket.BaslangicTarihi = anket.BaslangicTarihi;
                    _anket.BitisTarihi = anket.BitisTarihi;
                    _anket.Sira = anket.Sira;
                    _anket.YayindaMi = anket.YayindaMi;
                    _anket.ZiyaretciCevapAktifMi = anket.ZiyaretciCevapAktifMi;

                    AnketService.Add(_anket);
                }
                else
                {
                    _anket = AnketService.FindBy(x => x.AnketId == anket.AnketId).SingleOrDefault();

                    if (_anket == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _anket.Aciklama = anket.Aciklama;
                    _anket.Adi = anket.Adi;
                    _anket.AktifMi = anket.AktifMi;
                    _anket.BaslangicTarihi = anket.BaslangicTarihi;
                    _anket.BitisTarihi = anket.BitisTarihi;
                    _anket.Sira = anket.Sira;
                    _anket.YayindaMi = anket.YayindaMi;
                    _anket.ZiyaretciCevapAktifMi = anket.ZiyaretciCevapAktifMi;

                    AnketService.Edit(_anket);
                }

                var flag = AnketService.Save();

                if (flag)
                    return Json(_anket.AnketId, JsonRequestBehavior.DenyGet);
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

                var _anket = AnketService.FindBy(x => x.AnketId == id).SingleOrDefault();

                if (_anket == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _anket.AktifMi = false;

                AnketService.Edit(_anket);

                var flag = AnketService.Save();

                if (!flag)
                    return Json(false, JsonRequestBehavior.DenyGet);

                // Anket soruları AktifMi=pasif
                var anketSorulari = AnketSoruService.FindBy(x => x.AnketId == id && x.AktifMi == true).ToList();

                foreach (var _soru in anketSorulari)
                {
                    _soru.AktifMi = false;

                    AnketSoruService.Edit(_soru);

                    AnketSoruService.Save();
                }

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult Ara(string adi, string aciklama, DateTime? baslangicTarihi, DateTime? bitisTarihi, int ziyaretciCevap, int yayin, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = AnketService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= x.BaslangicTarihi && baslangicTarihi <= x.BitisTarihi && bitisTarihi >= x.BaslangicTarihi && bitisTarihi >= x.BitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= x.BaslangicTarihi && baslangicTarihi <= x.BitisTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= x.BaslangicTarihi && bitisTarihi >= x.BitisTarihi)))
            ) &&
            (ziyaretciCevap == 2 || ((ziyaretciCevap == 0 && x.ZiyaretciCevapAktifMi == false) || (ziyaretciCevap == 1 && x.ZiyaretciCevapAktifMi == true))) &&
            (yayin == 2 || ((yayin == 0 && x.YayindaMi == false) || (yayin == 1 && x.YayindaMi == true))) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            );

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.AnketId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    Aciklama = x.Aciklama,
                    Adi = x.Adi,
                    AktifMi = x.AktifMi,
                    AnketId = x.AnketId,
                    BaslangicTarihi = x.BaslangicTarihi.ToString("dd.MM.yyyy"),
                    BitisTarihi = x.BitisTarihi.ToString("dd.MM.yyyy"),
                    Sira = x.Sira,
                    YayindaMi = x.YayindaMi,
                    ZiyaretciCevapAktifMi = x.ZiyaretciCevapAktifMi
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SoruKaydetGuncelle(List<AnketSoru> anketSorular)
        {
            try
            {
                if (anketSorular.Count == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                AnketSoru _anketSoru;
                foreach (var soru in anketSorular)
                {
                    if (soru.AnketSoruId == 0)
                    {
                        _anketSoru = new AnketSoru();
                        _anketSoru.AktifMi = soru.AktifMi;
                        _anketSoru.AnketId = soru.AnketId;
                        _anketSoru.Sira = soru.Sira;
                        _anketSoru.Soru = soru.Soru;

                        AnketSoruService.Add(_anketSoru);
                    }
                    else
                    {
                        _anketSoru = AnketSoruService.FindBy(x => x.AnketSoruId == soru.AnketSoruId).SingleOrDefault();

                        if (_anketSoru == null)
                            return Json(false, JsonRequestBehavior.DenyGet);

                        _anketSoru.AktifMi = soru.AktifMi;
                        _anketSoru.AnketId = soru.AnketId;
                        _anketSoru.Sira = soru.Sira;
                        _anketSoru.Soru = soru.Soru;

                        AnketSoruService.Edit(_anketSoru);
                    }
                }

                var flag = AnketSoruService.Save();

                if (flag)
                    return Json(true, JsonRequestBehavior.DenyGet);
                return Json(false, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }
        #endregion
    }
}