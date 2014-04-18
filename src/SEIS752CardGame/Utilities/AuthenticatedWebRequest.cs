using System.Web;
using System.Web.Mvc;

namespace SEIS752CardGame.Utilities
{
    public class AuthenticatedWebRequest : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authVal = SessionDataPersistor.Instance.GetFromSession<string>("isAuth");
            return ("Y".Equals(authVal));
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            filterContext.Result = new RedirectResult("/?path=" + 
                HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.PathAndQuery));
        }
    }
}