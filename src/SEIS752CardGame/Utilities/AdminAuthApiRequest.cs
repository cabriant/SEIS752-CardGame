using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;
using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Utilities
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AdminAuthApiRequest : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var user = SessionDataPersistor.Instance.GetFromSession<UserModel>(SessionDataPersistor.SessionKey.UserKey);
            return (user != null && user.Type == UserModel.UserType.Admin);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            filterContext.Response.StatusCode = HttpStatusCode.BadRequest;
        }
    }
}