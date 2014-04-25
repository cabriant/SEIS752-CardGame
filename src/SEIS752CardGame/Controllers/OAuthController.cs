using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Oauth2Login;
using Oauth2Login.Client;
using SEIS752CardGame.Models.Login;
using SEIS752CardGame.Utilities;

namespace SEIS752CardGame.Controllers
{
    public class OAuthController : BaseWebController
    {
        public enum OAuthType
        {
            Google
        }

        private static Oauth2LoginContext _context;

        [HttpGet]
        public ActionResult Login(OAuthType type)
        {
            if (IsUserAuthenticated())
                return RedirectToAction("Index", "Main");

            var url = string.Empty;
            AbstractClientProvider client;

            switch (type)
            {
                case OAuthType.Google:
                    client = Oauth2LoginFactory.CreateClient<GoogleClinet>("Google");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            if (client != null)
            {
                _context = Oauth2LoginContext.Create(client);
                url = _context.BeginAuth();
            }

            if (!string.IsNullOrEmpty(url))
                return Redirect(url);

            return Redirect("/");
        }

        [HttpGet]
        public ActionResult Auth(string state, string code)
        {
            try
            {
                var token = _context.Token;
                var result = _context.Profile;
                var strResult = _context.Client.ProfileJsonString;

                return Content("<div>token: "+token+"</div><div>strResult: "+strResult+"</div>");
            }
            catch (Exception)
            {
                
                throw;
            }

            return null;
        }
    }
}