using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.Custom;
using CommerceProject.Business.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace CommerceProject.Presentation.IlkOrta.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class BaseController : Controller
    {
        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        ISepetService SepetService;
        IKategoriService KategoriService;
        public BaseController(IIcerikAyarService iIcerikAyarService,
                              IKullaniciService iKullaniciService,
                              ISepetService iSepetService,
                              IKategoriService iKategoriService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;
            SepetService = iSepetService;
            KategoriService = iKategoriService;

            IcerikAyar icerikAyar = null;
            if (ViewBag.IcerikAyar == null)
            {
                icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);
                ViewBag.IcerikAyar = icerikAyar;
            }
            else
            {
                icerikAyar = ViewBag.IcerikAyar;
            }

            Kullanici girisYapanKullanici = null;
            if (ViewBag.Kullanici == null)
            {
                girisYapanKullanici = KullaniciService.GetAuthenticatedUser();
                ViewBag.Kullanici = girisYapanKullanici;
            }
            else
            {
                girisYapanKullanici = ViewBag.Kullanici;
            }

            if (ViewBag.MenuKategoriler == null)
            {
                if (icerikAyar.CacheAktifMi)
                    ViewBag.MenuKategoriler = KategoriService.GetMenuCategoriesFromCache(9);
                else
                    ViewBag.MenuKategoriler = KategoriService.GetMenuCategories(9);
            }

            if (ViewBag.Sepet == null)
            {
                ViewBag.Sepet = SepetService.GetUserProductsFromBasket(girisYapanKullanici == null ? Guid.Empty : girisYapanKullanici.KullaniciId);
            }
        }

        protected override void ExecuteCore()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");

            base.ExecuteCore();
        }
    }
}