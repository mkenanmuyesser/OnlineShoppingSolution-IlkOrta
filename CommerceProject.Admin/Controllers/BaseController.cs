using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.Custom;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace CommerceProject.Admin.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class BaseController : Controller
    {
        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        public BaseController(IIcerikAyarService iIcerikAyarService,
                              IKullaniciService iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            if (ViewBag.IcerikAyar == null)
            {
                var icerikAyar = IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);
                ViewBag.IcerikAyar = icerikAyar;
            }

            if (ViewBag.GirisYapanKullanici == null)
            {
                var girisYapanKullanici = KullaniciService.GetAuthenticatedUser();
                ViewBag.GirisYapanKullanici = girisYapanKullanici;
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