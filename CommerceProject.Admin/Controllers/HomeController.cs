using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.BusinessContracts;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Business.Entities;
using System.Data;
using CommerceProject.Business.Custom;
using static CommerceProject.Admin.Helper.PageHelper;
using System.Data.Entity;
using CommerceProject.Admin.Helper;
using CommerceProject.Admin.Models;

namespace CommerceProject.Admin.Controllers
{
    public class HomeController : BaseController
    {
        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        ISiparisService SiparisService;
        ISiparisDetayService SiparisDetayService;
        IIadeTalepService IadeTalepService;
        IUrunService UrunService;
        IUrunYorumService UrunYorumService;
        IStokHareketService StokHareketService;
        public HomeController(IIcerikAyarService iIcerikAyarService,
                              IKullaniciService iKullaniciService,
                              ISiparisService iSiparisService,
                              ISiparisDetayService iSiparisDetayService,
                              IIadeTalepService iIadeTalepService,
                              IUrunService iUrunService,
                              IUrunYorumService iUrunYorumService,
                              IStokHareketService iStokHareketService) : base(iIcerikAyarService,
                                                                          iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            SiparisService = iSiparisService;
            SiparisDetayService = iSiparisDetayService;
            IadeTalepService = iIadeTalepService;
            UrunService = iUrunService;
            UrunYorumService = iUrunYorumService;
            StokHareketService = iStokHareketService;
        }

