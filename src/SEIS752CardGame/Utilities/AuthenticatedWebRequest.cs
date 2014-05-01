using System.Web;
using System.Web.Mvc;
using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Utilities
{
    public class AuthenticatedWebRequest : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = SessionDataPersistor.Instance.GetFromSession<UserModel>(SessionDataPersistor.SessionKey.UserKey);
            return (user != null);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            filterContext.Result = new RedirectResult("/?path=" + 
                HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.PathAndQuery));
        }
    }
}