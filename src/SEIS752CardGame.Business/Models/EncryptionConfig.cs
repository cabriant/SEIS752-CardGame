using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEIS752CardGame.Business.Models
{
	public class EncryptionConfig
	{
		public string Password { get; set; }
		public string IV { get; set; }
		public string Hash { get; set; }
		public int KeySize { get; set; }
		public string Salt { get; set; }
		public int Iterations { get; set; }
	}
}
