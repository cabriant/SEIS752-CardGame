using System.Collections.Generic;
using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Business.Services
{
	public interface IGameService
	{
		GameModel GetCurrentGameForTable(string tableId, TableModel.PokerGameType tableType);
		GameModel GetGameById(string gameId);
		GameModel CreateGameForTable(string tableId, TableModel.PokerGameType tableType);
		void AddUserToGame(string userId, string gameId);
		bool GetIsPlayerInGame(string userId, string gameId);
		string PlaceBetForPlayer(string userId, string gameId, int bet);
		void UpdateGamePotValueAndCurrentBet(string gameId, int tablePotValue, int currentBet);
		string UpdatePlayerBetForSplitHand(string userId, string gameId);
		bool PlaceAnteForPlayerInGame(string userId, string gameId, int ante);
		void SetGameStage(string gameId, int newState);
		void SetGameCompleted(string gameId);
		bool UpdatePlayerWinnings(string userId, string gameId, int totalWinnings);
		void UpdateHouseCards(string gameId, List<string> cards);
		void UpdatePlayerCards(string userId, string gameId, List<HandModel> hands);
		bool CheckIsPlayersTurn(string gameId, string tableId, TableModel.PokerGameType tableType, string userId);
		void ClearGameCurrentBet(string gameId);
		void ClearGamePlayersBet(string gameId);
		void SetPlayerOrderForGame(string gameId, List<string> userIds);
		void SetGameLastRaise(string gameId, int raiseValue);
		void SetGameLastAction(string gameId, string lastAction);
	}
}