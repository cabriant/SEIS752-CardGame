using System.Web.Mvc;
using System.Web.Routing;

namespace SEIS752CardGame.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "OAuthLoginRoute",
                "OAuth/Login/{type}",
                new {controller = "OAuth", action = "Login"});

            routes.MapRoute(
                "OAuthAuthRoute",
                "OAuth/Auth",
                new { controller = "OAuth", action = "Auth" });

            // Always pass all url requests to main controller's index action
            routes.MapRoute(
                "MainRoute",
                "{*url}",
                new { controller = "Main", action = "Index" }
            );
        }
    }
}