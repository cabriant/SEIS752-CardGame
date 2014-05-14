using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using PhoneNumbers;
using SEIS752CardGame.Business.Models;
using SEIS752CardGame.Business.Services;
using SEIS752CardGame.Models.Account;
using SEIS752CardGame.Utilities;
using Twilio;

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
			string code = null;
			string phoneNumber = null;
			if (!string.IsNullOrEmpty(model.Email))
				code = UserService.Instance.CreateResetCode(model.Email, out phoneNumber);
			else
				errors.Add("Please enter an email address.");

			if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(phoneNumber))
			{
				// Send the code
				var twilioConfig = ConfigurationService.Instance.GetTwilioConfiguration();

				var twilioClient = new TwilioRestClient(twilioConfig.AccountSid, twilioConfig.AuthToken);
				var message = twilioClient.SendSmsMessage(twilioConfig.PhoneNumber, phoneNumber, "Card Game: Your password reset code is " + code);
			}

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

        [HttpGet, AuthenticatedApiRequest]
	    public AccountValueResponse GetUserCashValue()
	    {
	        return new AccountValueResponse {UserCashValue = UserService.Instance.GetUserAccountValue(CurrentUser.Id)};
	    }

		[HttpPost, AuthenticatedApiRequest]
		public AccountValueUpdateResponse UpdateAccountValue([FromBody] AccountValueUpdateModel model)
		{
			var errors = new List<string>();
			var success = false;
			var newAccountValue = 0;
			if (model.AddCash <= 0)
				errors.Add("You must add more than $0 to your account");

			if (!errors.Any())
				 success = UserService.Instance.UpdateUserCashValue(CurrentUser.Id, model.AddCash);

			if (success)
				newAccountValue = UserService.Instance.GetUserAccountValue(CurrentUser.Id);

			return new AccountValueUpdateResponse {Success = success, NewValue = newAccountValue, Errors = errors};
		}
	}
}