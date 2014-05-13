using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEIS752CardGame.Business.Models;
using Database = SEIS752CardGame.Business.Utilities.Database;

namespace SEIS752CardGame.Business.Services
{
	public class GameService : BaseService<GameService, IGameService>, IGameService
	{
		public GameModel GetCurrentGameForTable(string tableId, TableModel.PokerGameType tableType)
		{
			var context = Database.GetContext();
			var currentGame = (from g in context.games
								where g.table_id == tableId
								&& !g.completed
								select g).FirstOrDefault();

			if (currentGame == null) 
				return null;
			
			var game = new GameModel(currentGame);

			var playerGames = currentGame.player_game.Where(pg => pg.game_id == currentGame.game_id).ToList();
			var players = new List<PlayerModel>();

			if (playerGames.Any())
			{
				players.AddRange(playerGames.Select(playerGame => new PlayerModel(playerGame)));
			}

			game.Players = players;

			return game;
		}

		public GameModel GetGameById(string gameId)
		{
			var context = Database.GetContext();
			var currentGame = (from g in context.games
							   where g.game_id == gameId
							   select g).SingleOrDefault();

			if (currentGame == null)
				return null;

			var game = new GameModel(currentGame);

			var playerGames = currentGame.player_game.Where(pg => pg.game_id == currentGame.game_id).ToList();
			var players = new List<PlayerModel>();

			if (playerGames.Any())
			{
				players.AddRange(playerGames.Select(playerGame => new PlayerModel(playerGame)));
			}

			game.Players = players;

			return game;
		}

		public GameModel CreateGameForTable(string tableId, TableModel.PokerGameType tableType)
		{
			var gameModel = new GameModel()
			{
				Id = Guid.NewGuid().ToString("N"),
				TableId = tableId,
				Completed = false,
				GameInfo = new GameInfo()
			};

			var game = new game
			{
				game_id = gameModel.Id,
				table_id = gameModel.TableId, 
				completed = gameModel.Completed,
				game_info = gameModel.JsonGameInfo
			};

			var context = Database.GetContext();
			context.games.Add(game);
			context.SaveChanges();

			return new GameModel(game);
		}

		public void AddUserToGame(string userId, string gameId)
		{
			var context = Database.GetContext();
			var game = (from g in context.games
						where g.game_id == gameId
						select g).SingleOrDefault();
			var user = (from u in context.users
						  where u.user_id == userId
						  select u).SingleOrDefault();

			if (game != null && user != null)
			{
				user.player_game.Add(new player_game() { user = user, game = game });
				context.SaveChanges();
			}
		}

		public bool GetIsPlayerInGame(string userId, string gameId)
		{
			var context = Database.GetContext();
			var playerGame = (from pg in context.player_game
							  where pg.user_id == userId
							  && pg.game_id == gameId
							  select pg).SingleOrDefault();

			return playerGame != null;
		}

		public string PlaceBetForPlayer(string userId, string gameId, int bet)
		{
			var context = Database.GetContext();
			var playerGame = (from pg in context.player_game
							  where pg.user_id == userId
							  && pg.game_id == gameId
							  select pg).SingleOrDefault();

			var player = (from p in context.users
						  where p.user_id == userId
						  select p).SingleOrDefault();

			if (playerGame == null || player == null) 
				return "Error occurred in placing bet.";
			
			if (playerGame.ante_bet.HasValue)
				playerGame.ante_bet += bet;
			else
				playerGame.ante_bet = bet;
			player.account_value = player.account_value - bet;

			if (player.account_value >= 0)
				context.SaveChanges();
			else
			{
				var changedEntities = context.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged);
				foreach (var changedEntity in changedEntities)
				{
					changedEntity.Reload();
				}
				return "You do not have enough cash to make this bet.";
			}
			return "Bet placed successfully.";
		}