        #region Actions
        [AuthorizeManager]
        [ActionManager]
        public ActionResult Index()
        {
            ViewBag.PageName = Pages.home_index;
            ViewBag.PageProperties = PageProperties.SetPageProperties("Anasayfa", "", "Kontrol Paneli", "");
            ViewBag.Kullanici = PageProperties.LoggedInUser(KullaniciService);

            ViewBag.YeniSiparisSayisi = SiparisService.FindBy(x => x.AktifMi == true && x.SiparisDurumTipId == 1).Count();
            ViewBag.YeniIadeTalepSayisi = IadeTalepService.FindBy(x => x.AktifMi == true && x.IadeTalepDurumTipId == 1).Count();
            ViewBag.OnayliKullaniciSayisi = KullaniciService.FindBy(x => x.AktifMi == true && x.EpostaOnayliMi == true && x.HesapKilitliMi == false).Count();

            var kullaniciDictionary = new Dictionary<string, List<Kullanici>>();
            var son10OnayliKullanici = KullaniciService.FindBy(x => x.AktifMi == true && x.EpostaOnayliMi == true && x.HesapKilitliMi == false,
                true, new string[] { "KullaniciDetay" }).OrderByDescending(x => x.UyelikTarihi).Take(10).ToList();
            var son10OnayBekleyenKullanici = KullaniciService.FindBy(x => x.AktifMi == true && x.EpostaOnayliMi == false && x.HesapKilitliMi == false,
                true, new string[] { "KullaniciDetay" }).OrderByDescending(x => x.UyelikTarihi).Take(10).ToList();
            var son10HesabiKilitliKullanici = KullaniciService.FindBy(x => x.AktifMi == true && x.HesapKilitliMi == true,
                true, new string[] { "KullaniciDetay" }).OrderByDescending(x => x.UyelikTarihi).Take(10).ToList();

            kullaniciDictionary.Add("onayli", son10OnayliKullanici);
            kullaniciDictionary.Add("bekleyen", son10OnayBekleyenKullanici);
            kullaniciDictionary.Add("kilitli", son10HesabiKilitliKullanici);

            ViewBag.Son10KullaniciBilgisi = kullaniciDictionary;

            var siparisDictionary = new Dictionary<string, List<Siparis>>();
            var son10BeklemedeSiparis = SiparisService.FindBy(x => x.AktifMi == true && x.SiparisDurumTipId == 1,
                true, new string[] { "SiparisDetay", "Kullanici", "Kullanici.KullaniciDetay" }).OrderByDescending(x => x.SiparisTarihi).Take(10).ToList();
            var son10OnayBekleyenSiparis = SiparisService.FindBy(x => x.AktifMi == true && x.SiparisDurumTipId == 2,
                true, new string[] { "SiparisDetay", "Kullanici", "Kullanici.KullaniciDetay" }).OrderByDescending(x => x.SiparisTarihi).Take(10).ToList();
            var son10OnaylananSiparis = SiparisService.FindBy(x => x.AktifMi == true && x.SiparisDurumTipId == 3,
                true, new string[] { "SiparisDetay", "Kullanici", "Kullanici.KullaniciDetay" }).OrderByDescending(x => x.SiparisTarihi).Take(10).ToList();
            var son10GonderilenSiparis = SiparisService.FindBy(x => x.AktifMi == true && x.SiparisDurumTipId == 4,
                true, new string[] { "SiparisDetay", "Kullanici", "Kullanici.KullaniciDetay" }).OrderByDescending(x => x.SiparisTarihi).Take(10).ToList();

            siparisDictionary.Add("beklemede", son10BeklemedeSiparis);
            siparisDictionary.Add("onayBekleyen", son10OnayBekleyenSiparis);
            siparisDictionary.Add("onaylanan", son10OnaylananSiparis);
            siparisDictionary.Add("gonderilen", son10GonderilenSiparis);

            ViewBag.Son10SiparisBilgisi = siparisDictionary;

            var iadeTalepDictionary = new Dictionary<string, List<IadeTalep>>();
            var son10BeklemedeIadeTalep = IadeTalepService.FindBy(x => x.AktifMi == true && x.IadeTalepDurumTipId == 1,
                true, new string[] { "SiparisDetay", "SiparisDetay.Urun" }).OrderByDescending(x => x.OlusturmaTarihi).Take(10).ToList();
            var son10IncelenenIadeTalep = IadeTalepService.FindBy(x => x.AktifMi == true && x.IadeTalepDurumTipId == 2,
                true, new string[] { "SiparisDetay", "SiparisDetay.Urun" }).OrderByDescending(x => x.OlusturmaTarihi).Take(10).ToList();
            var son10SonuclananIadeTalep = IadeTalepService.FindBy(x => x.AktifMi == true && x.IadeTalepDurumTipId == 3,
                true, new string[] { "SiparisDetay", "SiparisDetay.Urun" }).OrderByDescending(x => x.OlusturmaTarihi).Take(10).ToList();

            iadeTalepDictionary.Add("beklemede", son10BeklemedeIadeTalep);
            iadeTalepDictionary.Add("incelenen", son10IncelenenIadeTalep);
            iadeTalepDictionary.Add("sonuclanan", son10SonuclananIadeTalep);

            ViewBag.Son10IadeTalepBilgisi = iadeTalepDictionary;

            ViewBag.BekleyenUrunYorumlari = UrunYorumService.FindBy(x => x.AktifMi == true && x.GosterilsinMi == false,
                true, new string[] { "Kullanici", "Kullanici.KullaniciDetay", "Urun" }).ToList();

            ViewBag.BekleyenSiparisler = SiparisService.FindBy(x => x.AktifMi == true && x.SiparisDurumTipId == 1,
                true, new string[] { "SiparisDetay", "Kullanici", "Kullanici.KullaniciDetay" }).ToList();

            //var urunListesi = UrunService.FindBy(x => x.AktifMi == true).ToList();
            var stokGirisHareketleri = StokHareketService.FindBy(x => x.AktifMi == true && x.StokHareketTipId == 1).GroupBy(x => x.UrunId).Select(x => new { UrunId = x.Key, ToplamAdet = x.Sum(u => u.Miktar) })
                .ToDictionary(x => x.UrunId, x => x.ToplamAdet);
            var stokCikisHareketleri = StokHareketService.FindBy(x => x.AktifMi == true && x.StokHareketTipId == 2).GroupBy(x => x.UrunId).Select(x => new { UrunId = x.Key, ToplamAdet = x.Sum(u => u.Miktar) })
                .ToDictionary(x => x.UrunId, x => x.ToplamAdet);

            //var urunStokListesi = UrunService.FindBy(x => x.AktifMi == true).ToList().GroupBy(x => x.UrunId)
            //    .Select(x => new UrunDusukStokDataObj()
            //    {
            //        UrunId = x.Key,
            //        StokAlarmSeviyesi = urunListesi.First(u => u.UrunId == x.Key).StokAlarmSeviyesi,
            //        ToplamGirisAdedi = stokGirisHareketleri.Any(u => u.Key == x.Key) ? stokGirisHareketleri[x.Key] : 0,
            //        ToplamCikisAdedi = stokCikisHareketleri.Any(u => u.Key == x.Key) ? stokCikisHareketleri[x.Key] : 0
            //    }).ToList();

            //decimal dusukStokUrunAdedi = 0;
            //foreach (var urunstok in urunStokListesi)
            //{
            //    if(urunstok.ToplamGirisAdedi - urunstok.ToplamCikisAdedi <= urunstok.StokAlarmSeviyesi)
            //    {
            //        dusukStokUrunAdedi++;
            //    }
            //}
            //ViewBag.StokDusukUrun = dusukStokUrunAdedi;
            //ViewBag.StokToplamUrun = urunStokListesi.Count;



            //decimal dusukStokUrunAdedi = 0;
            //decimal stokGiris = 0;
            //decimal stokCikis = 0;

            //foreach (var urun in urunListesi)
            //{
            //    if(stokGirisHareketleri.ContainsKey(urun.UrunId) && stokCikisHareketleri.ContainsKey(urun.UrunId))
            //    {
            //        stokGiris = stokGirisHareketleri[urun.UrunId];
            //        stokCikis = stokCikisHareketleri[urun.UrunId];

            //        if((stokGiris - stokCikis) <= urun.StokAlarmSeviyesi)
            //        {
            //            dusukStokUrunAdedi++;
            //        }
            //    }
            //}
            //ViewBag.DusukStokBilgisi = dusukStokUrunAdedi + " / " + urunListesi.Count + " (%" + ((100 * dusukStokUrunAdedi / urunListesi.Count)).ToString("#.##") + ")";

            return View();
        }

