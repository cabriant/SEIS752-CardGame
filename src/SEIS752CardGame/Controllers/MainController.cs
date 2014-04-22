using System.Web.Mvc;
using SEIS752CardGame.Utilities;

namespace SEIS752CardGame.Controllers
{
    //[AuthenticatedWebRequest]
    public class MainController : BaseWebController
    {
        public ActionResult Index(string url)
        {
            var urlPath = (string.IsNullOrEmpty(url) ? "" : url.ToLower());
            if (!IsUserAuthenticated() && !urlPath.StartsWith("login"))
                return Redirect("/login/" + url);
            
            if (IsUserAuthenticated() && urlPath.StartsWith("login"))
            {
                var newUrl = "/";
                if (urlPath.StartsWith("login/"))
                    newUrl = newUrl + urlPath.Replace("login/", "");
                return Redirect(newUrl);
            }

            return View();
        }
    }
}