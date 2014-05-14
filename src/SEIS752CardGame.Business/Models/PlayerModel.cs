using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SEIS752CardGame.Business.Models
{
	public class PlayerModel
	{
		private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects };

		public PlayerModel(player_game player)
		{
			DisplayName = player.user.display_name;
			BetValue = (player.ante_bet.HasValue ? player.ante_bet.Value : 0);
			CashValue = player.user.account_value;
			Id = player.user_id;
			HasAnted = player.has_anted_bet;
			JsonHands = player.player_hand;
			JsonPlayerActions = player.player_actions;
			IsTurn = false;
		}

		public string DisplayName { get; set; }
		public int BetValue { get; set; }
		public int CashValue { get; set; }
		public string Id { get; set; }
		public bool HasAnted { get; set; }
		public string JsonHands { get; private set; }
		public string JsonPlayerActions { get; private set; }
		public bool IsTurn { get; set; }

		private HandCollection _hands = null;

		public HandCollection Hands
		{
			get
			{
				if (JsonHands == null)
					return null;
				return (_hands ?? (_hands = JsonConvert.DeserializeObject<HandCollection>(JsonHands, JsonSettings)));
			}
			set
			{
				_hands = value;
				JsonHands = JsonConvert.SerializeObject(_hands, JsonSettings);
			}
		}

		private PlayerActions _playerActions = null;

		public PlayerActions PlayerActions
		{
			get
			{
				if (JsonPlayerActions == null)
					return null;
				return (_playerActions ?? (_playerActions = JsonConvert.DeserializeObject<PlayerActions>(JsonPlayerActions, JsonSettings)));
			}
			set
			{
				_playerActions = value;
				JsonPlayerActions = JsonConvert.SerializeObject(_playerActions, JsonSettings);
			}
		}
	}
}
