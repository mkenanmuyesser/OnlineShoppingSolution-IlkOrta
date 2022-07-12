using CommerceProject.Presentation.IlkOrta.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CommerceProject.Presentation.IlkOrta
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // BotDetect requests must not be routed
            routes.IgnoreRoute("{*botdetect}",
              new { botdetect = @"(.*)BotDetectCaptcha\.ashx" });

            routes.Add("Ürün Detayı", new SeoFriendlyRoute("Urun/Detay/{id}",
           new RouteValueDictionary(new { controller = "Urun", action = "Detay" }),
           new MvcRouteHandler()));

            routes.Add("Paket Detayı", new SeoFriendlyRoute("Urun/Paket/{id}",
          new RouteValueDictionary(new { controller = "Urun", action = "Paket" }),
          new MvcRouteHandler()));

            routes.Add("Kategori Detayı", new SeoFriendlyRoute("Urun/Index/{id}",
      new RouteValueDictionary(new { controller = "Urun", action = "Index" }),
      new MvcRouteHandler()));

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );          
        }
    }
}
