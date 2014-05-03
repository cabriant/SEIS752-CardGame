using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEIS752CardGame.Models.Account
{
	public class ForgotResponse
	{
		public bool Success { get; set; }
		public string Token { get; set; }
		public List<string> Errors { get; set; } 
	}
}