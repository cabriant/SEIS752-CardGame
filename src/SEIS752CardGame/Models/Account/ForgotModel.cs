using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEIS752CardGame.Models.Account
{
	public class ForgotModel
	{
		public string Email { get; set; }
		public string Code { get; set; }
		public string Token { get; set; }
		public string NewPassword { get; set; }
		public string NewPasswordConfirm { get; set; }
	}
}