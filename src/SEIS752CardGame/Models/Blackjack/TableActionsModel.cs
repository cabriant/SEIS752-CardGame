using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEIS752CardGame.Models.Blackjack
{
	public class TableActionsModel
	{
		public string TableId { get; set; }
		public string GameId { get; set; }
		public int Bet { get; set; }
	}
}