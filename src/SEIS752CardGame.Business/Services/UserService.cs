using System;
using System.Linq;
using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Business.Services
{
	public class UserService : BaseService<UserService, IUserService>, IUserService
	{
		private const int CODE_EXPIRATION_SECONDS = -120; // 2 minute expiration

		public UserModel GetUser(string userId)
		{
			var context = Utilities.Database.GetContext();
			var user = (from aUser in context.users
						  where aUser.user_id == userId
						  select aUser).FirstOrDefault();

			return (user != null ? new UserModel(user) : null);
		}

		public UserModel AuthenticateUser(string email, string password)
		{
			var context = Utilities.Database.GetContext();
			var user = (from aUser in context.users
						where aUser.email == email
						&& aUser.user_pwd == password
						select aUser).FirstOrDefault();

			return (user == null ? null : new UserModel(user));
		}

        public UserModel AuthenticateOAuthUser(string email, string oauthAuthToken)
        {
            var context = Utilities.Database.GetContext();
            var user = (from aUser in context.users
                        where aUser.email == email
                        && aUser.oauth_auth_token == oauthAuthToken
                        select aUser).FirstOrDefault();

            return (user == null ? null : new UserModel(user));
        }

		public bool CreateUser(UserModel model)
		{
			var newUser = new user()
			{
				user_id = Guid.NewGuid().ToString("N"),
				account_type = (int)model.AcctType,
				email = model.Email,
				oauth_user_id = model.OAuthId,
				user_pwd = model.Password,
				display_name = model.DisplayName,
				phone_number = model.PhoneNumber,
				user_type = (int)model.Type,
				oauth_auth_token = model.OAuthToken,
				oauth_refresh_token = model.OAuthRefreshToken,
				account_value = 5000
			};

			var context = Utilities.Database.GetContext();
			context.users.Add(newUser);

			var success = false;
			try
			{
				 success = (1 == context.SaveChanges());
			}
			catch (InvalidOperationException e)
			{
				//throw;
			}

			return success;
		}

		public bool UpdateUser(UserModel model)
		{
			var context = Utilities.Database.GetContext();
			var user = (from aUser in context.users
						where aUser.user_id == model.Id
						select aUser).FirstOrDefault();

			if (user == null)
				return false;

			user.email = model.Email;
			user.user_pwd = model.Password;
			user.display_name = model.DisplayName;
			user.phone_number = model.PhoneNumber;

			var success = false;
			try
			{
				 success = (1 == context.SaveChanges());
			}
			catch (InvalidOperationException e)
			{
				//throw;
			}

			return success;
		}

		public bool CheckEmailInUse(string email)
		{
			var context = Utilities.Database.GetContext();
			var user = (from aUser in context.users
						where aUser.email == email
						select aUser).FirstOrDefault();

			return (user != null);
		}

		public bool CheckDisplayNameInUse(string displayName)
		{
			var context = Utilities.Database.GetContext();
			var user = (from aUser in context.users
						where aUser.display_name == displayName
						select aUser).FirstOrDefault();

			return (user != null);
		}

		public bool UpdateUserCashValue(string userId, int amtAddSub)
		{
			var context = Utilities.Database.GetContext();
			var user = (from u in context.users
						where u.user_id == userId
						select u).SingleOrDefault();

			if (user == null) 
				return false;

			var success = false;
			try
			{
				user.account_value += amtAddSub;
				success = (1 == context.SaveChanges());
			}
			catch (Exception)
			{
				//throw;
			}
			return success;
		}

		public int GetUserAccountValue(string userId)
		{
			var context = Utilities.Database.GetContext();
			var user = (from u in context.users
						  where u.user_id == userId
						  select u).SingleOrDefault();

			return (user != null ? user.account_value : 0);
		}

		public void CreateAndSendResetCode(string email)
		{
			var context = Utilities.Database.GetContext();
			var user = (from u in context.users
						where u.email == email
						select u).SingleOrDefault();

			if (user == null || string.IsNullOrEmpty(user.phone_number))
				return;

			var codesToInvalidate = (from c in context.user_pwd_reset
									where c.user_id == user.user_id
										  && c.is_code_valid
										  || c.is_token_valid
									select c).ToList();

			foreach (var codeToInvalidate in codesToInvalidate)
			{
				codeToInvalidate.is_code_valid = false;
				codeToInvalidate.is_token_valid = false;
			}

			var code = Guid.NewGuid().ToString("N").ToUpper().Substring(0, 6);

			var pwdResetRequest = new user_pwd_reset()
			{
				user_id = user.user_id,
				code_sent_to = user.phone_number,
				verification_code = code,
				is_code_valid = true,
				sent_date = DateTime.Now
			};

			context.user_pwd_reset.Add(pwdResetRequest);
			
			var success = false;
			try
			{
				// Save new code and invalidate others
				success = (1 == context.SaveChanges());
			}
			catch (Exception)
			{
				throw;
			}

			if (!success)
				return;
			
			// Send the code to the user's phone number

		}

		public string ValidateCodeAndCreateToken(string email, string code)
		{
			var context = Utilities.Database.GetContext();
			var user = (from u in context.users
						where u.email == email
						select u).SingleOrDefault();

			if (user == null)
				return null;

			var passwordResetRequest = (from r in context.user_pwd_reset
										where r.user_id == user.user_id
											  && r.is_code_valid
											  && r.verification_code != null
											  && r.verification_token == null
										select r).SingleOrDefault();

			// No valid rows
			if (passwordResetRequest == null)
				return null;

			if (passwordResetRequest.sent_date < (DateTime.Now.AddSeconds(CODE_EXPIRATION_SECONDS)) ||
			    passwordResetRequest.verification_code != code.Trim().ToUpper())
				return null;

			var token = Guid.NewGuid().ToString("N").ToUpper();

			passwordResetRequest.is_code_valid = false;
			passwordResetRequest.code_validation_date = DateTime.Now;
			passwordResetRequest.verification_token = token;
			passwordResetRequest.is_token_valid = true;

			try
			{
				context.SaveChanges();
			}
			catch (Exception)
			{
				throw;
			}

			return token;
		}

		public bool ValidateTokenAndResetPassword(string email, string token, string newPassword)
		{
			var context = Utilities.Database.GetContext();
			var user = (from u in context.users
						where u.email == email
						select u).SingleOrDefault();

			if (user == null)
				return false;

			var passwordResetRequest = (from r in context.user_pwd_reset
										where r.user_id == user.user_id
											  && r.is_code_valid == false
											  && r.is_token_valid
											  && r.verification_token != null
										select r).SingleOrDefault();

			// No valid rows
			if (passwordResetRequest == null)
				return false;

			if (passwordResetRequest.verification_token != token.Trim().ToUpper())
				return false;

			passwordResetRequest.token_validation_date = DateTime.Now;
			passwordResetRequest.is_token_valid = false;

			user.user_pwd = newPassword;

			var success = false;
			try
			{
				success = (1 == context.SaveChanges());
			}
			catch (Exception)
			{
				throw;
			}

			return success;
		}
	}
}
