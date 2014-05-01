using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
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
			var newUser = new UserModel(UserModel.AccountType.Local, UserModel.UserType.Standard)
			{
				Email = model.Email,
				Password = model.Password,
				DisplayName = model.DisplayName,
				PhoneNumber = model.PhoneNumber
			};

			var success = UserService.Instance.CreateUser(newUser);
			
			return new AccountCreateResponse() { Success = success };
		}
	}
}