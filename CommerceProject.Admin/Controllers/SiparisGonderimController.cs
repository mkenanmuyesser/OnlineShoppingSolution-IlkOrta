using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.BusinessContracts;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Business.Entities;
using System.Data;
using CommerceProject.Admin.Helper;
using static CommerceProject.Admin.Helper.PageHelper;
using CommerceProject.Business.Helper.Email;

namespace CommerceProject.Admin.Controllers
{
    public class SiparisGonderimController : BaseController
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
        ISiparisGonderimService SiparisGonderimService;
        IGonderimService GonderimService;
        ITeslimZamaniService TeslimZamaniService;
        IAdresIlService AdresIlService;
        IAdresIlceService AdresIlceService;
        ISiparisService SiparisService;
        ISiparisDetayService SiparisDetayService;
        ISiparisDurumTipService SiparisDurumTipService;
        public SiparisGonderimController(IIcerikAyarService iIcerikAyarService,
                                         IKullaniciService iKullaniciService, 
                                         ISiparisGonderimService iSiparisGonderimService, 
                                         IGonderimService iGonderimService, 
                                         ITeslimZamaniService iTeslimZamaniService,
                                         IAdresIlService iAdresIlService,
                                         IAdresIlceService iAdresIlceService,
                                         ISiparisService iSiparisService,
                                         ISiparisDetayService iSiparisDetayService,
                                         ISiparisDurumTipService iSiparisDurumTipService) : base(iIcerikAyarService,
                                                                                                 iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            SiparisGonderimService = iSiparisGonderimService;
            GonderimService = iGonderimService;
            TeslimZamaniService = iTeslimZamaniService;
            AdresIlService = iAdresIlService;
            AdresIlceService = iAdresIlceService;
            SiparisService = iSiparisService;
            SiparisDetayService = iSiparisDetayService;
            SiparisDurumTipService = iSiparisDurumTipService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.siparisgonderim_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Sipariş Gönderim", "Arama", "Sipariş Gönderim Arama İşlemleri", "");
            ViewBag.GonderimTipListesi = GonderimService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.GonderimId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.TeslimZamaniTipListesi = TeslimZamaniService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.TeslimZamaniId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.IlListesi = AdresIlService.GetAll(true).ToList().Select(x => new { id = x.AdresIlId, text = x.IlAdi, aktif = x.AktifMi }).ToList();
            ViewBag.IlceListesi = AdresIlceService.GetAll(true).ToList().Select(x => new { id = x.AdresIlceId, text = x.IlceAdi, ilId = x.AdresIlId, aktif = x.AktifMi }).ToList();

            return View();
        }
       
        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.siparisgonderim_save;
            ViewBag.GonderimTipListesi = GonderimService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.GonderimId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.TeslimZamaniTipListesi = TeslimZamaniService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.TeslimZamaniId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.SiparisDurumTipListesi = SiparisDurumTipService.FindBy(x => x.AktifMi == true).ToList().Select(x => new { id = x.SiparisDurumTipId, text = x.Adi, aktif = x.AktifMi }).ToList();

