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
    public class BankaController : BaseController
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
        ISanalPosService SanalPosService;
        IBankaService BankaService;
        IHesapNumarasiService HesapNumarasiService;
        ITaksitService TaksitService;
        public BankaController(IIcerikAyarService iIcerikAyarService,
                               IKullaniciService iKullaniciService,
                               ISanalPosService iSanalPosService,
                               IBankaService iBankaService,
                               IHesapNumarasiService iHesapNumarasiService,
                               ITaksitService iTaksitService) : base(iIcerikAyarService,
                                                                     iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            SanalPosService = iSanalPosService;
            BankaService = iBankaService;
            HesapNumarasiService = iHesapNumarasiService;
            TaksitService = iTaksitService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageProperties = PageProperties.SetPageProperties("Banka", "", "Banka Ayarları", "");

            return View();
        }

        [AuthorizeManager]
        public ActionResult VirtualPosSave(int? id = null)
        {
            ViewBag.PageName = Pages.banka_virtualpossave;

            if (id == null)
            {
                ViewBag.PageProperties = PageProperties.SetPageProperties("Banka", "Sanal Pos İşlemleri", "Sanal Pos İşlemleri", "");
                ViewBag.SanalPos = null;
                return View();
            }

            var banka = BankaService.FindBy(x => x.AktifMi == true && x.BankaId == id).SingleOrDefault();

            if (banka == null)
            {
                ViewBag.PageProperties = PageProperties.SetPageProperties("Banka", "Sanal Pos İşlemleri", "Sanal Pos İşlemleri", "");
                TempData["message"] = "Banka bilgisi bulunamadı. Oyüzden kayıt işlemine devam edemezsiniz.";
                ViewBag.SanalPos = null;
                return View();
            }

            ViewBag.BankaBilgisi = banka.Adi;
            ViewBag.PageProperties = PageProperties.SetPageProperties("Banka", "Sanal Pos (" + banka.Adi + ")", "Sanal Pos İşlemleri", banka.Adi);

            var sanalPos = SanalPosService.FindBy(x => x.BankaId == id).SingleOrDefault();

            if (sanalPos == null)
            {
                ViewBag.SanalPos = new SanalPos() { BankaId = (int)id };
            }
            else
            {
                ViewBag.SanalPos = sanalPos;
            }

            return View();
        }

        [AuthorizeManager]
        public ActionResult AccountNumberSearch()
        {
            ViewBag.PageName = Pages.banka_accountnumbersearch;
            ViewBag.PageProperties = PageProperties.SetPageProperties("Banka", "Hesap Numarası Arama", "Hesap Numarası Arama İşlemleri", "");
            ViewBag.BankaListesi = BankaService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.BankaId, text = x.Adi }).ToList();

            return View();
        }

        [AuthorizeManager]
        public ActionResult AccountNumberSave(int? id = null)
        {
            ViewBag.PageName = Pages.banka_accountnumbersave;
            ViewBag.BankaListesi = BankaService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.BankaId, text = x.Adi }).ToList();

            if (id == null)
            {
                ViewBag.HesapNumarasi = new HesapNumarasi();
                ViewBag.PageProperties = PageProperties.SetPageProperties("Banka", "Hesap Numarası Kayıt", "Hesap Numarası Kayıt İşlemleri", "");
            }
            else
            {
                ViewBag.HesapNumarasi = HesapNumarasiService.FindBy(x => x.HesapNumarasiId == id).SingleOrDefault();
                ViewBag.PageProperties = PageProperties.SetPageProperties("Banka", "Hesap Numarası Güncelleme", "Hesap Numarası Güncelleme İşlemleri", "");
            }

            return View();
        }

        [AuthorizeManager]
        public ActionResult InstallmentSearch()
        {
            ViewBag.PageName = Pages.banka_installmentsearch;
            ViewBag.PageProperties = PageProperties.SetPageProperties("Banka", "Taksit Arama", "Taksit Arama İşlemleri", "");
            ViewBag.BankaListesi = BankaService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.BankaId, text = x.Adi }).ToList();

            return View();
        }

        [AuthorizeManager]
        public ActionResult InstallmentSave(int? id = null)
        {
            ViewBag.PageName = Pages.banka_installmentsave;
            ViewBag.BankaListesi = BankaService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.BankaId, text = x.Adi }).ToList();

            if (id == null)
            {
                ViewBag.Taksit = new Taksit();
                ViewBag.PageProperties = PageProperties.SetPageProperties("Banka", "Taksit Kayıt", "Taksit Kayıt İşlemleri", "");
            }
            else
            {
                ViewBag.Taksit = TaksitService.FindBy(x => x.TaksitId == id).SingleOrDefault();
                ViewBag.PageProperties = PageProperties.SetPageProperties("Banka", "Taksit Güncelleme", "Taksit Güncelleme İşlemleri", "");
            }

            return View();
        }

        public ActionResult HesapNumarasiExcelRaporuAl(int bankaId, int aktiflik)
        {
            var sonucListesi = HesapNumarasiService.FindBy(x =>
            (bankaId == 0 || x.BankaId == bankaId) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "Banka" }).ToList().Select(x => new
            {
                Sira = x.Sira,
                BankaAdi = x.Banka != null ? x.Banka.Adi : "",
                Iban = x.Iban,
                Sehir = x.Sehir,
                SubeKodu = x.SubeKodu,
                SubeAdi = x.SubeAdi,
                HesapNo = x.HesapNo,
                HesapSahibi = x.HesapSahibi,
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }

        public ActionResult TaksitExcelRaporuAl(int bankaId, int aktiflik)
        {
            var sonucListesi = TaksitService.FindBy(x =>
            (bankaId == 0 || x.BankaId == bankaId) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "Banka" }).ToList().Select(x => new
            {
                Sira = x.Sira,
                BankaAdi = x.Banka != null ? x.Banka.Adi : "",
                TaksitSayisi = x.TaksitSayisi,
                FaizOrani = "%" + x.FaizOrani,
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult SanalPosKaydet(SanalPos sanalPos)
        {
            try
            {
                var _sanalPos = SanalPosService.FindBy(x => x.BankaId == sanalPos.BankaId).SingleOrDefault();

                if (_sanalPos == null)
                {
                    _sanalPos = new SanalPos();

                    _sanalPos.AktifMi = sanalPos.AktifMi;
                    _sanalPos.BankaId = sanalPos.BankaId;
                    _sanalPos.ChargeType = sanalPos.ChargeType;
                    _sanalPos.ClientId = sanalPos.ClientId;
                    _sanalPos.Currency = sanalPos.Currency;
                    _sanalPos.Host = sanalPos.Host;
                    _sanalPos.Name = sanalPos.Name;
                    _sanalPos.Password = sanalPos.Password;
                    _sanalPos.PosNumber = sanalPos.PosNumber;
                    _sanalPos.Xcip = sanalPos.Xcip;
                    _sanalPos.DDDAktifMi = sanalPos.DDDAktifMi;
                    _sanalPos.DDDStoreKey = sanalPos.DDDStoreKey;

                    SanalPosService.Add(_sanalPos);
                }
                else
                {
                    _sanalPos.AktifMi = sanalPos.AktifMi;
                    _sanalPos.BankaId = sanalPos.BankaId;
                    _sanalPos.ChargeType = sanalPos.ChargeType;
                    _sanalPos.ClientId = sanalPos.ClientId;
                    _sanalPos.Currency = sanalPos.Currency;
                    _sanalPos.Host = sanalPos.Host;
                    _sanalPos.Name = sanalPos.Name;
                    _sanalPos.Password = sanalPos.Password;
                    _sanalPos.PosNumber = sanalPos.PosNumber;
                    _sanalPos.Xcip = sanalPos.Xcip;
                    _sanalPos.DDDAktifMi = sanalPos.DDDAktifMi;
                    _sanalPos.DDDStoreKey = sanalPos.DDDStoreKey;

                    SanalPosService.Edit(_sanalPos);
                }

                var flag = SanalPosService.Save();

                if (flag)
                    return Json(_sanalPos.SanalPosId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult HesapNumarasiKaydetGuncelle(HesapNumarasi hesapNumarasi)
        {
            try
            {
                HesapNumarasi _hesapNumarasi;

                if (hesapNumarasi.HesapNumarasiId == 0)
                {
                    _hesapNumarasi = new HesapNumarasi();
                    _hesapNumarasi.AktifMi = hesapNumarasi.AktifMi;
                    _hesapNumarasi.BankaId = hesapNumarasi.BankaId;
                    _hesapNumarasi.HesapNo = hesapNumarasi.HesapNo;
                    _hesapNumarasi.HesapSahibi = hesapNumarasi.HesapSahibi;
                    _hesapNumarasi.Iban = hesapNumarasi.Iban;
                    _hesapNumarasi.Sehir = hesapNumarasi.Sehir;
                    _hesapNumarasi.Sira = hesapNumarasi.Sira;
                    _hesapNumarasi.SubeAdi = hesapNumarasi.SubeAdi;
                    _hesapNumarasi.SubeKodu = hesapNumarasi.SubeKodu;

                    HesapNumarasiService.Add(_hesapNumarasi);
                }
                else
                {
                    _hesapNumarasi = HesapNumarasiService.FindBy(x => x.HesapNumarasiId == hesapNumarasi.HesapNumarasiId).SingleOrDefault();

                    if (_hesapNumarasi == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _hesapNumarasi.AktifMi = hesapNumarasi.AktifMi;
                    _hesapNumarasi.BankaId = hesapNumarasi.BankaId;
                    _hesapNumarasi.HesapNo = hesapNumarasi.HesapNo;
                    _hesapNumarasi.HesapSahibi = hesapNumarasi.HesapSahibi;
                    _hesapNumarasi.Iban = hesapNumarasi.Iban;
                    _hesapNumarasi.Sehir = hesapNumarasi.Sehir;
                    _hesapNumarasi.Sira = hesapNumarasi.Sira;
                    _hesapNumarasi.SubeAdi = hesapNumarasi.SubeAdi;
                    _hesapNumarasi.SubeKodu = hesapNumarasi.SubeKodu;

                    HesapNumarasiService.Edit(_hesapNumarasi);
                }

                var flag = HesapNumarasiService.Save();

                if (flag)
                    return Json(_hesapNumarasi.HesapNumarasiId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult HesapNumarasiSil(int id)
        {
            try
            {
                if (id == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _hesapNumarasi = HesapNumarasiService.FindBy(x => x.HesapNumarasiId == id).SingleOrDefault();

                if (_hesapNumarasi == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _hesapNumarasi.AktifMi = false;

                HesapNumarasiService.Edit(_hesapNumarasi);

                var flag = HesapNumarasiService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult HesapNumarasiAra(int bankaId, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = HesapNumarasiService.FindBy(x =>
            (bankaId == 0 || x.BankaId == bankaId) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "Banka" });

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.HesapNumarasiId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    AktifMi = x.AktifMi,
                    BankaId = x.BankaId,
                    BankaAdi = x.Banka != null ? x.Banka.Adi : "",
                    HesapNo = x.HesapNo,
                    HesapNumarasiId = x.HesapNumarasiId,
                    HesapSahibi = x.HesapSahibi,
                    Iban = x.Iban,
                    Sehir = x.Sehir,
                    Sira = x.Sira,
                    SubeAdi = x.SubeAdi,
                    SubeKodu = x.SubeKodu
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TaksitKaydetGuncelle(Taksit taksit)
        {
            try
            {
                Taksit _taksit;

                if (taksit.TaksitId == 0)
                {
                    _taksit = new Taksit();
                    _taksit.AktifMi = taksit.AktifMi;
                    _taksit.BankaId = taksit.BankaId;
                    _taksit.FaizOrani = taksit.FaizOrani;
                    _taksit.Sira = taksit.Sira;
                    _taksit.TaksitSayisi = taksit.TaksitSayisi;

                    TaksitService.Add(_taksit);
                }
                else
                {
                    _taksit = TaksitService.FindBy(x => x.TaksitId == taksit.TaksitId).SingleOrDefault();

                    if (_taksit == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _taksit.AktifMi = taksit.AktifMi;
                    _taksit.BankaId = taksit.BankaId;
                    _taksit.FaizOrani = taksit.FaizOrani;
                    _taksit.Sira = taksit.Sira;
                    _taksit.TaksitSayisi = taksit.TaksitSayisi;

                    TaksitService.Edit(_taksit);
                }

                var flag = TaksitService.Save();

                if (flag)
                    return Json(_taksit.TaksitId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult TaksitSil(int id)
        {
            try
            {
                if (id == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _taksit = TaksitService.FindBy(x => x.TaksitId == id).SingleOrDefault();

                if (_taksit == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _taksit.AktifMi = false;

                TaksitService.Edit(_taksit);

                var flag = TaksitService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult TaksitAra(int bankaId, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = TaksitService.FindBy(x =>
            (bankaId == 0 || x.BankaId == bankaId) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "Banka" });

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.TaksitId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    AktifMi = x.AktifMi,
                    BankaId = x.BankaId,
                    BankaAdi = x.Banka != null ? x.Banka.Adi : "",
                    FaizOrani = x.FaizOrani,
                    Sira = x.Sira,
                    TaksitId = x.TaksitId,
                    TaksitSayisi = x.TaksitSayisi
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}