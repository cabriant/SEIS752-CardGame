using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SEIS752CardGame.Business.Models
{
	public class GameModel
	{
		private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects };

		public GameModel(game game)
		{
			Id = game.game_id;
			TableId = game.table_id;
			Completed = game.completed;
			JsonGameInfo = game.game_info;
		}

		public GameModel()
		{
			
		}

		public string Id { get; set; }
		public string TableId { get; set; }
		public bool Completed { get; set; }
		public string JsonGameInfo { get; private set; }

		private GameInfo _gameInfo = null;
		public GameInfo GameInfo
		{
			get
			{
				return _gameInfo ?? (_gameInfo = JsonConvert.DeserializeObject<GameInfo>(JsonGameInfo, JsonSettings));
			}
			set
			{
				_gameInfo = value;
				JsonGameInfo = JsonConvert.SerializeObject(_gameInfo, JsonSettings);
			}
		}

		public List<PlayerModel> Players { get; set; } 

		// Proxies to GameInfo updating
		public int GameState
		{
			get { return GameInfo.GameState; }
			set
			{
				var gi = GameInfo;
				gi.GameState = value;
				GameInfo = gi;
			}
		}

		public List<string> HouseCards
		{
			get { return GameInfo.HouseCards; }
			set
			{
				var gi = GameInfo;
				gi.HouseCards = value;
				GameInfo = gi;
			}
		}

		public int TablePotValue
		{
			get { return GameInfo.TablePotValue; }
			set
			{
				var gi = GameInfo;
				gi.TablePotValue = value;
				GameInfo = gi;
			}
		}

		public int CurrentRoundBet
		{
			get { return GameInfo.CurrentRoundBet; }
			set
			{
				var gi = GameInfo;
				gi.CurrentRoundBet = value;
				GameInfo = gi;
			}
		}

		public List<string> RequiredPlayerActions
		{
			get { return GameInfo.RequiredPlayerActions; }
			set
			{
				var gi = GameInfo;
				gi.RequiredPlayerActions = value;
				GameInfo = gi;
			}
		}

		public string LastAction
		{
			get { return GameInfo.LastAction; }
			set
			{
				var gi = GameInfo;
				gi.LastAction = value;
				GameInfo = gi;
			}
		}

		public int LastRaiseValue
		{
			get { return GameInfo.LastRaiseValue; }
			set
			{
				var gi = GameInfo;
				gi.LastRaiseValue = value;
				GameInfo = gi;
			}
		}

	}
}