        [AuthorizeManager]
        public ActionResult NotAuthorize()
        {
            ViewBag.PageName = Pages.home_index;
            ViewBag.PageProperties = PageProperties.SetPageProperties("Anasayfa", "", "Yetki Yok", "");
            ViewBag.Kullanici = PageProperties.LoggedInUser(KullaniciService);           

            return View();
        }

        public ActionResult SiparisAdetRaporPartial(string dateType)
        {
            var date = DateTime.Now.Date;
            var count = 0;
            var siparisForDictionary = new Dictionary<string, int>();
            var siparisBilgiDictionary = new Dictionary<string, decimal>();

            switch (dateType)
            {
                // Bugün için sipariş toplamı
                case "day":
                    var siparisForToday = SiparisService.FindBy(x => x.AktifMi == true && x.SiparisDurumTipId != 8 && x.SiparisDurumTipId != 9 && DbFunctions.TruncateTime(x.SiparisTarihi) == date, true).ToList();

                    for (int i = 0; i < 6; i++)
                    {
                        count = siparisForToday.Where(x => x.SiparisTarihi >= date && x.SiparisTarihi <= date.AddHours(4)).ToList().Count();

                        siparisForDictionary.Add(date.ToString("HH:mm") + "-" + date.AddHours(4).ToString("HH:mm"), count);
                        date = date.AddHours(4);
                    }
                    ViewBag.SiparisChartData = siparisForDictionary.Select(x => new { date = x.Key, value = x.Value }).ToList();
                    ViewBag.CategoryAxisTitle = "Saat Aralığı";

                    siparisBilgiDictionary.Add("siparis", siparisForToday.Sum(x => x.OdenecekTutar));
                    siparisBilgiDictionary.Add("komisyon", siparisForToday.Sum(x => x.ToplamKomisyon));
                    siparisBilgiDictionary.Add("iskonto", siparisForToday.Sum(x => x.ToplamIskonto));
                    siparisBilgiDictionary.Add("iade", siparisForToday.Sum(x => x.IadeToplam));

                    break;
                // Bu hafta için sipariş toplamı
                case "week":
                    // 0- Pazartesi .... 6- Pazar
                    var dayOfWeek = (((int)date.DayOfWeek == 0) ? 7 : (int)date.DayOfWeek) - 1;

                    var firstDateOfWeek = new DateTime(date.Year, date.Month, date.AddDays((-1) * dayOfWeek).Day, 0, 0, 0, 0); //date.AddDays((-1) * dayOfWeek);
                    var lastDateOfWeek = new DateTime(date.Year, date.Month, date.AddDays(6 - dayOfWeek).Day, 23, 59, 59, 999); //date.AddDays(6 - dayOfWeek);

                    var siparisForWeek = SiparisService.FindBy(x => x.AktifMi == true && x.SiparisDurumTipId != 8 && x.SiparisDurumTipId != 9 && x.SiparisTarihi >= firstDateOfWeek && x.SiparisTarihi <= lastDateOfWeek, true).ToList();

                    for (int i = 0; i < 7; i++)
                    {
                        count = siparisForWeek.Where(x => x.SiparisTarihi >= firstDateOfWeek && x.SiparisTarihi <= firstDateOfWeek.AddDays(1)).ToList().Count();

                        siparisForDictionary.Add(firstDateOfWeek.ToString("dd.MM.yyyy"), count);
                        firstDateOfWeek = firstDateOfWeek.AddDays(1);
                    }
                    ViewBag.SiparisChartData = siparisForDictionary.Select(x => new { date = x.Key, value = x.Value }).ToList();
                    ViewBag.CategoryAxisTitle = "Haftanın Günleri";

                    siparisBilgiDictionary.Add("siparis", siparisForWeek.Sum(x => x.OdenecekTutar));
                    siparisBilgiDictionary.Add("komisyon", siparisForWeek.Sum(x => x.ToplamKomisyon));
                    siparisBilgiDictionary.Add("iskonto", siparisForWeek.Sum(x => x.ToplamIskonto));
                    siparisBilgiDictionary.Add("iade", siparisForWeek.Sum(x => x.IadeToplam));

                    break;
                // Bu ay için sipariş toplamı
                case "month":
                    var totalDaysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0);
                    var lastDayOfMonth = new DateTime(date.Year, date.Month, totalDaysInMonth, 23, 59, 59, 999);

                    var siparisForMonth = SiparisService.FindBy(x => x.AktifMi == true && x.SiparisDurumTipId != 8 && x.SiparisDurumTipId != 9 && x.SiparisTarihi >= firstDayOfMonth && x.SiparisTarihi <= lastDayOfMonth, true).ToList();

                    for (int i = 0; i < totalDaysInMonth; i++)
                    {
                        count = siparisForMonth.Where(x => x.SiparisTarihi >= firstDayOfMonth && x.SiparisTarihi <= firstDayOfMonth.AddDays(1)).ToList().Count();

                        siparisForDictionary.Add(firstDayOfMonth.Day.ToString(), count);
                        firstDayOfMonth = firstDayOfMonth.AddDays(1);
                    }
                    ViewBag.SiparisChartData = siparisForDictionary.Select(x => new { date = x.Key, value = x.Value }).ToList();
                    ViewBag.CategoryAxisTitle = "Ayın Günleri";

                    siparisBilgiDictionary.Add("siparis", siparisForMonth.Sum(x => x.OdenecekTutar));
                    siparisBilgiDictionary.Add("komisyon", siparisForMonth.Sum(x => x.ToplamKomisyon));
                    siparisBilgiDictionary.Add("iskonto", siparisForMonth.Sum(x => x.ToplamIskonto));
                    siparisBilgiDictionary.Add("iade", siparisForMonth.Sum(x => x.IadeToplam));

                    break;
            }
            ViewBag.SiparisBilgi = siparisBilgiDictionary;

