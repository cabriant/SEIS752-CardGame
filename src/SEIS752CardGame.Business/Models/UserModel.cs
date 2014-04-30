using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEIS752CardGame.Business.Database;

namespace SEIS752CardGame.Business.Models
{
	public class UserModel
	{
		public enum UserType
		{
			Standard = 0,
			Admin
		}

		public enum AccountType
		{
			Local = 0,
			Google
		}

		public UserModel(user u)
		{
			Id = u.user_id;
			AcctType = (AccountType) u.account_type;
			Email = u.email;
			OAuthId = u.oauth_user_id;
			Password = u.user_pwd;
			DisplayName = u.display_name;
			PhoneNumber = u.phone_number;
			Type = (UserType) u.user_type;
			OAuthToken = u.oauth_auth_token;
			OAuthRefreshToken = u.oauth_refresh_token;
		}

		public string Id { get; set; }
		public AccountType AcctType { get; set; }
		public string Email { get; set; }
		public string OAuthId { get; set; }
		public string Password { get; set; }
		public string DisplayName { get; set; }
		public string PhoneNumber { get; set; }
		public UserType Type { get; set; }
		public string OAuthToken { get; set; }
		public string OAuthRefreshToken { get; set; }
	}
}
