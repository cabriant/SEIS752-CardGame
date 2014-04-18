using System.Web.Mvc;
using System.Web.Routing;

namespace SEIS752CardGame.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Always pass all url requests to main controller's index action
            routes.MapRoute(
                "MainRoute",
                "{*url}",
                new { controller = "Main", action = "Index" }
            );
        }
    }
}