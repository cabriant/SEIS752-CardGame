using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEIS752CardGame.Models.Blackjack
{
	public class BlackjackTableCreateModel
	{
		public string Name { get; set; }
		public int? Ante { get; set; }
		public int? MaxRaise { get; set; }
		public int MaxPlayers { get; set; }
	}
}