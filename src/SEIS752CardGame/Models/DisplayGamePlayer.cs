using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Models
{
	public class DisplayGamePlayer
	{
		public string Id { get; set; }
		public string DisplayName { get; set; }
		public int CashValue { get; set; }
		public int BetValue { get; set; }
		public List<HandModel> Hands { get; set; }
		public bool IsMe { get; set; }
		public bool IsDealer { get; set; }
		public List<HandInfo> HandValue { get; set; }
		public bool HasAnted { get; set; }
		public List<string> Actions { get; set; }
		public bool IsTurn { get; set; }

		// For texas holdem and regular poker
		//public IBasicPokerHand PokerHand { get; set; }
	}

	public class HandInfo
	{
		// Blackjack
		public int HandValue { get; set; }
		public string HandOutcome { get; set; }
	}
}