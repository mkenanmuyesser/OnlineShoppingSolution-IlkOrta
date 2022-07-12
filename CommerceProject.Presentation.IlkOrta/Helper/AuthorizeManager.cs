using System;
using System.Web.Mvc;

namespace CommerceProject.Presentation.IlkOrta.Helper
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

        public string RedirectUrl = "/";

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            if (!filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult(RedirectUrl);
            }
        }
    }
}