using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace SEIS752CardGame.Utilities
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthenticatedApiRequest : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var authVal = SessionDataPersistor.Instance.GetFromSession<string>("isAuth");
            return ("Y".Equals(authVal));
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            filterContext.Response.StatusCode = HttpStatusCode.BadRequest;
        }
    }
}