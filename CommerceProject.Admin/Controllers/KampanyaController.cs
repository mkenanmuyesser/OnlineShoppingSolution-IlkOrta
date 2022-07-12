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
    public class KampanyaController : BaseController
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
        IKampanyaService KampanyaService;
        IKampanyaResimService KampanyaResimService;
        public KampanyaController(IIcerikAyarService iIcerikAyarService,
                                  IKullaniciService iKullaniciService,
                                  IKampanyaService iKampanyaService,
                                  IKampanyaResimService iKampanyaResimService) : base(iIcerikAyarService,
                                                                                      iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            KampanyaService = iKampanyaService;
            KampanyaResimService = iKampanyaResimService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.kampanya_index;
            ViewBag.PageProperties = Helper.PageHelper.PageProperties.SetPageProperties("Kampanya", "Arama", "Kampanya Arama İşlemleri", "");

            return View();
        }       

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.kampanya_save;

            if (id == null)
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Kampanya", "Kayıt", "Kampanya Kayıt İşlemleri", "");
                ViewBag.Kampanya = new Kampanya();
                ViewBag.KampanyaResimler = new List<KampanyaResim>();
            }
            else
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Kampanya", "Güncelleme", "Kampanya Güncelleme İşlemleri", "");
                ViewBag.Kampanya = KampanyaService.FindBy(x => x.KampanyaId == id).SingleOrDefault();
                ViewBag.KampanyaResimler = KampanyaResimService.FindBy(x => x.KampanyaId == id).ToList();
            }

            return View();
        }

        public ActionResult ExcelRaporuAl(string adi, string konu, string icerik, DateTime? baslangicTarihi, DateTime? bitisTarihi, int anasayfa, int aktiflik)
        {
            var sonucListesi = KampanyaService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(konu) || x.Konu.Contains(konu)) &&
            (string.IsNullOrEmpty(icerik) || x.Icerik.Contains(icerik)) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (x.Tarih >= baslangicTarihi && x.Tarih <= bitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (x.Tarih >= baslangicTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (x.Tarih <= bitisTarihi)))
            ) &&
            (anasayfa == 2 || ((anasayfa == 0 && x.AnaSayfadaGosterilsinMi == false) || (anasayfa == 1 && x.AnaSayfadaGosterilsinMi == true))) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList()
            .Select(x => new
            {
                Adi = x.Adi,
                Konu = x.Konu,
                Icerik = x.Icerik,
                BaslangicTarihi = x.BaslangicTarihi.HasValue ? x.BaslangicTarihi.Value.ToString("dd.MM.yyyy") : "",
                BitisTarihi = x.BitisTarihi.HasValue ? x.BitisTarihi.Value.ToString("dd.MM.yyyy") : "",
                YayinlanmaTarihi = x.Tarih.ToString("dd.MM.yyyy"),
                AnaSayfadaGosterilsinMi = x.AnaSayfadaGosterilsinMi ? "Evet" : "Hayır",
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult KaydetGuncelle(Kampanya kampanya)
        {
            try
            {
                Kampanya _kampanya;

                if (kampanya.KampanyaId == 0)
                {
                    _kampanya = new Kampanya();
                    _kampanya.Adi = kampanya.Adi;
                    _kampanya.AktifMi = kampanya.AktifMi;
                    _kampanya.AnaSayfadaGosterilsinMi = kampanya.AnaSayfadaGosterilsinMi;
                    _kampanya.BaslangicTarihi = kampanya.BaslangicTarihi;
                    _kampanya.BitisTarihi = kampanya.BitisTarihi;
                    _kampanya.Icerik = kampanya.Icerik;
                    _kampanya.Konu = kampanya.Konu;
                    _kampanya.Tarih = kampanya.Tarih;

                    KampanyaService.Add(_kampanya);
                }
                else
                {
                    _kampanya = KampanyaService.FindBy(x => x.KampanyaId == kampanya.KampanyaId).SingleOrDefault();

                    if (_kampanya == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _kampanya.Adi = kampanya.Adi;
                    _kampanya.AktifMi = kampanya.AktifMi;
                    _kampanya.AnaSayfadaGosterilsinMi = kampanya.AnaSayfadaGosterilsinMi;
                    _kampanya.BaslangicTarihi = kampanya.BaslangicTarihi;
                    _kampanya.BitisTarihi = kampanya.BitisTarihi;
                    _kampanya.Icerik = kampanya.Icerik;
                    _kampanya.Konu = kampanya.Konu;
                    _kampanya.Tarih = kampanya.Tarih;

                    KampanyaService.Edit(_kampanya);
                }

                var flag = KampanyaService.Save();

                if (flag)
                    return Json(_kampanya.KampanyaId, JsonRequestBehavior.DenyGet);
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

                var _kampanya = KampanyaService.FindBy(x => x.KampanyaId == id).SingleOrDefault();

                if (_kampanya == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _kampanya.AktifMi = false;

                KampanyaService.Edit(_kampanya);

                var flag = KampanyaService.Save();

                if (!flag)
                    return Json(false, JsonRequestBehavior.DenyGet);

                // İlişkili resimler AktifMi=false
                var _resimler = KampanyaResimService.FindBy(x => x.AktifMi == true && x.KampanyaId == id).ToList();

                foreach (var _resim in _resimler)
                {
                    _resim.AktifMi = false;
                    KampanyaResimService.Edit(_resim);

                    KampanyaResimService.Save();
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

                    var savePath = Path.Combine(Server.MapPath("~/Uploads/Campaign/"), guid + dosyaUzantisi);
                    dosyaPath = string.Format("/Uploads/Campaign/{0}",
                                        guid + dosyaUzantisi);

                    Request.Files[0].SaveAs(savePath);

                    // save db
                    var _kampanyaResim = new KampanyaResim();
                    _kampanyaResim.AktifMi = true;
                    _kampanyaResim.KampanyaId = key;
                    _kampanyaResim.ResimYolu = dosyaPath;
                    _kampanyaResim.Tarih = DateTime.Now;
                    _kampanyaResim.YeniMi = true;

                    KampanyaResimService.Add(_kampanyaResim);
                    var flag = KampanyaResimService.Save();

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
        public JsonResult Ara(string adi, string konu, string icerik, DateTime? baslangicTarihi, DateTime? bitisTarihi, int anasayfa, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = KampanyaService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(konu) || x.Konu.Contains(konu)) &&
            (string.IsNullOrEmpty(icerik) || x.Icerik.Contains(icerik)) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (x.Tarih >= baslangicTarihi && x.Tarih <= bitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (x.Tarih >= baslangicTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (x.Tarih <= bitisTarihi)))
            ) &&
            (anasayfa == 2 || ((anasayfa == 0 && x.AnaSayfadaGosterilsinMi == false) || (anasayfa == 1 && x.AnaSayfadaGosterilsinMi == true))) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.KampanyaId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    KampanyaId = x.KampanyaId,
                    AktifMi = x.AktifMi,
                    AnaSayfadaGosterilsinMi = x.AnaSayfadaGosterilsinMi,
                    Adi = x.Adi,
                    Konu = x.Konu,
                    BaslangicTarihi = x.BaslangicTarihi.HasValue ? x.BaslangicTarihi.Value.ToString("dd.MM.yyyy") : "",
                    BitisTarihi = x.BitisTarihi.HasValue ? x.BitisTarihi.Value.ToString("dd.MM.yyyy") : "",
                    Icerik = x.Icerik,
                    Tarih = x.Tarih.ToString("dd.MM.yyyy")
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ResimKaydetGuncelle(KampanyaResim kampanyaResim)
        {
            try
            {
                if (kampanyaResim == null || kampanyaResim.KampanyaResimId == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _kampanyaResim = KampanyaResimService.FindBy(x => x.KampanyaResimId == kampanyaResim.KampanyaResimId).SingleOrDefault();

                if (_kampanyaResim == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _kampanyaResim.Aciklama = kampanyaResim.Aciklama;
                _kampanyaResim.AktifMi = kampanyaResim.AktifMi;
                _kampanyaResim.AltAttribute = kampanyaResim.AltAttribute;
                _kampanyaResim.Tarih = kampanyaResim.Tarih;
                _kampanyaResim.TitleAttribute = kampanyaResim.TitleAttribute;
                _kampanyaResim.YeniMi = kampanyaResim.YeniMi;

                KampanyaResimService.Edit(_kampanyaResim);

                var flag = KampanyaResimService.Save();

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