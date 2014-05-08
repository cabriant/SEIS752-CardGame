using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEIS752CardGame.Business.Models
{
	public class GoogleOauthConfig
	{
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string CallbackUrl { get; set; }
		public string Scope { get; set; }
		public string SuccessUrl { get; set; }
		public string ErrorUrl { get; set; }
	}
}
