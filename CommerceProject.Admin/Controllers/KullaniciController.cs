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
using CommerceProject.Business.Helper.Cryptography;

namespace CommerceProject.Admin.Controllers
{
    public class KullaniciController : BaseController
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
        IRolService RolService;
        IKullaniciRolService KullaniciRolService;
        IKullaniciDetayService KullaniciDetayService;
        IKullaniciAdresService KullaniciAdresService;
        IAdresService AdresService;
        IAdresIlService AdresIlService;
        IAdresIlceService AdresIlceService;
        ISiparisService SiparisService;
        IIadeTalepService IadeTalepService;
        IUrunService UrunService;
        ISepetService SepetService;
        ISepetTipService SepetTipService;
        IIstekListesiService IstekListesiService;
        IFaturaTipService FaturaTipService;
        ISirketService SirketService;
        IYetkiService YetkiService;
        IKullaniciYetkiService KullaniciYetkiService;
        public KullaniciController(IIcerikAyarService iIcerikAyarService,
                                   IKullaniciService iKullaniciService,
                                   IRolService iRolService,
                                   IKullaniciRolService iKullaniciRolService,
                                   IKullaniciDetayService iKullaniciDetayService,
                                   IKullaniciAdresService iKullaniciAdresService,
                                   IAdresService iAdresService,
                                   IAdresIlService iAdresIlService,
                                   IAdresIlceService iAdresIlceService,
                                   ISiparisService iSiparisService,
                                   IIadeTalepService iIadeTalepService,
                                   IUrunService iUrunService,
                                   ISepetService iSepetService,
                                   ISepetTipService iSepetTipService,
                                   IIstekListesiService iIstekListesService,
                                   IFaturaTipService iFaturaTipService,
                                   ISirketService iSirketService,
                                   IYetkiService iYetkiService,
                                   IKullaniciYetkiService iKullaniciYetkiService) : base(iIcerikAyarService,
                                                                               iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            RolService = iRolService;
            KullaniciRolService = iKullaniciRolService;
            KullaniciDetayService = iKullaniciDetayService;
            KullaniciAdresService = iKullaniciAdresService;
            AdresService = iAdresService;
            AdresIlService = iAdresIlService;
            AdresIlceService = iAdresIlceService;
            SiparisService = iSiparisService;
            IadeTalepService = iIadeTalepService;
            UrunService = iUrunService;
            SepetService = iSepetService;
            SepetTipService = iSepetTipService;
            IstekListesiService = iIstekListesService;
            FaturaTipService = iFaturaTipService;
            SirketService = iSirketService;
            YetkiService = iYetkiService;
            KullaniciYetkiService = iKullaniciYetkiService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.kullanici_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Kullanıcı", "Arama", "Kullanıcı Arama İşlemleri", "");
            ViewBag.RolListesi = RolService.GetAll(true).ToList().Select(x => new { id = x.RolId, text = x.Adi, aktif = x.AktifMi }).ToList();

            return View();
        }

