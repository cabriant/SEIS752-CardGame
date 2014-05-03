using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using PhoneNumbers;
using SEIS752CardGame.Business.Models;
using SEIS752CardGame.Business.Services;
using SEIS752CardGame.Models.Account;

namespace SEIS752CardGame.Controllers
{
	public class AccountServiceController : BaseApiController
	{
		[HttpPost]
		public AccountCreateResponse Create([FromBody]AccountCreateModel model)
		{
			var errors = new List<string>();
			if (!string.IsNullOrEmpty(model.Email) && UserService.Instance.CheckEmailInUse(model.Email))
				errors.Add("Email address is already in use");
			if (!string.IsNullOrEmpty(model.DisplayName) && UserService.Instance.CheckDisplayNameInUse(model.DisplayName))
				errors.Add("Display name is already in use");
			if (!string.IsNullOrEmpty(model.Password) && model.Password.Length < 8)
				errors.Add("Password must be greater than 7 characters in length");
			if (string.IsNullOrEmpty(model.Email))
				errors.Add("An email address is required");
			if (string.IsNullOrEmpty(model.Password))
				errors.Add("A password is required");
			if (string.IsNullOrEmpty(model.ConfirmPassword))
				errors.Add("You must confirm the original password");
			if (!string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.ConfirmPassword) && model.Password != model.ConfirmPassword)
				errors.Add("The password entered does not match");
			if (string.IsNullOrEmpty(model.DisplayName))
				errors.Add("A display name is required");

			if (!string.IsNullOrEmpty(model.PhoneNumber))
			{
				try
				{
					var number = PhoneNumberUtil.GetInstance().Parse(model.PhoneNumber, "US");
				}
				catch (Exception)
				{
					errors.Add("Phone number could not be verified");
				}
			}

			var success = false;
			if (!errors.Any())
			{
				var newUser = new UserModel(UserModel.AccountType.Local, UserModel.UserType.Standard)
				{
					Email = model.Email,
					Password = model.Password,
					DisplayName = model.DisplayName,
					PhoneNumber = model.PhoneNumber
				};

				success = UserService.Instance.CreateUser(newUser);
			}
			
			return new AccountCreateResponse() { Success = success, Errors = errors };
		}

		[HttpPost]
		public ForgotResponse Forgot([FromBody] ForgotModel model)
		{
			var errors = new List<string>();
			if (!string.IsNullOrEmpty(model.Email))
				UserService.Instance.CreateAndSendResetCode(model.Email);
			else
				errors.Add("Please enter an email address.");

			// Just tell them we sent a code, even if we didn't (for security reasons)
			return new ForgotResponse { Success = !errors.Any(), Errors = errors };
		}

		[HttpPost]
		public ForgotResponse Verify([FromBody] ForgotModel model)
		{
			string token = null;
			var errors = new List<string>();
			if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Code))
				token = UserService.Instance.ValidateCodeAndCreateToken(model.Email, model.Code);

			if (string.IsNullOrEmpty(model.Code))
				errors.Add("Please enter the code received on your mobile device.");
			else if (string.IsNullOrEmpty(token))
				errors.Add("Code is no longer valid. Please request a new code.");

			return new ForgotResponse { Success = !string.IsNullOrEmpty(token), Token = token, Errors = errors };
		}

		[HttpPost]
		public ForgotResponse Reset([FromBody] ForgotModel model)
		{
			var success = false;
			var errors = new List<string>();
			if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Token) &&
				!string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.NewPasswordConfirm))
			{
				if (model.NewPassword.Length < 8)
					errors.Add("Password must be greater than 7 characters in length");
				if (model.NewPassword != model.NewPasswordConfirm)
					errors.Add("The password entered does not match");
				
				if (!errors.Any())
					success = UserService.Instance.ValidateTokenAndResetPassword(model.Email, model.Token, model.NewPassword);
			}

			return new ForgotResponse { Success = success, Errors = errors };
		}
	}
}