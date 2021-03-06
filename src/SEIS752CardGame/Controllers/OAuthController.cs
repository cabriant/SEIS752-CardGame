﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SEIS752CardGame.Models;
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

        [HttpGet]
        public ActionResult Login(OAuthType type, string redirect)
        {
	        if (IsUserAuthenticated())
	        {
				if (!string.IsNullOrEmpty(redirect))
					return Redirect("/" + redirect);
				return RedirectToAction("Index", "Main");
	        }

	        var redirectUrl = string.Empty;

            switch (type)
            {
                case OAuthType.Google:
		            var googleOauthConfig = ConfigurationService.Instance.GetGoogleOauthConfig();

					redirectUrl = GoogleOauthService.Instance.CreateInitialAuthUrl(googleOauthConfig.ClientId, googleOauthConfig.CallbackUrl, (string.IsNullOrEmpty(redirect) ? "" : redirect), googleOauthConfig.Scope);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

	        return Redirect(!string.IsNullOrEmpty(redirectUrl) ? redirectUrl : "/");
        }

        [HttpGet]
        public ActionResult Auth(string state, string code, string error)
        {
			if (IsUserAuthenticated())
			{
				if (!string.IsNullOrEmpty(state))
					return Redirect("/" + state);
				return RedirectToAction("Index", "Main");
			}

	        if (!string.IsNullOrEmpty(error) && error == "access_denied")
			{
				if (!string.IsNullOrEmpty(state))
					return Redirect("/login/" + state);
				return Redirect("/login");
	        }

			var errors = new List<string>();
	        try
	        {
				var googleOauthConfig = ConfigurationService.Instance.GetGoogleOauthConfig();
				var response = GoogleOauthService.Instance.RetrieveAccessToken(code, googleOauthConfig.ClientId,
					googleOauthConfig.ClientSecret, googleOauthConfig.CallbackUrl);
				if (response == null)
					throw new Exception("Unable to authenticate with Google. Please try again later.");

				var profile = GoogleOauthService.Instance.RetrieveUserProfile(response.access_token);
				if (profile == null)
					throw new Exception("Unable to authenticate with Google. Please try again later.");

				var oauthAccessToken = response.access_token;
				var oauthUserId = profile.id;
				var userEmail = profile.email;
				var userName = profile.given_name;
				var verifiedEmail = profile.verified_email;

				if (!verifiedEmail)
				{
					throw new Exception("Your email address must be verified by Google");
				}

				// try authenticate
				var user = UserService.Instance.AuthenticateOAuthUser(userEmail, oauthUserId, oauthAccessToken);
				if (user == null)
				{
					// Couldn't find the user, so create an account
					if (UserService.Instance.CheckEmailInUse(userEmail))
					{
						throw new Exception("Email address is already in use, please use a different account to log in");
					}

					var newUser = new UserModel(UserModel.AccountType.Google, UserModel.UserType.Standard)
					{
						OAuthToken = oauthAccessToken,
						Email = userEmail,
						OAuthId = oauthUserId,
						DisplayName = userName
					};

					var success = UserService.Instance.CreateUser(newUser);
					if (success)
					{
						user = UserService.Instance.AuthenticateOAuthUser(userEmail, oauthUserId, oauthAccessToken);
					}
				}

				if (user == null)
					throw new Exception("Unable to authenticate with Google. Please try again later.");

				SessionDataPersistor.Instance.StoreInSession(SessionDataPersistor.SessionKey.UserKey, user);
				return (!string.IsNullOrEmpty(state) ? Redirect("/" + state) : Redirect("/"));
	        }
	        catch (Exception e)
	        {
				errors.Add(e.Message);
	        }
			
			TempData["OauthErrors"] = errors;

	        return RedirectToAction("Error");
        }

	    public ActionResult Error()
	    {
		    var errors = (List<string>)TempData["OauthErrors"];

		    return View(new OauthErrorModel { Errors = errors });
	    }
    }
}