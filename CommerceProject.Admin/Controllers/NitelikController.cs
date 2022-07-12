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
    public class NitelikController : BaseController
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
        INitelikService NitelikService;
        INitelikGrupService NitelikGrupService;
        IUrunNitelikService UrunNitelikService;
        IUrunService UrunService;
        public NitelikController(IIcerikAyarService iIcerikAyarService,
                                 IKullaniciService iKullaniciService,
                                 INitelikService iNitelikService,
                                 IUrunNitelikService iUrunNitelikService,
                                 INitelikGrupService iNitelikGrupService,
                                 IUrunService iUrunService) : base(iIcerikAyarService,
                                                                   iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            NitelikService = iNitelikService;
            UrunNitelikService = iUrunNitelikService;
            NitelikGrupService = iNitelikGrupService;
            UrunService = iUrunService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.nitelik_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Nitelik", "Arama", "Nitelik Arama İşlemleri", "");
            ViewBag.NitelikGrupListesi = NitelikGrupService.GetAll(true).ToList().Select(x => new { id = x.NitelikGrupId, text = x.Adi, aktif = x.AktifMi }).ToList();

            return View();
        }     

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.nitelik_save;
            ViewBag.NitelikGrupListesi = NitelikGrupService.GetAll(true).ToList().Select(x => new { id = x.NitelikGrupId, text = x.Adi, aktif = x.AktifMi }).ToList();

            if (id == null)
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Nitelik", "Kayıt", "Nitelik Kayıt İşlemleri", "");
                ViewBag.Nitelik = new Nitelik();
            }
            else
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Nitelik", "Güncelleme", "Nitelik Güncelleme İşlemleri", "");
                ViewBag.Nitelik = NitelikService.FindBy(x => x.NitelikId == id).SingleOrDefault();
            }

            return View();
        }

        public ActionResult UrunNitelikDetayPartial(int id)
        {
            ViewBag.NitelikId = id;

            return PartialView("~/Views/Nitelik/Partials/UrunNitelikDetayPartial.cshtml");
        }

        public ActionResult UrunNitelikEkleGuncellePartial(int? id)
        {
            if(id == null)
            {
                ViewBag.UrunNitelik = new UrunNitelik();
            }
            else
            {
                ViewBag.UrunNitelik = UrunNitelikService.FindBy(x => x.UrunNitelikId == id, true, new string[] { "Urun" }).SingleOrDefault();
            }

            return PartialView("~/Views/Nitelik/Partials/UrunNitelikEkleGuncellePartial.cshtml");
        }

        public ActionResult ExcelRaporuAl(int nitelikGrupId, string adi, string aciklama, int aktiflik)
        {
            var sonucListesi = NitelikService.FindBy(x =>
            (nitelikGrupId == 0 || x.NitelikGrupId == nitelikGrupId) &&
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "NitelikGrup" }).ToList().Select(x => new
            {
                Adi = x.Adi,
                Aciklama = x.Aciklama,
                NitelikGrubuAdi = x.NitelikGrup.Adi,
                OlusturmaTarihi = x.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm"),
                Sira = x.Sira,
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult Ara(int nitelikGrupId, string adi, string aciklama, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = NitelikService.FindBy(x =>
            (nitelikGrupId == 0 || x.NitelikGrupId == nitelikGrupId) &&
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "NitelikGrup" });

            var count = tempList.Count();

            var sonucListesi = tempList.OrderByDescending(x => x.NitelikId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    NitelikId = x.NitelikId,
                    NitelikGrupAdi = x.NitelikGrup.Adi,
                    Aciklama = x.Aciklama,
                    Adi = x.Adi,
                    AktifMi = x.AktifMi,
                    OlusturmaTarihi = x.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm"),
                    Sira = x.Sira
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult KaydetGuncelle(Nitelik nitelik)
        {
            try
            {
                Nitelik _nitelik;

                if (nitelik.NitelikId == 0)
                {
                    _nitelik = new Nitelik();
                    _nitelik.Aciklama = nitelik.Aciklama;
                    _nitelik.Adi = nitelik.Adi;
                    _nitelik.AktifMi = nitelik.AktifMi;
                    _nitelik.GuncellemeTarihi = DateTime.Now;
                    _nitelik.NitelikGrupId = nitelik.NitelikGrupId;
                    _nitelik.OlusturmaTarihi = DateTime.Now;
                    _nitelik.Sira = nitelik.Sira;

                    NitelikService.Add(_nitelik);
                }
                else
                {
                    _nitelik = NitelikService.FindBy(x => x.NitelikId == nitelik.NitelikId).SingleOrDefault();

                    if (_nitelik == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _nitelik.Aciklama = nitelik.Aciklama;
                    _nitelik.Adi = nitelik.Adi;
                    _nitelik.AktifMi = nitelik.AktifMi;
                    _nitelik.GuncellemeTarihi = DateTime.Now;
                    _nitelik.NitelikGrupId = nitelik.NitelikGrupId;
                    _nitelik.Sira = nitelik.Sira;

                    NitelikService.Edit(_nitelik);
                }

                var flag = NitelikService.Save();

                if (flag)
                    return Json(_nitelik.NitelikId, JsonRequestBehavior.DenyGet);
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

                var _nitelik = NitelikService.FindBy(x => x.NitelikId == id).SingleOrDefault();

                if (_nitelik == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _nitelik.AktifMi = false;

                NitelikService.Edit(_nitelik);

                var flag = NitelikService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult UrunNitelikAra(int nitelikId, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = UrunNitelikService.FindBy(x => x.NitelikId == nitelikId, true, new string[] { "Nitelik", "Nitelik.NitelikGrup", "Urun" });

            var count = tempList.Count();

            var sonucListesi = tempList.OrderByDescending(x => x.UrunNitelikId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    AktifMi = x.AktifMi,
                    NitelikDegeri = x.NitelikDegeri,
                    NitelikId = x.NitelikId,
                    Sira = x.Sira,
                    UrunId = x.UrunId,
                    UrunNitelikId = x.UrunNitelikId,
                    UrunKod = x.Urun.UrunKod,
                    UrunBarkod = x.Urun.Barkod,
                    UrunAdi = x.Urun.Adi,
                    NitelikAdi = x.Nitelik != null ? x.Nitelik.Adi : "",
                    NitelikGrupAdi = x.Nitelik != null ? (x.Nitelik.NitelikGrup != null ? x.Nitelik.NitelikGrup.Adi : "") : ""
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UrunNitelikKaydetGuncelle(UrunNitelik urunNitelik)
        {
            try
            {
                UrunNitelik _urunNitelik;

                if (urunNitelik.UrunNitelikId == 0)
                {
                    // Bu nitelik için ürün kaydı daha önce yapılmış mı?
                    var _oncekiUrunNitelik = UrunNitelikService.FindBy(x => x.UrunId == urunNitelik.UrunId && x.NitelikId == urunNitelik.NitelikId && x.AktifMi == true).SingleOrDefault();
                    if(_oncekiUrunNitelik != null)
                    {
                        return Json(-1, JsonRequestBehavior.DenyGet);
                    }

                    _urunNitelik = new UrunNitelik();
                    _urunNitelik.AktifMi = urunNitelik.AktifMi;
                    _urunNitelik.NitelikDegeri = urunNitelik.NitelikDegeri;
                    _urunNitelik.NitelikId = urunNitelik.NitelikId;
                    _urunNitelik.Sira = urunNitelik.Sira;
                    _urunNitelik.UrunId = urunNitelik.UrunId;

                    UrunNitelikService.Add(_urunNitelik);
                }
                else
                {
                    _urunNitelik = UrunNitelikService.FindBy(x => x.UrunNitelikId == urunNitelik.UrunNitelikId).SingleOrDefault();

                    if (_urunNitelik == null)
                        return Json(0, JsonRequestBehavior.DenyGet);

                    _urunNitelik.AktifMi = urunNitelik.AktifMi;
                    _urunNitelik.NitelikDegeri = urunNitelik.NitelikDegeri;
                    _urunNitelik.Sira = urunNitelik.Sira;
                    _urunNitelik.UrunId = urunNitelik.UrunId;

                    UrunNitelikService.Edit(_urunNitelik);
                }

                var flag = UrunNitelikService.Save();

                if (flag)
                    return Json(_urunNitelik.UrunNitelikId, JsonRequestBehavior.DenyGet);
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