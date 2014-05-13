using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEIS752CardGame.Models.Blackjack
{
	public enum BlackjackGameStage
	{
		InitialBet,
		WaitingForOtherBets,
		DealCards,
		WaitingForOtherPlayerTurns,
		MyTurn,
		DealersTurn,
		Finished,
		Observing
	}

	public class BlackjackGameModel
	{
		public BlackjackGameStage CurrentStage { get; set; }
		public string Table { get; set; }
		public string CurrentGame { get; set; }
		public DisplayGamePlayer Dealer { get; set; }
		public List<DisplayGamePlayer> Players { get; set; }
		public bool CurrentPlayerCanSplitHand { get; set; }
		public int CurrentPlayersCashValue { get; set; }
	}
}