        [AuthorizeManager]
        public ActionResult Save(Guid? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.kullanici_save;

            ViewBag.RolListesi = RolService.GetAll(true).ToList().Select(x => new { id = x.RolId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.IlListesi = AdresIlService.GetAll(true).ToList().Select(x => new { id = x.AdresIlId, text = x.IlAdi, aktif = x.AktifMi }).ToList();
            ViewBag.IlceListesi = AdresIlceService.GetAll(true).ToList().Select(x => new { id = x.AdresIlceId, text = x.IlceAdi, ilId = x.AdresIlId, aktif = x.AktifMi }).ToList();
            ViewBag.SepetTipListesi = SepetTipService.GetAll(true).ToList().Select(x => new { id = x.SepetTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.FaturaTipListesi = FaturaTipService.GetAll(true).ToList().Select(x => new { id = x.FaturaTipId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.SirketListesi = SirketService.GetAll(true).ToList().Select(x => new { id = x.SirketId, text = x.Adi, aktif = x.AktifMi }).ToList();
            ViewBag.YetkiListesi = YetkiService.FindBy(x => x.AktifMi == true).ToList();

            if (id == null)
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Kullanıcı", "Kayıt", "Kullanıcı Kayıt İşlemleri", "");

                ViewBag.Kullanici = new Kullanici();
                ViewBag.KullaniciDetay = new KullaniciDetay();
                ViewBag.KullaniciRolListesi = string.Empty;
                ViewBag.KullaniciSiparisListesi = new List<Siparis>()
                {
                    new Siparis()
                    {
                        SiparisOdemeTip = new SiparisOdemeTip(),
                        SiparisDurumTip = new SiparisDurumTip(),
                        OdemeDurumTip = new OdemeDurumTip()
                    }
                };
                ViewBag.KullaniciIadeTalepListesi = new List<IadeTalep>()
                {
                    new IadeTalep()
                    {
                        IadeTalepDurumTip = new IadeTalepDurumTip(),
                        IadeTalepIstekTip = new IadeTalepIstekTip(),
                        IadeTalepNedenTip = new IadeTalepNedenTip(),
                        SiparisDetay = new SiparisDetay()
                        {
                            Siparis = new Siparis()
                        }
                    }
                };
                ViewBag.KullaniciIstekListesi = new List<IstekListesi>();
                ViewBag.KullaniciAdresListesi = new List<KullaniciAdres>();
                ViewBag.KullaniciYetkiListesi = new List<KullaniciYetki>();
            }
            else
            {
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Kullanıcı", "Güncelleme", "Kullanıcı Güncelleme İşlemleri", "");

                ViewBag.Kullanici = KullaniciService.FindBy(x => x.KullaniciId == id).SingleOrDefault();
                ViewBag.KullaniciDetay = KullaniciDetayService.GetSingle(x => x.KullaniciDetayId == id);
                ViewBag.KullaniciRolListesi = KullaniciRolService.FindBy(x => x.KullaniciId == id && x.AktifMi == true).ToList().Select(x => x.RolId).ToList();
                ViewBag.KullaniciSiparisListesi = SiparisService.FindBy(x => x.KullaniciId == id, false, new string[] { "SiparisOdemeTip", "SiparisDurumTip", "OdemeDurumTip" }).ToList();
                ViewBag.KullaniciIadeTalepListesi = IadeTalepService.GetAll(false, new string[] { "IadeTalepDurumTip", "IadeTalepIstekTip", "IadeTalepNedenTip", "SiparisDetay", "SiparisDetay.Siparis" }).ToList()
                    .Where(x => x.SiparisDetay.Siparis.KullaniciId == id).ToList();
                ViewBag.KullaniciIstekListesi = IstekListesiService.FindBy(x => x.KullaniciId == id, true, new string[] { "Urun" }).ToList();
                ViewBag.KullaniciAdresListesi = KullaniciAdresService.FindBy(x => x.KullaniciId == id, true, new string[] { "Adres", "Adres.FaturaTip", "Adres.AdresIl", "Adres.AdresIlce" }).ToList().
                    Select(x => new KullaniciAdres()
                    {
                        AdresId = x.AdresId,
                        AktifMi = x.AktifMi,
                        KullaniciAdresId = x.KullaniciAdresId,
                        KullaniciId = x.KullaniciId,
                        GuncellemeTarihi = x.GuncellemeTarihi,
                        Kullanici = null,
                        OlusturmaTarihi = x.OlusturmaTarihi,
                        Sira = x.Sira,
                        Adres = new Adres()
                        {
                            Aciklama = x.Adres.Aciklama,
                            Ad = x.Adres.Ad,
                            AdresBilgi = x.Adres.AdresBilgi,
                            AdresId = x.Adres.AdresId,
                            AdresIl = new AdresIl()
                            {
                                AdresIlId = x.Adres.AdresIl.AdresIlId,
                                AktifMi = x.Adres.AdresIl.AktifMi,
                                IlAdi = x.Adres.AdresIl.IlAdi,
                                Adres = null,
                                AdresIlce = null
                            },
                            AdresIlce = new AdresIlce()
                            {
                                AdresIlceId = x.Adres.AdresIlce.AdresIlceId,
                                AdresIlId = x.Adres.AdresIlce.AdresIlId,
                                AktifMi = x.Adres.AdresIlce.AktifMi,
                                IlceAdi = x.Adres.AdresIlce.IlceAdi,
                                Adres = null,
                                AdresIl = null
                            },
                            FaturaTip = new FaturaTip()
                            {
                                Adi = x.Adres.FaturaTip.Adi,
                                AktifMi = x.Adres.FaturaTip.AktifMi,
                                FaturaTipId = x.Adres.FaturaTip.FaturaTipId,
                                Sira = x.Adres.FaturaTip.Sira
                            },
                            AdresAdi = x.Adres.AdresAdi,
                            FaturaTipId = x.Adres.FaturaTipId,
                            FirmaUnvan = x.Adres.FirmaUnvan,
                            VergiDairesi = x.Adres.VergiDairesi,
                            AdresIlceId = x.Adres.AdresIlceId,
                            AdresIlId = x.Adres.AdresIlId,
                            AktifMi = x.Adres.AktifMi,
                            PostaKodu = x.Adres.PostaKodu,
                            Soyad = x.Adres.Soyad,
                            Tarih = x.Adres.Tarih,
                            TcNo = x.Adres.TcNo,
                            Telefon1 = x.Adres.Telefon1,
                            Telefon2 = x.Adres.Telefon2,
                            VergiNo = x.Adres.VergiNo,
                            Siparis = null,
                            Siparis1 = null
                        }
                    }).ToList();
                ViewBag.KullaniciYetkiListesi = KullaniciYetkiService.FindBy(x => x.AktifMi == true && x.KullaniciId == id).ToList();
            }

            return View();
        }

        public ActionResult KullaniciDetayPartial(Guid id)
        {
            ViewBag.Kullanici = KullaniciService.GetSingle(x => x.KullaniciId == id, true, new string[] { "KullaniciDetay", "KullaniciDetay.Sirket" });

            return PartialView("~/Views/Kullanici/Partials/KullaniciDetayPartial.cshtml");
        }

        public ActionResult ExcelRaporuAl(string adi, string soyadi, string eposta, DateTime? uyelikBaslangicTarihi, DateTime? uyelikBitisTarihi, DateTime? sonGirisBaslangicTarihi, DateTime? sonGirisBitisTarihi, int rolId, int aktiflik)
        {
            var sonucListesi = KullaniciService.FindBy(x =>
            ((string.IsNullOrEmpty(adi)) || (x.KullaniciDetay != null && x.KullaniciDetay.Ad.Contains(adi))) &&
            ((string.IsNullOrEmpty(soyadi)) || (x.KullaniciDetay != null && x.KullaniciDetay.Soyad.Contains(soyadi))) &&
            ((string.IsNullOrEmpty(eposta)) || (x.Eposta.Contains(eposta))) &&
            (
                (uyelikBaslangicTarihi == null && uyelikBitisTarihi == null) ||
                (uyelikBaslangicTarihi != null && uyelikBitisTarihi == null && uyelikBaslangicTarihi <= x.UyelikTarihi) ||
                (uyelikBaslangicTarihi == null && uyelikBitisTarihi != null && uyelikBitisTarihi >= x.UyelikTarihi) ||
                (uyelikBaslangicTarihi != null && uyelikBitisTarihi != null && (uyelikBaslangicTarihi <= x.UyelikTarihi && uyelikBitisTarihi >= x.UyelikTarihi))
            ) &&
            (
                (sonGirisBaslangicTarihi == null && sonGirisBitisTarihi == null) ||
                (sonGirisBaslangicTarihi != null && sonGirisBitisTarihi == null && sonGirisBaslangicTarihi <= x.SonGirisTarihi) ||
                (sonGirisBaslangicTarihi == null && sonGirisBitisTarihi != null && sonGirisBitisTarihi >= x.SonGirisTarihi) ||
                (sonGirisBaslangicTarihi != null && sonGirisBitisTarihi != null && (sonGirisBaslangicTarihi <= x.SonGirisTarihi && sonGirisBitisTarihi >= x.SonGirisTarihi))
            ) &&
            ((rolId == 0) || (x.KullaniciRol.Any(u => u.RolId == rolId))) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
             , true, new string[] { "KullaniciDetay", "KullaniciDetay.Sirket", "KullaniciRol" }).ToList().Select(x => new
             {
                 KullaniciId = x.KullaniciId,
                 Adi = x.KullaniciDetay != null ? x.KullaniciDetay.Ad : "?",
                 Soyadi = x.KullaniciDetay != null ? x.KullaniciDetay.Soyad : "?",
                 Eposta = x.Eposta,
                 Sirket = x.KullaniciDetay.Sirket != null ? x.KullaniciDetay.Sirket.Adi : "-",
                 UyelikTarihi = x.UyelikTarihi.ToString("dd.MM.yyyy HH:mm"),
                 SonGirisTarihi = x.SonGirisTarihi.ToString("dd.MM.yyyy HH:mm"),
                 AktifMi = x.AktifMi
             }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }

        public ActionResult SepetHareketExcelRaporuAl(Guid kullaniciId, int sepetTipId, int urunId, int aktiflik)
        {
            var sonucListesi = SepetService.FindBy(x =>
            (
            (x.KullaniciId == kullaniciId) &&
            (sepetTipId == 0 || x.SepetTipId == sepetTipId) &&
            (urunId == 0 || x.UrunId == urunId) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ), true, new string[] { "SepetTip", "Urun" }).ToList().Select(x => new
            {
                SepetTipi = x.SepetTip.Adi,
                Tarih = x.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm"),
                Urun = x.Urun.Adi,
                Miktar = x.Adet,
                BirimFiyat = x.Urun.Fiyat,
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult Sil(Guid id)
        {
            try
            {
                if (id == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _kullanici = KullaniciService.FindBy(x => x.KullaniciId == id).SingleOrDefault();

                if (_kullanici == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _kullanici.AktifMi = false;

                KullaniciService.Edit(_kullanici);

                var flag = KullaniciService.Save();

                if (!flag)
                    return Json(false, JsonRequestBehavior.DenyGet);

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult Ara(string adi, string soyadi, string eposta, DateTime? uyelikBaslangicTarihi, DateTime? uyelikBitisTarihi, DateTime? sonGirisBaslangicTarihi, DateTime? sonGirisBitisTarihi, int rolId, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            List<Guid> engellenenKullanicilar = new List<Guid>();
            engellenenKullanicilar.Add(Guid.Empty);
            engellenenKullanicilar.Add(Guid.Parse("fe3e6535-b81c-483e-ae2f-a116a1128c8a"));

            var tempList = KullaniciService.FindBy(x =>
            ((string.IsNullOrEmpty(adi)) || (x.KullaniciDetay != null && x.KullaniciDetay.Ad.Contains(adi))) &&
            ((string.IsNullOrEmpty(soyadi)) || (x.KullaniciDetay != null && x.KullaniciDetay.Soyad.Contains(soyadi))) &&
            ((string.IsNullOrEmpty(eposta)) || (x.Eposta.Contains(eposta))) &&
            (
                (uyelikBaslangicTarihi == null && uyelikBitisTarihi == null) ||
                (uyelikBaslangicTarihi != null && uyelikBitisTarihi == null && uyelikBaslangicTarihi <= x.UyelikTarihi) ||
                (uyelikBaslangicTarihi == null && uyelikBitisTarihi != null && uyelikBitisTarihi >= x.UyelikTarihi) ||
                (uyelikBaslangicTarihi != null && uyelikBitisTarihi != null && (uyelikBaslangicTarihi <= x.UyelikTarihi && uyelikBitisTarihi >= x.UyelikTarihi))
            ) &&
            (
                (sonGirisBaslangicTarihi == null && sonGirisBitisTarihi == null) ||
                (sonGirisBaslangicTarihi != null && sonGirisBitisTarihi == null && sonGirisBaslangicTarihi <= x.SonGirisTarihi) ||
                (sonGirisBaslangicTarihi == null && sonGirisBitisTarihi != null && sonGirisBitisTarihi >= x.SonGirisTarihi) ||
                (sonGirisBaslangicTarihi != null && sonGirisBitisTarihi != null && (sonGirisBaslangicTarihi <= x.SonGirisTarihi && sonGirisBitisTarihi >= x.SonGirisTarihi))
            ) &&
            ((rolId == 0) || (x.KullaniciRol.Any(u => u.RolId == rolId))) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))) &&
            !engellenenKullanicilar.Contains(x.KullaniciId)
             , true, new string[] { "KullaniciDetay", "KullaniciDetay.Sirket", "KullaniciRol" });

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.KullaniciId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    KullaniciId = x.KullaniciId,
                    Adi = x.KullaniciDetay != null ? x.KullaniciDetay.Ad : "?",
                    Soyadi = x.KullaniciDetay != null ? x.KullaniciDetay.Soyad : "?",
                    Eposta = x.Eposta,
                    Sirket = x.KullaniciDetay.Sirket != null ? x.KullaniciDetay.Sirket.Adi : "-",
                    UyelikTarihi = x.UyelikTarihi.ToString("dd.MM.yyyy HH:mm"),
                    SonGirisTarihi = x.SonGirisTarihi.ToString("dd.MM.yyyy HH:mm"),
                    AktifMi = x.AktifMi
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SepetHareketAra(Guid kullaniciId, int sepetTipId, int urunId, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = SepetService.FindBy(x =>
            (
            (x.KullaniciId == kullaniciId) &&
            (sepetTipId == 0 || x.SepetTipId == sepetTipId) &&
            (urunId == 0 || x.UrunId == urunId) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ), true, new string[] { "SepetTip", "Urun" });

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.SepetId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    SepetTipi = x.SepetTip.Adi,
                    Tarih = x.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm"),
                    Urun = x.Urun.Adi,
                    Miktar = x.Adet,
                    BirimFiyat = x.Urun.Fiyat,
                    AktifMi = x.AktifMi
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult KullaniciyaAitRolleriGetir(Guid id)
        {
            var roller = (from kr in KullaniciRolService.GetAll(true).ToList()
                          join r in RolService.GetAll(true).ToList() on kr.RolId equals r.RolId
                          where kr.KullaniciId == id
                          select new
                          {
                              RolAdi = r.Adi,
                              AktifMi = r.AktifMi
                          }).ToList();

            return Json(new
            {
                rolListesi = roller
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult KullaniciBilgiKaydetGuncelle(Kullanici kullaniciBilgi)
        {
            try
            {
                Kullanici _kullanici;

                if (kullaniciBilgi.KullaniciId == Guid.Empty)
                {
                    _kullanici = new Kullanici();

                    _kullanici.KullaniciId = Guid.NewGuid();
                    _kullanici.Eposta = kullaniciBilgi.Eposta;
                    _kullanici.Sifre = CryptoHelper.Encode(kullaniciBilgi.Sifre);
                    _kullanici.UyelikTarihi = DateTime.Now;
                    _kullanici.SonGirisTarihi = DateTime.Now;
                    _kullanici.BasarisizGirisSayisi = 0;
                    _kullanici.HesapKilitliMi = kullaniciBilgi.HesapKilitliMi;
                    _kullanici.EpostaOnayliMi = kullaniciBilgi.EpostaOnayliMi;
                    _kullanici.AktifMi = kullaniciBilgi.AktifMi;

                    KullaniciDetay _kullaniciDetay = new KullaniciDetay
                    {
                        KullaniciDetayId = _kullanici.KullaniciId,
                        Ad = "",
                        Soyad = "",
                        KullaniciResim = null,
                        SepetteUrunVarMi = false
                    };
                    _kullanici.KullaniciDetay = _kullaniciDetay;

                    KullaniciService.Add(_kullanici);
                }
                else
                {
                    _kullanici = KullaniciService.FindBy(x => x.KullaniciId == kullaniciBilgi.KullaniciId).SingleOrDefault();

                    if (_kullanici == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _kullanici.Eposta = kullaniciBilgi.Eposta;
                    _kullanici.Sifre = CryptoHelper.Encode(kullaniciBilgi.Sifre);
                    _kullanici.HesapKilitliMi = kullaniciBilgi.HesapKilitliMi;
                    _kullanici.EpostaOnayliMi = kullaniciBilgi.EpostaOnayliMi;
                    _kullanici.AktifMi = kullaniciBilgi.AktifMi;

                    KullaniciService.Edit(_kullanici);
                }

                var flag = KullaniciService.Save();

                if (flag)
                    return Json(_kullanici.KullaniciId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult KullaniciDetayKaydetGuncelle(Guid kullaniciId, string ad, string soyad, bool kullaniciResimVarmi, string roller, int? sirketId)
        {
            try
            {
                var _kullaniciDetay = KullaniciDetayService.FindBy(x => x.KullaniciDetayId == kullaniciId).SingleOrDefault();

                if (_kullaniciDetay == null)
                {
                    _kullaniciDetay = new KullaniciDetay();
                    _kullaniciDetay.KullaniciDetayId = kullaniciId;
                    _kullaniciDetay.Ad = ad;
                    _kullaniciDetay.Soyad = soyad;
                    _kullaniciDetay.SirketId = sirketId;

                    if (Request.Files.Count == 0)
                    {
                        _kullaniciDetay.KullaniciResim = null;
                    }
                    else
                    {
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                        {
                            fileData = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                        }
                        _kullaniciDetay.KullaniciResim = fileData;
                    }

                    _kullaniciDetay.SepetteUrunVarMi = false;

                    KullaniciDetayService.Add(_kullaniciDetay);
                }
                else
                {
                    _kullaniciDetay.Ad = ad;
                    _kullaniciDetay.Soyad = soyad;
                    _kullaniciDetay.SirketId = sirketId;

                    if (!kullaniciResimVarmi)
                    {
                        if (Request.Files.Count == 0)
                        {
                            _kullaniciDetay.KullaniciResim = null;
                        }
                        else
                        {
                            byte[] fileData = null;
                            using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                            {
                                fileData = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                            }
                            _kullaniciDetay.KullaniciResim = fileData;
                        }
                    }

                    KullaniciDetayService.Edit(_kullaniciDetay);
                }

                var flag = KullaniciDetayService.Save();

                // Roller
                if (string.IsNullOrEmpty(roller))
                {
                    var _kullaniciRoller = KullaniciRolService.FindBy(x => x.KullaniciId == kullaniciId && x.AktifMi == true).ToList();

                    foreach (var kRol in _kullaniciRoller)
                    {
                        var _kr = KullaniciRolService.FindBy(x => x.KullaniciRolId == kRol.KullaniciRolId).SingleOrDefault();
                        if (_kr != null)
                        {
                            _kr.AktifMi = false;
                            KullaniciRolService.Edit(_kr);
                        }
                    }
                    KullaniciRolService.Save();
                }
                else
                {
                    var rolList = roller.Split(',');
                    var _kullaniciRoller = KullaniciRolService.FindBy(x => x.KullaniciId == kullaniciId && x.AktifMi == true).ToList();

                    // yeni eklenen
                    for (int i = 0; i < rolList.Count(); i++)
                    {
                        if (!_kullaniciRoller.Any(x => x.RolId.ToString() == rolList[i]))
                        {
                            var _kRol = new KullaniciRol()
                            {
                                AktifMi = true,
                                KullaniciId = kullaniciId,
                                RolId = Convert.ToInt32(rolList[i])
                            };
                            KullaniciRolService.Add(_kRol);
                        }
                    }
                    // silinen
                    for (int i = 0; i < _kullaniciRoller.Count(); i++)
                    {
                        if (!rolList.Contains(_kullaniciRoller[i].RolId.ToString()))
                        {
                            var id = _kullaniciRoller[i].KullaniciRolId;
                            var _kRol = KullaniciRolService.FindBy(x => x.KullaniciRolId == id).SingleOrDefault();
                            if (_kRol != null)
                            {
                                _kRol.AktifMi = false;

                                KullaniciRolService.Edit(_kRol);
                            }
                        }
                    }
                    KullaniciRolService.Save();
                }

                if (flag)
                    return Json(_kullaniciDetay.KullaniciDetayId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult KullaniciAdresKaydetGuncelle(List<KullaniciAdres> adresListesi)
        {
            try
            {
                foreach (var adres in adresListesi)
                {
                    if (adres.KullaniciAdresId > 0)
                    {
                        var _adres = AdresService.FindBy(x => x.AdresId == adres.AdresId).SingleOrDefault();

                        if (_adres != null)
                        {
                            _adres.Aciklama = adres.Adres.Aciklama;
                            _adres.FaturaTipId = adres.Adres.FaturaTipId;
                            _adres.AdresAdi = adres.Adres.AdresAdi;
                            _adres.Ad = adres.Adres.Ad;
                            _adres.AdresBilgi = adres.Adres.AdresBilgi;
                            _adres.AdresIlceId = adres.Adres.AdresIlceId;
                            _adres.AdresIlId = adres.Adres.AdresIlId;
                            _adres.AktifMi = adres.Adres.AktifMi;
                            _adres.PostaKodu = adres.Adres.PostaKodu;
                            _adres.FirmaUnvan = adres.Adres.FirmaUnvan;
                            _adres.Soyad = adres.Adres.Soyad;
                            _adres.TcNo = adres.Adres.TcNo;
                            _adres.Telefon1 = adres.Adres.Telefon1;
                            _adres.Telefon2 = adres.Adres.Telefon2;
                            _adres.VergiDairesi = adres.Adres.VergiDairesi;
                            _adres.VergiNo = adres.Adres.VergiNo;

                            AdresService.Edit(_adres);

                            AdresService.Save();

                            var _kullaniciAdres = KullaniciAdresService.FindBy(x => x.KullaniciAdresId == adres.KullaniciAdresId).SingleOrDefault();

                            if (_kullaniciAdres != null)
                            {
                                _kullaniciAdres.GuncellemeTarihi = DateTime.Now;

                                KullaniciAdresService.Edit(_kullaniciAdres);

                                KullaniciAdresService.Save();
                            }
                        }
                    }
                    else
                    {
                        var _adres = new Adres();
                        _adres.Aciklama = adres.Adres.Aciklama;
                        _adres.FaturaTipId = adres.Adres.FaturaTipId;
                        _adres.AdresAdi = adres.Adres.AdresAdi;
                        _adres.Ad = adres.Adres.Ad;
                        _adres.AdresBilgi = adres.Adres.AdresBilgi;
                        _adres.AdresIlceId = adres.Adres.AdresIlceId;
                        _adres.AdresIlId = adres.Adres.AdresIlId;
                        _adres.AktifMi = adres.Adres.AktifMi;
                        _adres.PostaKodu = adres.Adres.PostaKodu;
                        _adres.FirmaUnvan = adres.Adres.FirmaUnvan;
                        _adres.Soyad = adres.Adres.Soyad;
                        _adres.Tarih = DateTime.Now;
                        _adres.TcNo = adres.Adres.TcNo;
                        _adres.Telefon1 = adres.Adres.Telefon1;
                        _adres.Telefon2 = adres.Adres.Telefon2;
                        _adres.VergiDairesi = adres.Adres.VergiDairesi;
                        _adres.VergiNo = adres.Adres.VergiNo;

                        AdresService.Add(_adres);

                        var flag = AdresService.Save();

                        if (flag)
                        {
                            var _kullaniciAdres = new KullaniciAdres();
                            _kullaniciAdres.AdresId = _adres.AdresId;
                            _kullaniciAdres.AktifMi = true;
                            _kullaniciAdres.GuncellemeTarihi = DateTime.Now;
                            _kullaniciAdres.KullaniciId = adres.KullaniciId;
                            _kullaniciAdres.OlusturmaTarihi = DateTime.Now;
                            _kullaniciAdres.Sira = 1;

                            KullaniciAdresService.Add(_kullaniciAdres);

                            KullaniciAdresService.Save();
                        }
                    }
                }

                return Json(true, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult KullaniciYetkiKaydetGuncelle(Guid kullaniciId, int[] yetkiler)
        {
            try
            {
                var kullaniciYetkiList = KullaniciYetkiService.FindBy(x => x.KullaniciId == kullaniciId).ToList();
                foreach (var ky in kullaniciYetkiList)
                {
                    ky.AktifMi = false;
                    KullaniciYetkiService.Edit(ky);
                }
                KullaniciYetkiService.Save();

                // Gelen yetkilere bak, varsa aktifleştir, yoksa yeni ekle
                KullaniciYetki _kullaniciYetki;
                foreach (var yetkiId in yetkiler)
                {
                    _kullaniciYetki = KullaniciYetkiService.GetSingle(x => x.KullaniciId == kullaniciId && x.YetkiId == yetkiId);
                    if (_kullaniciYetki != null)
                    {
                        _kullaniciYetki.AktifMi = true;
                        KullaniciYetkiService.Edit(_kullaniciYetki);
                    }
                    else
                    {
                        _kullaniciYetki = new KullaniciYetki();
                        _kullaniciYetki.AktifMi = true;
                        _kullaniciYetki.KullaniciId = kullaniciId;
                        _kullaniciYetki.YetkiId = yetkiId;

                        KullaniciYetkiService.Add(_kullaniciYetki);
                    }
                }

                var flag = KullaniciYetkiService.Save();

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