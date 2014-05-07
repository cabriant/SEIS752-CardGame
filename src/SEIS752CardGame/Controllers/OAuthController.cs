using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Oauth2Login;
using Oauth2Login.Client;
using SEIS752CardGame.Models.Login;
using SEIS752CardGame.Utilities;
using SEIS752CardGame.Business.Models;
using SEIS752CardGame.Business.Services;
using SEIS752CardGame.Models.Account;
using SEIS752CardGame.Models.User;

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
            if (IsUserAuthenticated())
                return RedirectToAction("Index", "Main");

            try
            {
                var token = _context.Token;
                Dictionary<String, String> dictionary = _context.Client.Profile;

                /*
                var output = "<div>token: " + token + "</div>";
                foreach (var pair in dictionary)
                {
                    var key = pair.Key;
                    var value = pair.Value;
                    output += "<div> Key: " + key + " Value: " + value + "</div>";
                }
                */

                var response = new AuthResponse();


                var errors = new List<string>();

                if(!string.Equals(dictionary["verified_email"],"true"))
                    response.Error="Your email address must be verified by Google";

                var success = false;
                if (!errors.Any())
                {

                    //try authenticate

                    var user = UserService.Instance.AuthenticateOAuthUser(dictionary["email"], _context.Token);
                    response = new AuthResponse {Authenticated = (user != null)};
                    if (user != null)
                    {
                        SessionDataPersistor.Instance.StoreInSession(SessionDataPersistor.SessionKey.UserKey, user);
                        response.User = new DisplayUserModel() {DisplayName = user.DisplayName, Id = user.Id};
                    }
                    else
                    {
                        //if !authenticated then try to create
                        var newUser = new UserModel(UserModel.AccountType.Google, UserModel.UserType.Standard)
                        {
                            OAuthToken = _context.Token,
                            Email = dictionary["email"],
                            OAuthId = dictionary["id"],
                            //Password = model.Password,
                            DisplayName = dictionary["given_name"],
                            //PhoneNumber = model.PhoneNumber
                        };

                        success = UserService.Instance.CreateUser(newUser);
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }

            return RedirectToAction("Index", "Main");
        }
    }
}