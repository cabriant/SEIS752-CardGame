using System.Web.Mvc;
using SEIS752CardGame.Utilities;

namespace SEIS752CardGame.Controllers
{
    //[AuthenticatedWebRequest]
    public class MainController : BaseWebController
    {
	    private const string LOGIN_PATH = "login";
	    private const string ACCOUNT_CREATE_PATH = "account/create";

        public ActionResult Index(string url)
        {
            var urlPath = (string.IsNullOrEmpty(url) ? "" : url.ToLower());
            if (!IsUserAuthenticated() && !urlPath.StartsWith(LOGIN_PATH) && !urlPath.StartsWith(ACCOUNT_CREATE_PATH))
                return Redirect(LOGIN_PATH + "/" + url);
            
            if (IsUserAuthenticated() && (urlPath.StartsWith(LOGIN_PATH) || urlPath.StartsWith(ACCOUNT_CREATE_PATH)))
            {
                var newUrl = "/";
                if (urlPath.StartsWith(LOGIN_PATH + "/"))
                    newUrl = newUrl + urlPath.Replace(LOGIN_PATH + "/", "");
                return Redirect(newUrl);
            }

            return View();
        }
    }
}