            if (id == null)
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Sipariş Gönderim", "Kayıt", "Sipariş Gönderim Kayıt İşlemleri", "");
                ViewBag.SiparisGonderim = new SiparisGonderim();
            }
            else
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Sipariş Gönderim", "Güncelleme", "Sipariş Gönderim Güncelleme İşlemleri", "");
                ViewBag.SiparisGonderim = SiparisGonderimService.FindBy(x => x.SiparisGonderimId == id, true, new string[] { "Gonderim", "TeslimZamani", "SiparisDetay", "SiparisDetay.Urun", "SiparisDetay.Siparis" }).SingleOrDefault();
            }

            return View();
        }

        public ActionResult ExcelRaporuAl(int gonderimAdresIlId, int gonderimAdresIlceId, int gonderimTipId, int teslimZamaniTipId, string takipNo, string aciklama, DateTime? gonderimBaslangic, DateTime? gonderimBitis, DateTime? teslimBaslangic, DateTime? teslimBitis, DateTime? siparisBaslangic, DateTime? siparisBitis, int aktiflik)
        {
            var sonucListesi = SiparisGonderimService.FindBy(x =>
            (gonderimAdresIlId == 0 || x.SiparisDetay.Siparis.Adres.AdresIlId == gonderimAdresIlId) &&
            (gonderimAdresIlceId == 0 || x.SiparisDetay.Siparis.Adres.AdresIlceId == gonderimAdresIlceId) &&
            (gonderimTipId == 0 || x.GonderimId == gonderimTipId) &&
            (teslimZamaniTipId == 0 || x.TeslimZamaniId == teslimZamaniTipId) &&
            (string.IsNullOrEmpty(takipNo) || x.TakipNo.Contains(takipNo)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (
                (gonderimBaslangic == null && gonderimBitis == null) ||
                ((gonderimBaslangic != null && gonderimBitis != null) && (gonderimBaslangic <= x.GonderimTarihi && gonderimBitis >= x.GonderimTarihi)) ||
                ((gonderimBaslangic != null && gonderimBitis == null) && (gonderimBaslangic <= x.GonderimTarihi)) ||
                ((gonderimBaslangic == null && gonderimBitis != null) && (gonderimBitis >= x.GonderimTarihi))
            ) &&
            (
                (teslimBaslangic == null && teslimBitis == null) ||
                ((teslimBaslangic != null && teslimBitis != null) && (teslimBaslangic <= x.TeslimTarihi && teslimBitis >= x.TeslimTarihi)) ||
                ((teslimBaslangic != null && teslimBitis == null) && (teslimBaslangic <= x.TeslimTarihi)) ||
                ((teslimBaslangic == null && teslimBitis != null) && (teslimBitis >= x.TeslimTarihi))
            ) &&
            (
                (siparisBaslangic == null && siparisBitis == null) ||
                ((siparisBaslangic != null && siparisBitis != null) && (siparisBaslangic <= x.SiparisDetay.Siparis.SiparisTarihi && siparisBitis >= x.SiparisDetay.Siparis.SiparisTarihi)) ||
                ((siparisBaslangic != null && siparisBitis == null) && (siparisBaslangic <= x.SiparisDetay.Siparis.SiparisTarihi)) ||
                ((siparisBaslangic == null && siparisBitis != null) && (siparisBitis >= x.SiparisDetay.Siparis.SiparisTarihi))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "Gonderim", "TeslimZamani", "SiparisDetay", "SiparisDetay.Urun", "SiparisDetay.Siparis", "SiparisDetay.Siparis.Adres", "SiparisDetay.Siparis.Adres.AdresIl", "SiparisDetay.Siparis.Adres.AdresIlce" }).ToList()
            .Select(x => new
            {
                TakipNo = x.TakipNo,
                Adet = x.Adet,
                UrunAdi = x.SiparisDetay.Urun.Adi,
                GonderimAdresiIlceAdi = x.SiparisDetay.Siparis.Adres.AdresIlce.IlceAdi,
                GonderimAdresiIlAdi = x.SiparisDetay.Siparis.Adres.AdresIl.IlAdi,
                GonderimTipi = x.Gonderim.Adi,
                TeslimZamaniTipi = x.TeslimZamani.Adi,
                Aciklama = x.Aciklama,
                GonderimTarihi = x.GonderimTarihi.ToString("dd.MM.yyyy HH:mm"),
                TeslimTarihi = x.TeslimTarihi.HasValue ? x.TeslimTarihi.Value.ToString("dd.MM.yyyy HH:mm") : "",
                SiparisTarihi = x.SiparisDetay.Siparis.SiparisTarihi.ToString("dd.MM.yyyy HH:mm"),
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            })
            .ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult Sil(int id)
        {
            try
            {
                if (id == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _siparisGonderim = SiparisGonderimService.FindBy(x => x.SiparisGonderimId == id).SingleOrDefault();

                if (_siparisGonderim == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _siparisGonderim.AktifMi = false;

                SiparisGonderimService.Edit(_siparisGonderim);

                var flag = SiparisGonderimService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult Ara(int gonderimAdresIlId, int gonderimAdresIlceId, int gonderimTipId, int teslimZamaniTipId, string takipNo, string aciklama, DateTime? gonderimBaslangic, DateTime? gonderimBitis, DateTime? teslimBaslangic, DateTime? teslimBitis, DateTime? siparisBaslangic, DateTime? siparisBitis, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = SiparisGonderimService.FindBy(x =>
            (gonderimAdresIlId == 0 || x.SiparisDetay.Siparis.Adres.AdresIlId == gonderimAdresIlId) &&
            (gonderimAdresIlceId == 0 || x.SiparisDetay.Siparis.Adres.AdresIlceId == gonderimAdresIlceId) &&
            (gonderimTipId == 0 || x.GonderimId == gonderimTipId) &&
            (teslimZamaniTipId == 0 || x.TeslimZamaniId == teslimZamaniTipId) &&
            (string.IsNullOrEmpty(takipNo) || x.TakipNo.Contains(takipNo)) &&
            (string.IsNullOrEmpty(aciklama) || x.Aciklama.Contains(aciklama)) &&
            (
                (gonderimBaslangic == null && gonderimBitis == null) ||
                ((gonderimBaslangic != null && gonderimBitis != null) && (gonderimBaslangic <= x.GonderimTarihi && gonderimBitis >= x.GonderimTarihi)) ||
                ((gonderimBaslangic != null && gonderimBitis == null) && (gonderimBaslangic <= x.GonderimTarihi)) ||
                ((gonderimBaslangic == null && gonderimBitis != null) && (gonderimBitis >= x.GonderimTarihi))
            ) &&
            (
                (teslimBaslangic == null && teslimBitis == null) ||
                ((teslimBaslangic != null && teslimBitis != null) && (teslimBaslangic <= x.TeslimTarihi && teslimBitis >= x.TeslimTarihi)) ||
                ((teslimBaslangic != null && teslimBitis == null) && (teslimBaslangic <= x.TeslimTarihi)) ||
                ((teslimBaslangic == null && teslimBitis != null) && (teslimBitis >= x.TeslimTarihi))
            ) &&
            (
                (siparisBaslangic == null && siparisBitis == null) ||
                ((siparisBaslangic != null && siparisBitis != null) && (siparisBaslangic <= x.SiparisDetay.Siparis.SiparisTarihi && siparisBitis >= x.SiparisDetay.Siparis.SiparisTarihi)) ||
                ((siparisBaslangic != null && siparisBitis == null) && (siparisBaslangic <= x.SiparisDetay.Siparis.SiparisTarihi)) ||
                ((siparisBaslangic == null && siparisBitis != null) && (siparisBitis >= x.SiparisDetay.Siparis.SiparisTarihi))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            , true, new string[] { "Gonderim", "TeslimZamani", "SiparisDetay", "SiparisDetay.Urun", "SiparisDetay.Siparis", "SiparisDetay.Siparis.Adres", "SiparisDetay.Siparis.Adres.AdresIl", "SiparisDetay.Siparis.Adres.AdresIlce" });

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.SiparisGonderimId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    SiparisGonderimId = x.SiparisGonderimId,
                    TakipNo = x.TakipNo,
                    Adet = x.Adet,
                    UrunAdi = x.SiparisDetay.Urun.Adi,
                    UrunId = x.SiparisDetay.UrunId,
                    IlceAdi = x.SiparisDetay.Siparis.Adres.AdresIlce.IlceAdi,
                    IlAdi = x.SiparisDetay.Siparis.Adres.AdresIl.IlAdi,
                    GonderimTipi = x.Gonderim.Adi,
                    TeslimZamaniTipi = x.TeslimZamani.Adi,
                    Aciklama = x.Aciklama,
                    GonderimTarihi = x.GonderimTarihi.ToString("dd.MM.yyyy HH:mm"),
                    TeslimTarihi = x.TeslimTarihi.HasValue ? x.TeslimTarihi.Value.ToString("dd.MM.yyyy HH:mm") : "",
                    SiparisTarihi = x.SiparisDetay.Siparis.SiparisTarihi.ToString("dd.MM.yyyy HH:mm"),
                    AktifMi = x.AktifMi
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SiparisDetayGetir(int siparisId)
        {
            var sonucListesi = SiparisDetayService.FindBy(x => x.SiparisId == siparisId, true, new string[] { "Urun" }).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    SiparisDetayId = x.SiparisDetayId,
                    UrunKod = x.Urun.UrunKod,
                    UrunBarkod = x.Urun.Barkod,
                    UrunAdi = x.Urun.Adi,
                    Adet = x.Adet,
                    AktifMi = x.AktifMi
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SiparisGonderimAdetKontrol(int siparisDetayId, int adet)
        {
            try
            {
                if (siparisDetayId == 0)
                    return Json(new
                    {
                        flag = false,
                        message = "Lütfen sipariş detayı seçiniz."
                    }, JsonRequestBehavior.DenyGet);

                if (adet == 0)
                    return Json(new
                    {
                        flag = false,
                        message = "Lütfen adet giriniz."
                    }, JsonRequestBehavior.DenyGet);

                var _siparisDetay = SiparisDetayService.FindBy(x => x.SiparisDetayId == siparisDetayId, true, new string[] { "SiparisGonderim" }).SingleOrDefault();

                if (_siparisDetay == null)
                {
                    return Json(new
                    {
                        flag = false,
                        message = "Sipariş detayı bulunamadı."
                    }, JsonRequestBehavior.DenyGet);
                }

                if (_siparisDetay.SiparisGonderim.Count == 0)
                {
                    return Json(new
                    {
                        flag = true,
                        message = ""
                    }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    var gonderilecekAdet = _siparisDetay.Adet;
                    var gonderilenAdet = _siparisDetay.SiparisGonderim.Sum(x => x.Adet);

                    if ((gonderilenAdet + adet) > gonderilecekAdet)
                    {
                        return Json(new
                        {
                            flag = false,
                            message = "Gönderilen Ürün Adedi: " + gonderilenAdet + "</br>" + "Girilen Ürün Adedi: " + adet + "</br>" + "<b>Kalan Gönderim Adedi: </b>" + (gonderilecekAdet - gonderilenAdet) + "</br></br>Lütfen girilen miktarı kontrol edin."
                        }, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            flag = true,
                            message = ""
                        }, JsonRequestBehavior.DenyGet);
                    }
                }
            }
            catch (Exception)
            {
                return Json(new
                {
                    flag = false,
                    message = "İşlem sırasında bir sorun oluştu. Lütfen tekrar deneyin."
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult SiparisGonderimKaydetGuncelle(SiparisGonderim siparisGonderim)
        {
            try
            {
                if (siparisGonderim == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                SiparisGonderim _siparisGonderim;
                if (siparisGonderim.SiparisGonderimId == 0)
                {
                    _siparisGonderim = new SiparisGonderim();
                    _siparisGonderim.Aciklama = siparisGonderim.Aciklama;
                    _siparisGonderim.Adet = siparisGonderim.Adet;
                    _siparisGonderim.AktifMi = siparisGonderim.AktifMi;
                    _siparisGonderim.GonderimId = siparisGonderim.GonderimId;
                    _siparisGonderim.GonderimTarihi = siparisGonderim.GonderimTarihi;
                    _siparisGonderim.GuncellemeTarihi = DateTime.Now;
                    _siparisGonderim.OlusturmaTarihi = DateTime.Now;
                    _siparisGonderim.SiparisDetayId = siparisGonderim.SiparisDetayId;
                    _siparisGonderim.TakipNo = siparisGonderim.TakipNo;
                    _siparisGonderim.TeslimTarihi = siparisGonderim.TeslimTarihi;
                    _siparisGonderim.TeslimZamaniId = siparisGonderim.TeslimZamaniId;

                    SiparisGonderimService.Add(_siparisGonderim);
                }
                else
                {
                    _siparisGonderim = SiparisGonderimService.FindBy(x => x.SiparisGonderimId == siparisGonderim.SiparisGonderimId).SingleOrDefault();

                    if (_siparisGonderim != null)
                    {
                        _siparisGonderim.Aciklama = siparisGonderim.Aciklama;
                        _siparisGonderim.Adet = siparisGonderim.Adet;
                        _siparisGonderim.AktifMi = siparisGonderim.AktifMi;
                        _siparisGonderim.GonderimId = siparisGonderim.GonderimId;
                        _siparisGonderim.GonderimTarihi = siparisGonderim.GonderimTarihi;
                        _siparisGonderim.GuncellemeTarihi = DateTime.Now;
                        _siparisGonderim.TakipNo = siparisGonderim.TakipNo;
                        _siparisGonderim.TeslimTarihi = siparisGonderim.TeslimTarihi;
                        _siparisGonderim.TeslimZamaniId = siparisGonderim.TeslimZamaniId;

                        SiparisGonderimService.Edit(_siparisGonderim);
                    }
                }

                var flag = SiparisGonderimService.Save();

                if (flag)
                    return Json(_siparisGonderim.SiparisGonderimId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult KullaniciEpostaGonder(Guid kullaniciId, string epostaKonu, string epostaIcerik)
        {
            try
            {
                var _kullanici = KullaniciService.FindBy(x => x.KullaniciId == kullaniciId).SingleOrDefault();

                if (_kullanici == null)
                    return Json(new
                    {
                        flag = false,
                        message = "Kullanıcı bulunamadı."
                    }, JsonRequestBehavior.DenyGet);

                var flag = EmailHelper.SendMail(epostaKonu, epostaIcerik, _kullanici.Eposta);

                if (flag)
                    return Json(new
                    {
                        flag = true,
                        message = "Eposta başarıyla gönderildi."
                    }, JsonRequestBehavior.DenyGet);

                return Json(new
                {
                    flag = false,
                    message = "İşlem sırasında bir sorun oluştu. Lütfen tekrar deneyin."
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    flag = false,
                    message = "İşlem sırasında bir sorun oluştu. Lütfen tekrar deneyin."
                }, JsonRequestBehavior.DenyGet);
            }
        }
        #endregion
    }
}