		public void UpdateGamePotValueAndCurrentBet(string gameId, int tablePotValue, int currentBet)
		{
			var context = Database.GetContext();
			var game = (from g in context.games
						where g.game_id == gameId
						select g).SingleOrDefault();

			if (game != null)
			{
				var gameModel = new GameModel(game);
				var gameInfo = gameModel.GameInfo;

				gameInfo.TablePotValue = tablePotValue;
				gameInfo.CurrentRoundBet = currentBet;

				gameModel.GameInfo = gameInfo;

				game.game_info = gameModel.JsonGameInfo;

				context.SaveChanges();
			}
		}

		public string UpdatePlayerBetForSplitHand(string userId, string gameId)
		{
			var context = Database.GetContext();
			var playerGame = (from pg in context.player_game
							  where pg.user_id == userId
							  && pg.game_id == gameId
							  select pg).SingleOrDefault();

			var player = (from p in context.users
						  where p.user_id == userId
						  select p).SingleOrDefault();

			if (playerGame != null && player != null)
			{
				var origBet = playerGame.ante_bet;

				playerGame.ante_bet = (2 * origBet);
				player.account_value = player.account_value - origBet.Value;

				if (player.account_value >= 0)
					context.SaveChanges();
				else
				{
					var changedEntities = context.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged);
					foreach (var changedEntity in changedEntities)
					{
						changedEntity.Reload();
					}
					return "You do not have enough cash to make this bet.";
				}
				return "Bet placed successfully.";
			}
			return "Error occurred in placing bet.";
		}

		public bool PlaceAnteForPlayerInGame(string userId, string gameId, int ante)
		{
			var context = Database.GetContext();
			var playerGame = (from pg in context.player_game
							  where pg.user_id == userId
							  && pg.game_id == gameId
							  select pg).SingleOrDefault();

			var player = (from p in context.users
						  where p.user_id == userId
						  select p).SingleOrDefault();

			var game = (from g in context.games
						where g.game_id == gameId
						select g).SingleOrDefault();

			if (playerGame != null && player != null && game != null)
			{
				playerGame.has_anted_bet = true;
				player.account_value = player.account_value - ante;
				
				var gameModel = new GameModel(game);
				var gameInfo = gameModel.GameInfo;

				gameInfo.TablePotValue += ante;

				gameModel.GameInfo = gameInfo;

				game.game_info = gameModel.JsonGameInfo;

				if (player.account_value >= 0)
				{
					context.SaveChanges();
					return true;
				}
				var changedEntities = context.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged);
				foreach (var changedEntity in changedEntities)
				{
					changedEntity.Reload();
				}
			}
			return false;
		}

		public void SetGameStage(string gameId, int newState)
		{
			var context = Database.GetContext();
			var game = (from g in context.games
						where g.game_id == gameId
						select g).SingleOrDefault();

			if (game != null)
			{
				var gameModel = new GameModel(game);
				var gameInfo = gameModel.GameInfo;

				gameInfo.GameState = newState;
				gameModel.GameInfo = gameInfo;

				game.game_info = gameModel.JsonGameInfo;
				context.SaveChanges();
			}
		}

		public bool UpdatePlayerWinnings(string userId, string gameId, int totalWinnings)
		{
			var context = Database.GetContext();
			var playerGame = (from pg in context.player_game
							  where pg.user_id == userId
							  && pg.game_id == gameId
							  select pg).SingleOrDefault();

			if (playerGame != null)
			{
				if (!playerGame.amt_won_lost.HasValue)
				{
					playerGame.amt_won_lost = totalWinnings;
					context.SaveChanges();
					return true;
				}
			}
			return false;
		}

		public void UpdateHouseCards(string gameId, List<string> cards)
		{
			var context = Database.GetContext();
			var game = (from g in context.games
						where g.game_id == gameId
						select g).SingleOrDefault();

			if (game != null)
			{
				var gameModel = new GameModel(game);
				var gameInfo = gameModel.GameInfo;
				gameInfo.HouseCards = cards;
				gameModel.GameInfo = gameInfo;
				game.game_info = gameModel.JsonGameInfo;

				context.SaveChanges();
			}
		}

		public void UpdatePlayerCards(string userId, string gameId, List<HandModel> hands)
		{
			var context = Database.GetContext();
			var playerGame = (from pg in context.player_game
							  where pg.user_id == userId
							  && pg.game_id == gameId
							  select pg).SingleOrDefault();

			if (playerGame != null)
			{
				var playerModel = new PlayerModel(playerGame);
				playerModel.Hands = new HandCollection() { HandList = hands };

				playerGame.player_hand = playerModel.JsonHands;
				context.SaveChanges();
			}
		}

		public bool CheckIsPlayersTurn(string gameId, string tableId, TableModel.PokerGameType tableType, string userId)
		{
			var game = GetCurrentGameForTable(tableId, tableType);

			if (game == null) 
				return false;

			if (tableType == TableModel.PokerGameType.Blackjack)
			{
				var firstPlayer = game.Players.FirstOrDefault(p => p.Hands.HandList.Any(h => h.Cards.Any() && !h.Folded && !h.Done));
				if (firstPlayer != null)
					return (firstPlayer.Id == userId);
			}
			//else if (tableType == TableType.FiveCardDraw)
			//{
			//	var currentPlayerId = game.ReqPlayerActions.FirstOrDefault();
			//	if (currentPlayerId != null)
			//		return (currentPlayerId == playerId);
			//}
			//else if (tableType == TableType.TexasHoldem)
			//{
			//	var currentPlayerId = game.ReqPlayerActions.FirstOrDefault();
			//	if (currentPlayerId != null)
			//		return (currentPlayerId == playerId);
			//}
			return false;
		}

		public void ClearGameCurrentBet(string gameId)
		{
			var context = Database.GetContext();
			var game = (from g in context.games
						where g.game_id == gameId
						select g).SingleOrDefault();

			if (game != null)
			{
				var gameModel = new GameModel(game);
				var gameInfo = gameModel.GameInfo;
				gameInfo.CurrentRoundBet = 0;
				gameInfo.LastRaiseValue = 0;
				gameModel.GameInfo = gameInfo;
				game.game_info = gameModel.JsonGameInfo;

				context.SaveChanges();
			}
		}

		public void ClearGamePlayersBet(string gameId)
		{
			var context = Database.GetContext();
			var playerGames = (from pg in context.player_game
							   where pg.game_id == gameId
							   select pg).ToList();

			if (playerGames != null)
			{
				foreach (var playerGame in playerGames)
				{
					playerGame.ante_bet = 0;
				}
				context.SaveChanges();
			}
		}

		public void SetPlayerOrderForGame(string gameId, List<string> userIds)
		{
			var context = Database.GetContext();
			var game = (from g in context.games
						where g.game_id == gameId
						select g).SingleOrDefault();

			if (game != null)
			{
				var gameModel = new GameModel(game);
				var gameInfo = gameModel.GameInfo;
				gameInfo.RequiredPlayerActions = userIds;
				gameModel.GameInfo = gameInfo;
				game.game_info = gameModel.JsonGameInfo;

				context.SaveChanges();
			}
		}

		public void SetGameLastRaise(string gameId, int raiseValue)
		{
			var context = Database.GetContext();
			var game = (from g in context.games
						where g.game_id == gameId
						select g).SingleOrDefault();

			if (game != null)
			{
				var gameModel = new GameModel(game);
				var gameInfo = gameModel.GameInfo;
				gameInfo.LastRaiseValue = raiseValue;
				gameModel.GameInfo = gameInfo;
				game.game_info = gameModel.JsonGameInfo;
				
				context.SaveChanges();
			}
		}

		public void SetGameLastAction(string gameId, string lastAction)
		{
			var context = Database.GetContext();
			var game = (from g in context.games
						where g.game_id == gameId
						select g).SingleOrDefault();

			if (game != null)
			{
				var gameModel = new GameModel(game);
				var gameInfo = gameModel.GameInfo;
				gameInfo.LastAction = lastAction;
				gameModel.GameInfo = gameInfo;
				game.game_info = gameModel.JsonGameInfo;
				
				context.SaveChanges();
			}
		}
	}
}
