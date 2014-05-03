using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEIS752CardGame.Models
{
	public abstract class BaseApiResponse
	{
		public bool Success { get; set; }
		public List<string> Errors { get; set; }
	}
}