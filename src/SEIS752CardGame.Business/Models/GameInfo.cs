using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEIS752CardGame.Business.Models
{
	public enum BlackjackCurrentState
	{
		Betting = 0,
		Dealing,
		RoundTwo,
		Finished,
		InProg
	}

	//public enum TexasHoldemCurrentState
	//{
	//	Ante,
	//	Dealing,
	//	BettingRoundOne,
	//	Flop,
	//	BettingRoundTwo,
	//	Turn,
	//	BettingRoundThree,
	//	River,
	//	BettingRoundFour,
	//	Finished,
	//	FinishedAndPaid,
	//	InProg
	//}

	//public enum FiveCardDrawCurrentState
	//{
	//	Ante,
	//	Dealing,
	//	BettingRoundOne,
	//	Discard,
	//	BettingRoundTwo,
	//	Finished,
	//	FinishedAndPaid,
	//	InProg
	//}

	public class GameInfo
	{
		public GameInfo()
		{
			GameState = 0;
			HouseCards = new List<string>();
			TablePotValue = 0;
			CurrentRoundBet = 0;
			RequiredPlayerActions = new List<string>();
			LastRaiseValue = 0;
		}

		public int GameState { get; set; }
		public List<string> HouseCards { get; set; }
		public int TablePotValue { get; set; }
		public int CurrentRoundBet { get; set; }
		public List<string> RequiredPlayerActions { get; set; }
		public string LastAction { get; set; }
		public int LastRaiseValue { get; set; }
	}
}
