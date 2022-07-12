using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceProject.Admin.Helper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeManager : AuthorizeAttribute
    {
        //private bool _isAuthorized;

        //protected override bool AuthorizeCore(HttpContextBase httpContext)
        //{
        //    _isAuthorized = base.AuthorizeCore(httpContext);
        //    return _isAuthorized;
        //}

        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    base.OnAuthorization(filterContext);

        //    if (!_isAuthorized)
        //    {
        //        filterContext.Controller.TempData.Add("RedirectReason", "Unauthorized");
        //    }
        //}

        public string RedirectUrl = "/Giris/";

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            var controller = "";
            var action = "";

            if (filterContext != null && filterContext.RouteData != null && filterContext.RouteData.Values.Any())
            {
                controller = filterContext.RouteData.Values["controller"].ToString();
                action = filterContext.RouteData.Values["action"].ToString();
            }

            if (!filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(controller) && string.IsNullOrEmpty(action))
                    filterContext.Result = new RedirectResult(RedirectUrl);
                else
                    filterContext.Result = new RedirectResult(RedirectUrl + "?R=" + controller + "/" + action);
            }
        }
    }
}