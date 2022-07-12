using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.BusinessServices;
using CommerceProject.Business.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CommerceProject.Presentation.IlkOrta.Helper
{
    public class ActionManager : ActionFilterAttribute
    {
        IIcerikAyarService IcerikAyarService = DependencyResolver.Current.GetService<IIcerikAyarService>();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (IcerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari).UygulamaAktifMi == false)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                    new
                    {
                        controller = "Home",
                        action = "Bakim"
                    }));
            }
        }
    }
}