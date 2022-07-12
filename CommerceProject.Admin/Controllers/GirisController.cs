using CommerceProject.Admin.Helper;
using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.Entities;
using System.Web.Mvc;
using System.Linq;
using CommerceProject.Business.Helper.Program;

namespace CommerceProject.Admin.Controllers
{
    [AllowAnonymous]
    public class GirisController : BaseController
    {
        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        IIpAdresService IpAdresService;
        public GirisController(IIcerikAyarService iIcerikAyarService,
                               IKullaniciService iKullaniciService,
                               IIpAdresService iIpAdresService) : base(iIcerikAyarService,
                                                                       iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            IpAdresService = iIpAdresService;
        }

        #region Actions
        public ActionResult Index()
        {
            // Kullanıcı her durumda mutlaka bu actiona yönlendiriliyor. Burada ip bloklama aktifse ip kontrolü yapacağız. Eğer gelen ip listesinde varsa view gösterilecek. Yoksa ana internet uygulamasına yönlendirme yapılacak.
            IcerikAyar icerikAyar = ViewBag.IcerikAyar as IcerikAyar;
            if (icerikAyar.IpBloklamaAktifMi)
            {
                //gelen kullanıcının ip adresi listedekilerle uyuşuyormu.
                string gelenIp = ProgramHelper.GetClientIpAddress(Request);
                if (!IpAdresService.FindBy(x => x.AktifMi, false).ToList().Any(x => x.IpAdresi == gelenIp))
                    return Redirect(icerikAyar.Url);
            }

            return View();
        }
        #endregion

        #region Ajax Methods          
        [HttpPost]
        public ActionResult KullaniciGiris(string eposta, string sifre, bool beniHatirla, string returnUrl)
        {
            var kullaniciGirisSonuc = KullaniciService.LoginAdminUser(eposta, sifre, beniHatirla);
            if (kullaniciGirisSonuc == true)
                return Json(new
                {
                    flag = true,
                    returnUrl = "/" + returnUrl
                }, JsonRequestBehavior.DenyGet);
            else
                return Json(new
                {
                    flag = false,
                    returnUrl = ""
                }, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public ActionResult KullaniciCikis(string returnUrl)
        {
            KullaniciService.LogoutUser();

            return Redirect(returnUrl);
        }
        #endregion
    }
}