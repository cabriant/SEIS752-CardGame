using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEIS752CardGame.Business.Models
{
	public class DealResult
	{
		public List<string> CardContainer { get; set; }
		public DeckModel NewDeck { get; set; }
	}
}
