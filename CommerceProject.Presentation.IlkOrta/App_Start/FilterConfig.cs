using System.Web;
using System.Web.Mvc;

namespace CommerceProject.Presentation.IlkOrta
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
