using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEIS752CardGame.Models.Account
{
	public class AccountValueUpdateResponse : BaseApiResponse
	{
		public int NewValue { get; set; }
	}
}