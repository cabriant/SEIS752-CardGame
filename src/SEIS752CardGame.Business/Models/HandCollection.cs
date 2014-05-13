using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEIS752CardGame.Business.Models
{
	public class HandCollection
	{
		public List<HandModel> HandList { get; set; }
	}

	public class HandModel
	{
		public bool Folded { get; set; }
		public bool Done { get; set; }
		public List<string> Cards { get; set; }
	}
}
