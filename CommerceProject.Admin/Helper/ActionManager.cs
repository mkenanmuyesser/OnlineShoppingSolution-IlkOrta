using CommerceProject.Business.BusinessServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceProject.Admin.Helper
{
    public class ActionManager : ActionFilterAttribute
    {
        private KullaniciService kullaniciService = new KullaniciService();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var controller = "";
            var action = "";

            if (filterContext != null && filterContext.RouteData != null && filterContext.RouteData.Values.Any())
            {
                controller = filterContext.RouteData.Values["controller"].ToString();
                action = filterContext.RouteData.Values["action"].ToString();
            }

            if (filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (!(controller=="Home" && action=="Index"))
                {
                    Guid superAdminGuid = Guid.Parse("fe3e6535-b81c-483e-ae2f-a116a1128c8a");
                    var kullanici = kullaniciService.GetAuthenticatedUser(true);
                    // süper admin ise heryeri görebilir
                    if (kullanici.KullaniciId != superAdminGuid)
                    {
                        if (string.IsNullOrEmpty(controller) && string.IsNullOrEmpty(action))
                        {
                            filterContext.Result = new RedirectResult("/Home/NotAuthorize");
                        }

                        // sayfayı görme yetkisi var mı?
                        var sayfayaAitYetkiVarmi = kullanici.KullaniciYetki.Any(x => x.AktifMi && x.Yetki != null && x.Yetki.AktifMi && x.Yetki.Controller == controller && x.Yetki.Action == action);
                        if (!sayfayaAitYetkiVarmi)
                        {
                            filterContext.Result = new RedirectResult("/Home/NotAuthorize");
                        }
                    }
                }                           
            }
        }
    }
}