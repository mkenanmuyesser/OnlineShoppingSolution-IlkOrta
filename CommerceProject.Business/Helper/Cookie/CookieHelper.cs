using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.BusinessServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CommerceProject.Business.Helper.Cookie
{
    public class CookieHelper
    {
        private const string CookieName = "CommerceProject";
        private static IIcerikAyarService IcerikAyarService = new IcerikAyarService();

        public static HttpCookie GetUserCookie()
        {
            var icerikAyar = IcerikAyarService.GetFirst();
            var cookie = HttpContext.Current.Request.Cookies[CookieName];

            if (cookie == null)
            {
                cookie = new HttpCookie(CookieName);
                cookie.Expires = DateTime.Now.AddHours(icerikAyar.CookieTime);
                //cookie.Secure = true;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            return cookie;
        }

        public static bool SetUserCookie(HttpCookie cookie)
        {
            try
            {
                var icerikAyar = IcerikAyarService.GetFirst();
                cookie.Expires = DateTime.Now.AddHours(icerikAyar.CookieTime);
                HttpContext.Current.Response.Cookies.Add(cookie);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool ClearCookie(HttpCookie cookie)
        {
            try
            {
                cookie.Expires = DateTime.Now.AddSeconds(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
