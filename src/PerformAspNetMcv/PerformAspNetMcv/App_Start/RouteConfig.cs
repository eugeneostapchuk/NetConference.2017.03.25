using System.Web.Mvc;
using System.Web.Routing;
using PerformAspNetMcv.Tips;

namespace PerformAspNetMcv
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Fast",
                url: "fasturl",
                defaults: new { controller = "IndexFast", action = "Invoke"}
            );

            var route = new Route("testurl", new RouteHandler())
            {
                Defaults = new RouteValueDictionary(),
                Constraints = new RouteValueDictionary()
            };

            routes.Add(route);
        }
    }
}