            return PartialView("~/Views/Home/Partials/SiparisAdetRaporPartial.cshtml");
        }

        public ActionResult IadeTalepAdetRaporPartial(string dateType)
        {
            var date = DateTime.Now.Date;
            var count = 0;
            var iadeTalepForDictionary = new Dictionary<string, int>();
            var iadeTalepBilgiDictionary = new Dictionary<string, string>();

            string enFazlaIadeTalepUrunAdi = "", enFazlaIadeTalepUrunSayisi = "", enFazlaIadeTalepKullaniciEposta = "", enFazlaIadeTalepKullaniciSayisi = "", enFazlaIadeTalepNedenTipiAdi = "", enFazlaIadeTalepNedenTipiSayisi = "";
            IadeTalep iadeTalep;

            switch (dateType)
            {
                // Bugün için iade talep toplamı
                case "day":
                    var iadeTalepForToday = IadeTalepService.FindBy(x => x.AktifMi == true && DbFunctions.TruncateTime(x.OlusturmaTarihi) == date,
                        true, new string[] { "IadeTalepNedenTip", "SiparisDetay", "SiparisDetay.Urun", "SiparisDetay.Siparis", "SiparisDetay.Siparis.Kullanici" }).ToList();

                    for (int i = 0; i < 6; i++)
                    {
                        count = iadeTalepForToday.Where(x => x.OlusturmaTarihi >= date && x.OlusturmaTarihi <= date.AddHours(4)).ToList().Count();

                        iadeTalepForDictionary.Add(date.ToString("HH:mm") + "-" + date.AddHours(4).ToString("HH:mm"), count);
                        date = date.AddHours(4);
                    }
                    ViewBag.IadeTalepChartData = iadeTalepForDictionary.Select(x => new { date = x.Key, value = x.Value }).ToList();
                    ViewBag.CategoryAxisTitle = "Saat Aralığı";

                    iadeTalep = iadeTalepForToday.SingleOrDefault(x => x.SiparisDetay.Urun.UrunId == (iadeTalepForToday.Max(u => u.SiparisDetay.Urun.UrunId)));
                    if (iadeTalep != null)
                        enFazlaIadeTalepUrunAdi = iadeTalepForToday.SingleOrDefault(x => x.SiparisDetay.Urun.UrunId == (iadeTalepForToday.Max(u => u.SiparisDetay.Urun.UrunId))).SiparisDetay.Urun.Adi;
                    else
                        enFazlaIadeTalepUrunAdi = "YOK";

                    enFazlaIadeTalepUrunSayisi = iadeTalepForToday.Where(x => x.SiparisDetay.Urun.UrunId == (iadeTalepForToday.Max(u => u.SiparisDetay.Urun.UrunId))).Count().ToString();

                    iadeTalep = iadeTalepForToday.SingleOrDefault(x => x.SiparisDetay.Siparis.KullaniciId == (iadeTalepForToday.Max(u => u.SiparisDetay.Siparis.KullaniciId)));
                    if (iadeTalep != null)
                        enFazlaIadeTalepKullaniciEposta = iadeTalepForToday.SingleOrDefault(x => x.SiparisDetay.Siparis.KullaniciId == (iadeTalepForToday.Max(u => u.SiparisDetay.Siparis.KullaniciId))).SiparisDetay.Siparis.Kullanici.Eposta;
                    else
                        enFazlaIadeTalepKullaniciEposta = "YOK";

                    enFazlaIadeTalepKullaniciSayisi = iadeTalepForToday.Where(x => x.SiparisDetay.Siparis.KullaniciId == (iadeTalepForToday.Max(u => u.SiparisDetay.Siparis.KullaniciId))).Count().ToString();

                    iadeTalep = iadeTalepForToday.SingleOrDefault(x => x.IadeTalepNedenTipId == (iadeTalepForToday.Max(u => u.IadeTalepNedenTipId)));
                    if (iadeTalep != null)
                        enFazlaIadeTalepNedenTipiAdi = iadeTalepForToday.SingleOrDefault(x => x.IadeTalepNedenTipId == (iadeTalepForToday.Max(u => u.IadeTalepNedenTipId))).IadeTalepNedenTip.Adi;
                    else
                        enFazlaIadeTalepNedenTipiAdi = "YOK";

                    enFazlaIadeTalepNedenTipiSayisi = iadeTalepForToday.Where(x => x.IadeTalepNedenTipId == (iadeTalepForToday.Max(u => u.IadeTalepNedenTipId))).Count().ToString();

                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepUrunAdi", enFazlaIadeTalepUrunAdi);
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepUrunSayisi", enFazlaIadeTalepUrunSayisi.ToString());
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepKullaniciEposta", enFazlaIadeTalepKullaniciEposta);
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepKullaniciSayisi", enFazlaIadeTalepKullaniciSayisi.ToString());
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepNedenTipiAdi", enFazlaIadeTalepNedenTipiAdi);
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepNedenTipiSayisi", enFazlaIadeTalepNedenTipiSayisi);

                    break;
                // Bu hafta için iade talep toplamı
                case "week":
                    // 0- Pazartesi .... 6- Pazar
                    var dayOfWeek = (((int)date.DayOfWeek == 0) ? 7 : (int)date.DayOfWeek) - 1;

                    var firstDateOfWeek = new DateTime(date.Year, date.Month, date.AddDays((-1) * dayOfWeek).Day, 0, 0, 0, 0); //date.AddDays((-1) * dayOfWeek);
                    var lastDateOfWeek = new DateTime(date.Year, date.Month, date.AddDays(6 - dayOfWeek).Day, 23, 59, 59, 999); //date.AddDays(6 - dayOfWeek);

                    var iadeTalepForWeek = IadeTalepService.FindBy(x => x.AktifMi == true && x.OlusturmaTarihi >= firstDateOfWeek && x.OlusturmaTarihi <= lastDateOfWeek,
                        true, new string[] { "IadeTalepNedenTip", "SiparisDetay", "SiparisDetay.Urun", "SiparisDetay.Siparis", "SiparisDetay.Siparis.Kullanici" }).ToList();

                    for (int i = 0; i < 7; i++)
                    {
                        count = iadeTalepForWeek.Where(x => x.OlusturmaTarihi >= firstDateOfWeek && x.OlusturmaTarihi <= firstDateOfWeek.AddDays(1)).ToList().Count();

                        iadeTalepForDictionary.Add(firstDateOfWeek.ToString("dd.MM.yyyy"), count);
                        firstDateOfWeek = firstDateOfWeek.AddDays(1);
                    }
                    ViewBag.IadeTalepChartData = iadeTalepForDictionary.Select(x => new { date = x.Key, value = x.Value }).ToList();
                    ViewBag.CategoryAxisTitle = "Haftanın Günleri";

                    iadeTalep = iadeTalepForWeek.SingleOrDefault(x => x.SiparisDetay.Urun.UrunId == (iadeTalepForWeek.Max(u => u.SiparisDetay.Urun.UrunId)));
                    if (iadeTalep != null)
                        enFazlaIadeTalepUrunAdi = iadeTalepForWeek.SingleOrDefault(x => x.SiparisDetay.Urun.UrunId == (iadeTalepForWeek.Max(u => u.SiparisDetay.Urun.UrunId))).SiparisDetay.Urun.Adi;
                    else
                        enFazlaIadeTalepUrunAdi = "YOK";

                    enFazlaIadeTalepUrunSayisi = iadeTalepForWeek.Where(x => x.SiparisDetay.Urun.UrunId == (iadeTalepForWeek.Max(u => u.SiparisDetay.Urun.UrunId))).Count().ToString();

                    iadeTalep = iadeTalepForWeek.SingleOrDefault(x => x.SiparisDetay.Siparis.KullaniciId == (iadeTalepForWeek.Max(u => u.SiparisDetay.Siparis.KullaniciId)));
                    if (iadeTalep != null)
                        enFazlaIadeTalepKullaniciEposta = iadeTalepForWeek.SingleOrDefault(x => x.SiparisDetay.Siparis.KullaniciId == (iadeTalepForWeek.Max(u => u.SiparisDetay.Siparis.KullaniciId))).SiparisDetay.Siparis.Kullanici.Eposta;
                    else
                        enFazlaIadeTalepKullaniciEposta = "YOK";

                    enFazlaIadeTalepKullaniciSayisi = iadeTalepForWeek.Where(x => x.SiparisDetay.Siparis.KullaniciId == (iadeTalepForWeek.Max(u => u.SiparisDetay.Siparis.KullaniciId))).Count().ToString();

                    iadeTalep = iadeTalepForWeek.SingleOrDefault(x => x.IadeTalepNedenTipId == (iadeTalepForWeek.Max(u => u.IadeTalepNedenTipId)));
                    if (iadeTalep != null)
                        enFazlaIadeTalepNedenTipiAdi = iadeTalepForWeek.SingleOrDefault(x => x.IadeTalepNedenTipId == (iadeTalepForWeek.Max(u => u.IadeTalepNedenTipId))).IadeTalepNedenTip.Adi;
                    else
                        enFazlaIadeTalepNedenTipiAdi = "YOK";

                    enFazlaIadeTalepNedenTipiSayisi = iadeTalepForWeek.Where(x => x.IadeTalepNedenTipId == (iadeTalepForWeek.Max(u => u.IadeTalepNedenTipId))).Count().ToString();

                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepUrunAdi", enFazlaIadeTalepUrunAdi);
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepUrunSayisi", enFazlaIadeTalepUrunSayisi.ToString());
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepKullaniciEposta", enFazlaIadeTalepKullaniciEposta);
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepKullaniciSayisi", enFazlaIadeTalepKullaniciSayisi.ToString());
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepNedenTipiAdi", enFazlaIadeTalepNedenTipiAdi);
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepNedenTipiSayisi", enFazlaIadeTalepNedenTipiSayisi);

                    break;
                // Bu ay için iade talep toplamı
                case "month":
                    var totalDaysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0);
                    var lastDayOfMonth = new DateTime(date.Year, date.Month, totalDaysInMonth, 23, 59, 59, 999);

                    var iadeTalepForMonth = IadeTalepService.FindBy(x => x.AktifMi == true && x.OlusturmaTarihi >= firstDayOfMonth && x.OlusturmaTarihi <= lastDayOfMonth,
                        true, new string[] { "IadeTalepNedenTip", "SiparisDetay", "SiparisDetay.Urun", "SiparisDetay.Siparis", "SiparisDetay.Siparis.Kullanici" }).ToList();

                    for (int i = 0; i < totalDaysInMonth; i++)
                    {
                        count = iadeTalepForMonth.Where(x => x.OlusturmaTarihi >= firstDayOfMonth && x.OlusturmaTarihi <= firstDayOfMonth.AddDays(1)).ToList().Count();

                        iadeTalepForDictionary.Add(firstDayOfMonth.Day.ToString(), count);
                        firstDayOfMonth = firstDayOfMonth.AddDays(1);
                    }
                    ViewBag.IadeTalepChartData = iadeTalepForDictionary.Select(x => new { date = x.Key, value = x.Value }).ToList();
                    ViewBag.CategoryAxisTitle = "Ayın Günleri";

                    iadeTalep = iadeTalepForMonth.SingleOrDefault(x => x.SiparisDetay.Urun.UrunId == (iadeTalepForMonth.Max(u => u.SiparisDetay.Urun.UrunId)));
                    if (iadeTalep != null)
                        enFazlaIadeTalepUrunAdi = iadeTalepForMonth.SingleOrDefault(x => x.SiparisDetay.Urun.UrunId == (iadeTalepForMonth.Max(u => u.SiparisDetay.Urun.UrunId))).SiparisDetay.Urun.Adi;
                    else
                        enFazlaIadeTalepUrunAdi = "YOK";

                    enFazlaIadeTalepUrunSayisi = iadeTalepForMonth.Where(x => x.SiparisDetay.Urun.UrunId == (iadeTalepForMonth.Max(u => u.SiparisDetay.Urun.UrunId))).Count().ToString();

                    iadeTalep = iadeTalepForMonth.SingleOrDefault(x => x.SiparisDetay.Siparis.KullaniciId == (iadeTalepForMonth.Max(u => u.SiparisDetay.Siparis.KullaniciId)));
                    if (iadeTalep != null)
                        enFazlaIadeTalepKullaniciEposta = iadeTalepForMonth.SingleOrDefault(x => x.SiparisDetay.Siparis.KullaniciId == (iadeTalepForMonth.Max(u => u.SiparisDetay.Siparis.KullaniciId))).SiparisDetay.Siparis.Kullanici.Eposta;
                    else
                        enFazlaIadeTalepKullaniciEposta = "YOK";

                    enFazlaIadeTalepKullaniciSayisi = iadeTalepForMonth.Where(x => x.SiparisDetay.Siparis.KullaniciId == (iadeTalepForMonth.Max(u => u.SiparisDetay.Siparis.KullaniciId))).Count().ToString();

                    iadeTalep = iadeTalepForMonth.SingleOrDefault(x => x.IadeTalepNedenTipId == (iadeTalepForMonth.Max(u => u.IadeTalepNedenTipId)));
                    if (iadeTalep != null)
                        enFazlaIadeTalepNedenTipiAdi = iadeTalepForMonth.SingleOrDefault(x => x.IadeTalepNedenTipId == (iadeTalepForMonth.Max(u => u.IadeTalepNedenTipId))).IadeTalepNedenTip.Adi;
                    else
                        enFazlaIadeTalepNedenTipiAdi = "YOK";

                    enFazlaIadeTalepNedenTipiSayisi = iadeTalepForMonth.Where(x => x.IadeTalepNedenTipId == (iadeTalepForMonth.Max(u => u.IadeTalepNedenTipId))).Count().ToString();

                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepUrunAdi", enFazlaIadeTalepUrunAdi);
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepUrunSayisi", enFazlaIadeTalepUrunSayisi.ToString());
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepKullaniciEposta", enFazlaIadeTalepKullaniciEposta);
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepKullaniciSayisi", enFazlaIadeTalepKullaniciSayisi.ToString());
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepNedenTipiAdi", enFazlaIadeTalepNedenTipiAdi);
                    iadeTalepBilgiDictionary.Add("enFazlaIadeTalepNedenTipiSayisi", enFazlaIadeTalepNedenTipiSayisi);

                    break;
            }
            ViewBag.IadeTalepBilgi = iadeTalepBilgiDictionary;

