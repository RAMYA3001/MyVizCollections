using System.Web;
using System.Web.Mvc;

namespace MyVizCollections
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // MUST ADD THIS
            filters.Add(new SessionExpireAttribute());
        }
    }
}