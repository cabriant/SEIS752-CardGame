using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEIS752CardGame.Business.Models
{
	public class TableModel
	{
		public enum PokerGameType
		{
			Blackjack,
			FiveCardDraw
		}

		public TableModel(poker_table table)
		{
			Id = table.table_id;
			GameType = (PokerGameType) table.table_game_type;
			DisplayName = table.table_disp_name;
			Ante = (table.ante.HasValue ? table.ante.Value : -1);
			MaxRaise = (table.max_raise.HasValue ? table.max_raise.Value : -1);
			MaxPlayers = table.max_players;
			Deck = table.table_deck;
			NumOfPlayers = table.users.Count;
		}

		public TableModel(PokerGameType gameType)
		{
			GameType = gameType;
		}

		public string Id { get; set; }
		public PokerGameType GameType { get; set; }
		public string DisplayName { get; set; }
		public int? Ante { get; set; }
		public int? MaxRaise { get; set; }
		public int MaxPlayers { get; set; }
		public string Deck { get; set; }
		public int NumOfPlayers { get; set; }
	}
}
