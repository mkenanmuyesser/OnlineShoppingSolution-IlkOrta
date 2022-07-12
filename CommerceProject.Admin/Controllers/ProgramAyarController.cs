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
using CommerceProject.Business.Helper.Caching;

namespace CommerceProject.Admin.Controllers
{
    public class ProgramAyarController : BaseController
    {
        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        IIpAdresService IpAdresService;
        public ProgramAyarController(IIcerikAyarService iIcerikAyarService,
                                     IKullaniciService iKullaniciService,
                                     IIpAdresService iIpAdresService) : base(iIcerikAyarService,
                                                                                 iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;
            IpAdresService = iIpAdresService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.programayar_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Program Ayar", "", "Program Ayarları", "");

            var icerikAyar = IcerikAyarService.GetAll().FirstOrDefault();

            if (icerikAyar == null)
            {
                ViewBag.IcerikAyar = new IcerikAyar();
            }
            else
            {
                ViewBag.IcerikAyar = icerikAyar;
            }

            ViewBag.IpAdresleri = IpAdresService.GetAll(true).ToList();

            return View();
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult UygulamaKaydetGuncelle(IcerikAyarUygulamaDataObj obj)
        {
            try
            {
                var _icerikAyar = IcerikAyarService.GetAll().FirstOrDefault();

                if (_icerikAyar == null)
                    _icerikAyar = new IcerikAyar();

                _icerikAyar.ParaPuanLimit = obj.ParaPuanLimit;
                _icerikAyar.ParaPuanKazanc = obj.ParaPuanKazanc;
                _icerikAyar.ClearCacheTime = obj.ClearCacheTime;
                _icerikAyar.KeepAliveTime = obj.KeepAliveTime;
                _icerikAyar.SecureUrl = obj.SecureUrl;
                _icerikAyar.SslAktifMi = obj.SslAktifMi;
                _icerikAyar.Url = obj.Url;
                _icerikAyar.UygulamaAciklama = obj.UygulamaAciklama;
                _icerikAyar.UygulamaAd = obj.UygulamaAd;
                _icerikAyar.UygulamaAktifMi = obj.UygulamaAktifMi;
                _icerikAyar.FacebookHesapUrl = obj.FacebookHesapUrl;
                _icerikAyar.TwitterHesapUrl = obj.TwitterHesapUrl;
                _icerikAyar.CookieTime = obj.CookieTime;
                _icerikAyar.ParaPuanAktifMi = obj.ParaPuanAktifMi;
                _icerikAyar.SayfaKuponAktifMi = obj.SayfaKuponAktifMi;
                _icerikAyar.CacheAktifMi = obj.CacheAktifMi;

                if (_icerikAyar.AyarId == 0)
                {
                    IcerikAyarService.Add(_icerikAyar);
                }
                else
                {
                    IcerikAyarService.Edit(_icerikAyar);
                }

                var flag = IcerikAyarService.Save();

                //ayarlar ne olursa olsun cacheden alındığı için cache temizlenmesi gerekiyor
                CacheHelper.ClearCache(CacheDataObj.IcerikAyarlari);

                if (flag)
                    return Json(_icerikAyar.AyarId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult SirketKaydetGuncelle(IcerikAyarSirketDataObj obj)
        {
            try
            {
                var _icerikAyar = IcerikAyarService.GetAll().FirstOrDefault();

                if (_icerikAyar == null)
                    _icerikAyar = new IcerikAyar();

                _icerikAyar.SirketAciklama = obj.SirketAciklama;
                _icerikAyar.SirketAd = obj.SirketAd;
                _icerikAyar.SirketAdres = obj.SirketAdres;
                _icerikAyar.SirketEposta = obj.SirketEposta;
                _icerikAyar.SirketFax1 = obj.SirketFax1;
                _icerikAyar.SirketFax2 = obj.SirketFax2;
                _icerikAyar.SirketHakkimizda = obj.SirketHakkimizda;
                _icerikAyar.SirketMapCode = obj.SirketMapCode;
                _icerikAyar.SirketTelefon1 = obj.SirketTelefon1;
                _icerikAyar.SirketTelefon2 = obj.SirketTelefon2;

                if (_icerikAyar.AyarId == 0)
                {
                    IcerikAyarService.Add(_icerikAyar);
                }
                else
                {
                    IcerikAyarService.Edit(_icerikAyar);
                }

                var flag = IcerikAyarService.Save();

                //ayarlar ne olursa olsun cacheden alındığı için cache temizlenmesi gerekiyor
                CacheHelper.ClearCache(CacheDataObj.IcerikAyarlari);

                if (flag)
                    return Json(_icerikAyar.AyarId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult SayfaKaydetGuncelle(IcerikAyarSayfaDataObj obj)
        {
            try
            {
                var _icerikAyar = IcerikAyarService.GetAll().FirstOrDefault();

                if (_icerikAyar == null)
                    _icerikAyar = new IcerikAyar();

                _icerikAyar.KdvDahilMi = obj.KdvDahilMi;
                _icerikAyar.MisafirSatinAlmaAktifMi = obj.MisafirSatinAlmaAktifMi;
                _icerikAyar.SayfaUrunYorumAktifMi = obj.SayfaUrunYorumAktifMi;

                if (_icerikAyar.AyarId == 0)
                {
                    IcerikAyarService.Add(_icerikAyar);
                }
                else
                {
                    IcerikAyarService.Edit(_icerikAyar);
                }

                var flag = IcerikAyarService.Save();

                //ayarlar ne olursa olsun cacheden alındığı için cache temizlenmesi gerekiyor
                CacheHelper.ClearCache(CacheDataObj.IcerikAyarlari);

                if (flag)
                    return Json(_icerikAyar.AyarId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult LogoKaydetGuncelle(string type)
        {
            try
            {
                if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                {
                    var guid = Guid.NewGuid();

                    var dosyaUzantisi = new FileInfo(Request.Files[0].FileName).Extension;

                    var savePath = Path.Combine(Server.MapPath("~/Uploads/Program/"), guid + dosyaUzantisi);
                    var dosyaPath = string.Format("/Uploads/Program/{0}",
                                        guid + dosyaUzantisi);

                    Request.Files[0].SaveAs(savePath);

                    var _icerikAyar = IcerikAyarService.GetAll().FirstOrDefault();

                    if (_icerikAyar == null)
                        _icerikAyar = new IcerikAyar();

                    if (type.ToLower() == "logokucuk")
                    {
                        _icerikAyar.KucukLogoResimUrl = dosyaPath;
                    }
                    else
                    {
                        _icerikAyar.BuyukLogoResimUrl = dosyaPath;
                    }

                    if (_icerikAyar.AyarId == 0)
                    {
                        IcerikAyarService.Add(_icerikAyar);
                    }
                    else
                    {
                        IcerikAyarService.Edit(_icerikAyar);
                    }

                    var flag = IcerikAyarService.Save();

                    //ayarlar ne olursa olsun cacheden alındığı için cache temizlenmesi gerekiyor
                    CacheHelper.ClearCache(CacheDataObj.IcerikAyarlari);

                    if (flag)
                        return Json(_icerikAyar.AyarId, JsonRequestBehavior.DenyGet);
                    return Json(0, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult EpostaKaydetGuncelle(IcerikAyarEpostaDataObj obj)
        {
            try
            {
                var _icerikAyar = IcerikAyarService.GetAll().FirstOrDefault();

                if (_icerikAyar == null)
                    _icerikAyar = new IcerikAyar();

                _icerikAyar.GonderilecekEposta = obj.GonderilecekEposta;
                _icerikAyar.GonderilecekEpostaAktifMi = obj.GonderilecekEpostaAktifMi;
                _icerikAyar.GonderilecekEpostaHost = obj.GonderilecekEpostaHost;
                _icerikAyar.GonderilecekEpostaKullaniciAdi = obj.GonderilecekEpostaKullaniciAdi;
                _icerikAyar.GonderilecekEpostaPort = obj.GonderilecekEpostaPort;
                _icerikAyar.GonderilecekEpostaSifre = obj.GonderilecekEpostaSifre;
                _icerikAyar.GonderilecekEpostaTanim = obj.GonderilecekEpostaTanim;

                if (_icerikAyar.AyarId == 0)
                {
                    IcerikAyarService.Add(_icerikAyar);
                }
                else
                {
                    IcerikAyarService.Edit(_icerikAyar);
                }

                var flag = IcerikAyarService.Save();

                //ayarlar ne olursa olsun cacheden alındığı için cache temizlenmesi gerekiyor
                CacheHelper.ClearCache(CacheDataObj.IcerikAyarlari);

                if (flag)
                    return Json(_icerikAyar.AyarId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult SmsKaydetGuncelle(IcerikAyarSmsDataObj obj)
        {
            try
            {
                var _icerikAyar = IcerikAyarService.GetAll().FirstOrDefault();

                if (_icerikAyar == null)
                    _icerikAyar = new IcerikAyar();

                _icerikAyar.SmsKullaniciAdi = obj.SmsKullaniciAdi;
                _icerikAyar.SmsSifre = obj.SmsSifre;

                if (_icerikAyar.AyarId == 0)
                {
                    IcerikAyarService.Add(_icerikAyar);
                }
                else
                {
                    IcerikAyarService.Edit(_icerikAyar);
                }

                var flag = IcerikAyarService.Save();

                //ayarlar ne olursa olsun cacheden alındığı için cache temizlenmesi gerekiyor
                CacheHelper.ClearCache(CacheDataObj.IcerikAyarlari);

                if (flag)
                    return Json(_icerikAyar.AyarId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult SeoKaydetGuncelle(IcerikAyarSeoDataObj obj)
        {
            try
            {
                var _icerikAyar = IcerikAyarService.GetAll().FirstOrDefault();

                if (_icerikAyar == null)
                    _icerikAyar = new IcerikAyar();

                _icerikAyar.MetaDescription = obj.MetaDescription;
                _icerikAyar.MetaKeywords = obj.MetaKeywords;
                _icerikAyar.MetaTitle = obj.MetaTitle;

                if (_icerikAyar.AyarId == 0)
                {
                    IcerikAyarService.Add(_icerikAyar);
                }
                else
                {
                    IcerikAyarService.Edit(_icerikAyar);
                }

                var flag = IcerikAyarService.Save();

                //ayarlar ne olursa olsun cacheden alındığı için cache temizlenmesi gerekiyor
                CacheHelper.ClearCache(CacheDataObj.IcerikAyarlari);

                if (flag)
                    return Json(_icerikAyar.AyarId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult IcerikKaydetGuncelle(IcerikAyarIcerikDataObj obj)
        {
            try
            {
                var _icerikAyar = IcerikAyarService.GetAll().FirstOrDefault();

                if (_icerikAyar == null)
                    _icerikAyar = new IcerikAyar();

                _icerikAyar.GuvenlikIlkeleri = obj.GuvenlikIlkeleri;
                _icerikAyar.IadeKosullari = obj.IadeKosullari;
                _icerikAyar.MesafeliSatisSozlesmesi = obj.MesafeliSatisSozlesmesi;
                _icerikAyar.TuketiciHaklari = obj.TuketiciHaklari;

                if (_icerikAyar.AyarId == 0)
                {
                    IcerikAyarService.Add(_icerikAyar);
                }
                else
                {
                    IcerikAyarService.Edit(_icerikAyar);
                }

                var flag = IcerikAyarService.Save();

                //ayarlar ne olursa olsun cacheden alındığı için cache temizlenmesi gerekiyor
                CacheHelper.ClearCache(CacheDataObj.IcerikAyarlari);

                if (flag)
                    return Json(_icerikAyar.AyarId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult IPAdresiBloklamaKaydetGuncelle(bool aktifMi)
        {
            try
            {
                var _icerikAyar = IcerikAyarService.GetAll().FirstOrDefault();

                if (_icerikAyar == null)
                    _icerikAyar = new IcerikAyar();

                _icerikAyar.IpBloklamaAktifMi = aktifMi;

                if (_icerikAyar.AyarId == 0)
                {
                    IcerikAyarService.Add(_icerikAyar);
                }
                else
                {
                    IcerikAyarService.Edit(_icerikAyar);
                }

                var flag = IcerikAyarService.Save();

                //ayarlar ne olursa olsun cacheden alındığı için cache temizlenmesi gerekiyor
                CacheHelper.ClearCache(CacheDataObj.IcerikAyarlari);

                if (flag)
                    return Json(_icerikAyar.AyarId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult IPAdresiKaydetGuncelle(IpAdres ipAdres)
        {
            try
            {
                if(ipAdres == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                IpAdres _ipAdres;
                if(ipAdres.IpAdresId == 0)
                {
                    _ipAdres = new IpAdres();
                    _ipAdres.IpAdresi = ipAdres.IpAdresi;
                    _ipAdres.AktifMi = ipAdres.AktifMi;

                    IpAdresService.Add(_ipAdres);
                }
                else
                {
                    _ipAdres = IpAdresService.GetSingle(x => x.IpAdresId == ipAdres.IpAdresId);

                    if(_ipAdres == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _ipAdres.AktifMi = ipAdres.AktifMi;

                    IpAdresService.Edit(_ipAdres);
                }

                var flag = IpAdresService.Save();

                //ayarlar ne olursa olsun cacheden alındığı için cache temizlenmesi gerekiyor
                CacheHelper.ClearCache(CacheDataObj.IcerikAyarlari);

                if (flag)
                    return Json(1, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        // Logo silme
        [HttpPost]
        public JsonResult LogoSil(string type)
        {
            try
            {
                var _icerikAyar = IcerikAyarService.GetAll().FirstOrDefault();

                if (_icerikAyar == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                if (type.ToLower() == "logokucuk")
                {
                    _icerikAyar.KucukLogoResimUrl = "";
                }
                else
                {
                    _icerikAyar.BuyukLogoResimUrl = "";
                }

                IcerikAyarService.Edit(_icerikAyar);

                var flag = IcerikAyarService.Save();

                //ayarlar ne olursa olsun cacheden alındığı için cache temizlenmesi gerekiyor
                CacheHelper.ClearCache(CacheDataObj.IcerikAyarlari);

                if (flag)
                    return Json(_icerikAyar.AyarId, JsonRequestBehavior.DenyGet);
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