            return PartialView("~/Views/Home/Partials/IadeTalepAdetRaporPartial.cshtml");
        }

        public ActionResult EnCokSatanUrunRaporPartial(string dateType)
        {
            var date = DateTime.Now.Date;
            var enCokSatanUrunForDictionary = new Dictionary<string, int>();
            var urunler = UrunService.GetAll(true).ToList();

            switch (dateType)
            {
                case "day":
                    var top10EnCokSatanUrunDictionary1 = SiparisDetayService.FindBy(x => x.AktifMi == true && x.Siparis.AktifMi == true && DbFunctions.TruncateTime(x.Siparis.SiparisTarihi) == date,
                        true, new string[] { "Siparis", "Urun" }).
                        GroupBy(x => x.UrunId).Select(x => new { UrunId = x.Key, ToplamSatisAdedi = x.Sum(u => u.Adet) }).ToDictionary(x => x.UrunId, x => x.ToplamSatisAdedi).OrderByDescending(x => x.Value).Take(10);

                    ViewBag.EnCokSatanUrunChartData = top10EnCokSatanUrunDictionary1.Select(x => new
                    {
                        urun = urunler.First(u => u.UrunId == x.Key).Adi,
                        adet = x.Value
                    }).ToList();

                    break;
                case "week":
                    // 0- Pazartesi .... 6- Pazar
                    var dayOfWeek = (((int)date.DayOfWeek == 0) ? 7 : (int)date.DayOfWeek) - 1;

                    var firstDateOfWeek = new DateTime(date.Year, date.Month, date.AddDays((-1) * dayOfWeek).Day, 0, 0, 0, 0);
                    var lastDateOfWeek = new DateTime(date.Year, date.Month, date.AddDays(6 - dayOfWeek).Day, 23, 59, 59, 999);


                    var top10EnCokSatanUrunDictionary2 = SiparisDetayService.FindBy(x => x.AktifMi == true && x.Siparis.AktifMi == true && x.Siparis.SiparisTarihi >= firstDateOfWeek && x.Siparis.SiparisTarihi <= lastDateOfWeek,
                       true, new string[] { "Siparis", "Urun" }).
                       GroupBy(x => x.UrunId).Select(x => new { UrunId = x.Key, ToplamSatisAdedi = x.Sum(u => u.Adet) }).ToDictionary(x => x.UrunId, x => x.ToplamSatisAdedi).OrderByDescending(x => x.Value).Take(10);

                    ViewBag.EnCokSatanUrunChartData = top10EnCokSatanUrunDictionary2.Select(x => new
                    {
                        urun = urunler.First(u => u.UrunId == x.Key).Adi,
                        adet = x.Value
                    }).ToList();

                    break;
                case "month":
                    var totalDaysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0);
                    var lastDayOfMonth = new DateTime(date.Year, date.Month, totalDaysInMonth, 23, 59, 59, 999);

                    var top10EnCokSatanUrunDictionary3 = SiparisDetayService.FindBy(x => x.AktifMi == true && x.Siparis.AktifMi == true && x.Siparis.SiparisTarihi >= firstDayOfMonth && x.Siparis.SiparisTarihi <= lastDayOfMonth,
                        true, new string[] { "Siparis", "Urun" }).
                        GroupBy(x => x.UrunId).Select(x => new { UrunId = x.Key, ToplamSatisAdedi = x.Sum(u => u.Adet) }).ToDictionary(x => x.UrunId, x => x.ToplamSatisAdedi).OrderByDescending(x => x.Value).Take(10);

                    ViewBag.EnCokSatanUrunChartData = top10EnCokSatanUrunDictionary3.Select(x => new
                    {
                        urun = urunler.First(u => u.UrunId == x.Key).Adi,
                        adet = x.Value
                    }).ToList();

                    break;
            }

            return PartialView("~/Views/Home/Partials/EnCokSatanUrunRaporPartial.cshtml");
        }

        public ActionResult EnCokKazandiranUrunRaporPartial(string dateType)
        {
            var date = DateTime.Now.Date;
            var enCokKazandiranUrunForDictionary = new Dictionary<string, int>();
            var urunler = UrunService.GetAll(true).ToList();

            switch (dateType)
            {
                case "day":
                    var top10EnCokKazandiranUrunDictionary1 = SiparisDetayService.FindBy(x => x.AktifMi == true && x.Siparis.AktifMi == true && DbFunctions.TruncateTime(x.Siparis.SiparisTarihi) == date,
                        true, new string[] { "Siparis", "Urun" }).
                        GroupBy(x => x.UrunId).Select(x => new { UrunId = x.Key, ToplamKazanc = x.Sum(u => u.HesaplananKdvDahilTutar) }).
                        ToDictionary(x => x.UrunId, x => x.ToplamKazanc).OrderByDescending(x => x.Value).Take(10);

                    ViewBag.EnCokKazandiranUrunChartData = top10EnCokKazandiranUrunDictionary1.Select(x => new
                    {
                        urun = urunler.First(u => u.UrunId == x.Key).Adi,
                        satis = x.Value.ToString("##")
                    }).ToList();

                    break;
                case "week":
                    // 0- Pazartesi .... 6- Pazar
                    var dayOfWeek = (((int)date.DayOfWeek == 0) ? 7 : (int)date.DayOfWeek) - 1;

                    var firstDateOfWeek = new DateTime(date.Year, date.Month, date.AddDays((-1) * dayOfWeek).Day, 0, 0, 0, 0);
                    var lastDateOfWeek = new DateTime(date.Year, date.Month, date.AddDays(6 - dayOfWeek).Day, 23, 59, 59, 999);


                    var top10EnCokSatanUrunDictionary2 = SiparisDetayService.FindBy(x => x.AktifMi == true && x.Siparis.AktifMi == true && x.Siparis.SiparisTarihi >= firstDateOfWeek && x.Siparis.SiparisTarihi <= lastDateOfWeek,
                       true, new string[] { "Siparis", "Urun" }).
                       GroupBy(x => x.UrunId).Select(x => new { UrunId = x.Key, ToplamKazanc = x.Sum(u => u.HesaplananKdvDahilTutar) }).
                       ToDictionary(x => x.UrunId, x => x.ToplamKazanc).OrderByDescending(x => x.Value).Take(10);

                    ViewBag.EnCokKazandiranUrunChartData = top10EnCokSatanUrunDictionary2.Select(x => new
                    {
                        urun = urunler.First(u => u.UrunId == x.Key).Adi,
                        satis = x.Value.ToString("##")
                    }).ToList();

                    break;
                case "month":
                    var totalDaysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0);
                    var lastDayOfMonth = new DateTime(date.Year, date.Month, totalDaysInMonth, 23, 59, 59, 999);

                    var top10EnCokSatanUrunDictionary3 = SiparisDetayService.FindBy(x => x.AktifMi == true && x.Siparis.AktifMi == true && x.Siparis.SiparisTarihi >= firstDayOfMonth && x.Siparis.SiparisTarihi <= lastDayOfMonth,
                        true, new string[] { "Siparis", "Urun" }).
                        GroupBy(x => x.UrunId).Select(x => new { UrunId = x.Key, ToplamKazanc = x.Sum(u => u.HesaplananKdvDahilTutar) }).
                        ToDictionary(x => x.UrunId, x => x.ToplamKazanc).OrderByDescending(x => x.Value).Take(10);

                    ViewBag.EnCokKazandiranUrunChartData = top10EnCokSatanUrunDictionary3.Select(x => new
                    {
                        urun = urunler.First(u => u.UrunId == x.Key).Adi,
                        satis = x.Value.ToString("##")
                    }).ToList();

                    break;
            }

            return PartialView("~/Views/Home/Partials/EnCokKazandiranUrunRaporPartial.cshtml");
        }

        public ActionResult LayoutSiparisPartial()
        {
            var suAn = DateTime.Now.Date;

            ViewBag.BeklemedeSiparisler = SiparisService.FindBy(x => x.AktifMi == true && x.SiparisDurumTipId == 1,
                true, new string[] { "Kullanici", "Kullanici.KullaniciDetay" }).ToList();

            ViewBag.OnayBekleyenSiparisler = SiparisService.FindBy(x => x.AktifMi == true && x.SiparisDurumTipId == 2,
                true, new string[] { "Kullanici", "Kullanici.KullaniciDetay" }).ToList();

            ViewBag.GonderildiSiparisler = SiparisService.FindBy(x => x.AktifMi == true && x.SiparisDurumTipId == 4 && (x.SiparisDetay.Any(p => p.SiparisGonderim.Any(z => DbFunctions.TruncateTime(z.OlusturmaTarihi) == suAn))),
                true, new string[] { "Kullanici", "Kullanici.KullaniciDetay", "SiparisDetay", "SiparisDetay.SiparisGonderim" }).ToList();

            return PartialView("~/Views/Shared/Partials/LayoutSiparisPartial.cshtml");
        }
        #endregion
    }
}