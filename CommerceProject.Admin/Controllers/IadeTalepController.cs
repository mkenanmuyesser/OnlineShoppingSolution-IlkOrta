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
using CommerceProject.Business.Helper.Email;

namespace CommerceProject.Admin.Controllers
{
    public class IadeTalepController : BaseController
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
        IIadeTalepService IadeTalepService;
        IIadeTalepDurumTipService IadeTalepDurumTipService;
        IIadeTalepIstekTipService IadeTalepIstekTipService;
        IIadeTalepNedenTipService IadeTalepNedenTipService;
        public IadeTalepController(IIcerikAyarService iIcerikAyarService,
                                   IKullaniciService iKullaniciService,
                                   IIadeTalepService iIadeTalepService,
                                   IIadeTalepDurumTipService iIadeTalepDurumTipService,
                                   IIadeTalepIstekTipService iIadeTalepIstekTipService,
                                   IIadeTalepNedenTipService iIadeTalepNedenTipService) : base(iIcerikAyarService,
                                                                                               iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            IadeTalepService = iIadeTalepService;
            IadeTalepDurumTipService = iIadeTalepDurumTipService;
            IadeTalepIstekTipService = iIadeTalepIstekTipService;
            IadeTalepNedenTipService = iIadeTalepNedenTipService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.iadetalep_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("İade Talep", "Arama", "İade Talep Arama İşlemleri", "");
            ViewBag.IadeTalepDurumTipListesi = IadeTalepDurumTipService.GetAll(true).ToList().Select(x => new { id = x.IadeTalepDurumTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.IadeTalepIstekTipListesi = IadeTalepIstekTipService.GetAll(true).ToList().Select(x => new { id = x.IadeTalepIstekTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.IadeTalepNedenTipListesi = IadeTalepNedenTipService.GetAll(true).ToList().Select(x => new { id = x.IadeTalepNedenTipId, text = x.Adi, aktif = x.AktifMi }).ToList();

            return View();
        }     

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.iadetalep_save;

            if (id == null)
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("İade Talep", "Kayıt", "İade Talep Kayıt İşlemleri", "");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("İade Talep", "Güncelleme", "İade Talep Güncelleme İşlemleri", "");
                ViewBag.IadeTalep = IadeTalepService.FindBy(x => x.IadeTalepId == id, true, new string[] { "SiparisDetay", "SiparisDetay.Siparis" }).SingleOrDefault();
                ViewBag.IadeTalepDurumTipListesi = IadeTalepDurumTipService.GetAll(true).ToList().Select(x => new { id = x.IadeTalepDurumTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
                ViewBag.IadeTalepIstekTipListesi = IadeTalepIstekTipService.GetAll(true).ToList().Select(x => new { id = x.IadeTalepIstekTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
                ViewBag.IadeTalepNedenTipListesi = IadeTalepNedenTipService.GetAll(true).ToList().Select(x => new { id = x.IadeTalepNedenTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
            }

            return View();
        }

        public ActionResult IadeTalepDetayPartial(int id)
        {
            ViewBag.IadeTalep = IadeTalepService.GetSingle(x => x.IadeTalepId == id, true, new string[] { "IadeTalepDurumTip", "IadeTalepIstekTip", "IadeTalepNedenTip", "SiparisDetay", "SiparisDetay.Urun", "SiparisDetay.Siparis", "SiparisDetay.Siparis.Kullanici", "SiparisDetay.Siparis.Kullanici.KullaniciDetay" });

            return PartialView("~/Views/IadeTalep/Partials/IadeTalepDetayPartial.cshtml");
        }

        public ActionResult ExcelRaporuAl(int iadeTalepNo, int iadeTalepIstekTipId, int iadeTalepNedenTipId, int iadeTalepDurumTipId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int aktiflik)
        {
            var sonucListesi = IadeTalepService.GetAll(true, new string[] { "IadeTalepDurumTip", "IadeTalepIstekTip", "IadeTalepNedenTip", "SiparisDetay", "SiparisDetay.Urun", "SiparisDetay.Siparis.Kullanici" }).Where(x =>
            (iadeTalepNo == 0 || x.IadeTalepId == iadeTalepNo) &&
            (iadeTalepIstekTipId == 0 || x.IadeTalepIstekTipId == iadeTalepIstekTipId) &&
            (iadeTalepNedenTipId == 0 || x.IadeTalepNedenTipId == iadeTalepNedenTipId) &&
            (iadeTalepDurumTipId == 0 || x.IadeTalepDurumTipId == iadeTalepDurumTipId) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (x.OlusturmaTarihi >= baslangicTarihi && x.OlusturmaTarihi <= bitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (x.OlusturmaTarihi >= baslangicTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (x.OlusturmaTarihi <= bitisTarihi)))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList().Select(x => new
            {
                IadeTalepNo = x.IadeTalepId,
                Urun = x.SiparisDetay.Urun.Adi,
                Adet = x.Adet,
                Kullanici = x.SiparisDetay.Siparis.Kullanici.Eposta,
                SiparisNo = x.SiparisDetay.SiparisId,
                IstekTarihi = x.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm"),
                SonIslemTarihi = x.GuncellemeTarihi.ToString("dd.MM.yyyy HH:mm"),
                IadeIstekTip = x.IadeTalepIstekTip.Adi,
                IadeNedenTip = x.IadeTalepNedenTip.Adi,
                IslemDurumu = x.IadeTalepDurumTip.Adi,
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult Ara(int iadeTalepNo, int iadeTalepIstekTipId, int iadeTalepNedenTipId, int iadeTalepDurumTipId, DateTime? baslangicTarihi, DateTime? bitisTarihi, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = IadeTalepService.GetAll(true, new string[] { "IadeTalepDurumTip", "IadeTalepIstekTip", "IadeTalepNedenTip", "SiparisDetay", "SiparisDetay.Urun", "SiparisDetay.Siparis.Kullanici" }).Where(x =>
            (iadeTalepNo == 0 || x.IadeTalepId == iadeTalepNo) &&
            (iadeTalepIstekTipId == 0 || x.IadeTalepIstekTipId == iadeTalepIstekTipId) &&
            (iadeTalepNedenTipId == 0 || x.IadeTalepNedenTipId == iadeTalepNedenTipId) &&
            (iadeTalepDurumTipId == 0 || x.IadeTalepDurumTipId == iadeTalepDurumTipId) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                (((baslangicTarihi != null && bitisTarihi != null) && (x.OlusturmaTarihi >= baslangicTarihi && x.OlusturmaTarihi <= bitisTarihi))) ||
                (((baslangicTarihi != null && bitisTarihi == null) && (x.OlusturmaTarihi >= baslangicTarihi))) ||
                (((baslangicTarihi == null && bitisTarihi != null) && (x.OlusturmaTarihi <= bitisTarihi)))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.IadeTalepId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    IadeTalepId = x.IadeTalepId,
                    Urun = x.SiparisDetay.Urun.Adi,
                    Adet = x.Adet,
                    Kullanici = x.SiparisDetay.Siparis.Kullanici.Eposta,
                    SiparisNo = x.SiparisDetay.SiparisId,
                    IstekTarihi = x.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm"),
                    SonIslemTarihi = x.GuncellemeTarihi.ToString("dd.MM.yyyy HH:mm"),
                    IadeIstekTip = x.IadeTalepIstekTip.Adi,
                    IadeNedenTip = x.IadeTalepNedenTip.Adi,
                    IslemDurumu = x.IadeTalepDurumTip.Adi,
                    AktifMi = x.AktifMi
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IadeTalepGuncelle(IadeTalep iadeTalep)
        {
            try
            {
                if (iadeTalep == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _iadeTalep = IadeTalepService.FindBy(x => x.IadeTalepId == iadeTalep.IadeTalepId).SingleOrDefault();

                if (_iadeTalep == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _iadeTalep.IadeTalepDurumTipId = iadeTalep.IadeTalepDurumTipId;
                _iadeTalep.Adet = iadeTalep.Adet;
                _iadeTalep.PersonelAciklama = iadeTalep.PersonelAciklama;
                _iadeTalep.AktifMi = iadeTalep.AktifMi;
                _iadeTalep.GuncellemeTarihi = DateTime.Now;

                IadeTalepService.Edit(_iadeTalep);

                var flag = IadeTalepService.Save();

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

                var _iadeTalep = IadeTalepService.FindBy(x => x.IadeTalepId == id).SingleOrDefault();

                if (_iadeTalep == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _iadeTalep.AktifMi = false;

                IadeTalepService.Edit(_iadeTalep);

                var flag = IadeTalepService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
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