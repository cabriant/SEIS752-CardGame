using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SEIS752CardGame.Business.Models
{
	public class TableModel
	{
		private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects };

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
			JsonDeck = table.table_deck;
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
		public int NumOfPlayers { get; set; }
		public string JsonDeck { get; set; }

		private DeckModel _deck = null;

		public DeckModel Deck
		{
			get
			{
				if (JsonDeck == null)
					return null;
				return (_deck ?? (_deck = JsonConvert.DeserializeObject<DeckModel>(JsonDeck, JsonSettings)));
			}
			set
			{
				_deck = value;
				JsonDeck = JsonConvert.SerializeObject(_deck, JsonSettings);
			}
		}
	}
}
