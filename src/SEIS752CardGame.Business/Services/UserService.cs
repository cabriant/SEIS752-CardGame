using System;
using System.Linq;
using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Business.Services
{
	public class UserService : BaseService<UserService, IUserService>, IUserService
	{
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
				oauth_refresh_token = model.OAuthRefreshToken
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
	}
}
