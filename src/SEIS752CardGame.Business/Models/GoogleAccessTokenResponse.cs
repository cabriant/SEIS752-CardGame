using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEIS752CardGame.Business.Models
{
	public class GoogleAccessTokenResponse
	{
		public string access_token { get; set; }
		public string refresh_token { get; set; }
		public string expires_in { get; set; }
		public string id_token { get; set; }
	}
}
