using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEIS752CardGame.Models.User
{
	public class AccessResponse : BaseApiResponse
	{
		public bool Create { get; set; }
		public bool Edit { get; set; }
		public bool Delete { get; set; }
		public bool Read { get; set; }
	}
}