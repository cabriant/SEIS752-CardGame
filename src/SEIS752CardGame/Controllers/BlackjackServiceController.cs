using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SEIS752CardGame.Business;
using SEIS752CardGame.Business.Models;
using SEIS752CardGame.Business.Services;
using SEIS752CardGame.Business.Utilities;
using SEIS752CardGame.Models;
using SEIS752CardGame.Models.Blackjack;
using SEIS752CardGame.Utilities;
using System.Web.Http;

namespace SEIS752CardGame.Controllers
{
	[AuthenticatedApiRequest]
	public class BlackjackServiceController : BaseApiController
	{
		[HttpGet]
		public BlackjackTableResponse Tables()
		{
			var tables = TableService.Instance.GetTablesForTableType(TableModel.PokerGameType.Blackjack);

			return new BlackjackTableResponse
			{
				Success = true,
				Tables = tables
			};
		}

		[HttpPost, AdminAuthApiRequest]
		public BlackjackTableResponse CreateTable([FromBody]BlackjackTableCreateModel model)
		{
			var newTable = new TableModel(TableModel.PokerGameType.Blackjack)
			{
				DisplayName = model.Name,
				Ante = model.Ante,
				MaxRaise = model.MaxRaise,
				MaxPlayers = model.MaxPlayers
			};

			var success = TableService.Instance.CreateTable(newTable);
			List<TableModel> tables = null;
			if (success)
				tables = TableService.Instance.GetTablesForTableType(TableModel.PokerGameType.Blackjack);

			return new BlackjackTableResponse { Success = success, Tables = tables };
		}

		[HttpGet]
		public InitTableResponse CheckJoinTable(string id)
		{
			var errors = new List<string>();
			var success = false;

			if (!TableService.Instance.CheckUserIsAtTable(CurrentUser.Id, id))
			{
				var table = TableService.Instance.GetTableByIdForTableType(TableModel.PokerGameType.Blackjack, id);
				if (table == null)
				{
					errors.Add("The table no longer exists.");
				}
				else if (table.NumOfPlayers >= table.MaxPlayers)
				{
					errors.Add("The table is full.");
				}
				else
				{
					success = TableService.Instance.AddUserToTable(CurrentUser.Id, id);
				}
			}
			else
			{
				success = true;
			}

			return new InitTableResponse { Success = success, Errors = errors };
		}

		[HttpGet]
		public InitTableResponse Table(string id)
		{
			var table = TableService.Instance.GetTableByIdForTableType(TableModel.PokerGameType.Blackjack, id);
			if (table == null)
				return new InitTableResponse {Success = false, Errors = new List<string> {"Table could not be found."}};
			
			var model = new BlackjackGameModel()
			{
				Table = id
			};

			// Get the current game being played at the table
			var currentActiveGame = GameService.Instance.GetCurrentGameForTable(id, TableModel.PokerGameType.Blackjack);
			if (currentActiveGame != null)
			{
				// If the game is currently in the betting stage and the user is not already in the game, add them
				model.CurrentGame = currentActiveGame.Id;
				if (currentActiveGame.GameInfo.GameState == (int)BlackjackCurrentState.Betting)
				{
					if (!GameService.Instance.GetIsPlayerInGame(CurrentUser.Id, currentActiveGame.Id))
						GameService.Instance.AddUserToGame(CurrentUser.Id, currentActiveGame.Id);
				}
				//else
				//{
				//	if (!GameService.Instance.GetIsPlayerInGame(CurrentUser.Id, currentActiveGame.Id))
				//		model.CurrentStage = BlackjackGameStage.Observing;
				//	else

				//}

				DetermineCurrentStage(model, currentActiveGame, table);
			}
			else
			{
				// Create a new game
				var game = GameService.Instance.CreateGameForTable(id, TableModel.PokerGameType.Blackjack);
				if (game != null)
				{
					// Add player to game
					model.CurrentGame = game.Id;
					GameService.Instance.AddUserToGame(CurrentUser.Id, game.Id);
					currentActiveGame = game;
				}
			}
			model.CurrentPlayersCashValue = UserService.Instance.GetUserAccountValue(CurrentUser.Id);
			
			if (currentActiveGame.HouseCards != null)
			{
				var hc = currentActiveGame.HouseCards;
				for (var i = 1; i < hc.Count; i++)
				{
					hc[i] = "c_back";
				}
				currentActiveGame.HouseCards = hc;
			}
			else
			{
				currentActiveGame.HouseCards = new List<string>();
			}
			model.Dealer = new DisplayGamePlayer
			{
				IsDealer = true,
				DisplayName = "Dealer",
				Hands = new List<HandModel> { new HandModel() { Cards = currentActiveGame.HouseCards} },
				HandValue = new List<HandInfo>()
			};

			var playerList = new List<DisplayGamePlayer>();

			foreach (var player in currentActiveGame.Players)
			{
				var playerToAdd = new DisplayGamePlayer()
				{
					DisplayName = player.DisplayName,
					IsMe = (player.Id == CurrentUser.Id),
					BetValue = player.BetValue,
					CashValue = player.CashValue
				};

				//if (player.Hands != null)
				//	player.Hands.HandList.ForEach(p => p.Remove("^done^"));
				if (player.Hands != null)
				{
					var handValues = new List<HandInfo>();
					foreach (var hand in player.Hands.HandList)
					{
						if (hand != null && hand.Cards.Any() && !hand.Folded)
							handValues.Add(GetValueOfCards(hand.Cards));
					}
					playerToAdd.HandValue = handValues;
					if (player.Hands.HandList.All(h => h != null && h.Cards.Any() && h.Folded))
						playerToAdd.Hands = new List<HandModel>();
					else
						playerToAdd.Hands = player.Hands.HandList;
				}

				playerList.Add(playerToAdd);
			}

			model.Players = playerList;

			return new InitTableResponse() { Success = true, Game = model };
		}

		[HttpGet]
		public InitTableResponse UpdateTable(string tableId, string gameId)
		{
			var model = new BlackjackGameModel();

			var table = TableService.Instance.GetTableByIdForTableType(TableModel.PokerGameType.Blackjack, tableId);
			if (table != null)
			{
				model.Table = table.Id;

				var game = GameService.Instance.GetGameById(gameId); //.GetCurrentGameForTable(table.Id);
				if (game != null)
				{
					model.CurrentStage = BlackjackGameStage.InitialBet;
					model.CurrentGame = game.Id;
					
					DetermineCurrentStage(model, game, table);

					model.Dealer = new DisplayGamePlayer()
					{
						IsDealer = true,
						DisplayName = "Dealer"
					};

					if (game.GameInfo.GameState == (int) BlackjackCurrentState.Finished)
					{
						model.CurrentStage = BlackjackGameStage.Finished;
						model.Dealer.Hands = new List<HandModel>() { new HandModel { Done = true, Cards = game.HouseCards } };

						var handValues = new List<HandInfo>();
						handValues.Add(GetValueOfCards(game.HouseCards));
						model.Dealer.HandValue = handValues;
					}
					else
					{
						for (int i = 1; i < game.HouseCards.Count; i++)
						{
							game.HouseCards[i] = "c_back";
						}
						model.Dealer.Hands = new List<HandModel>() { new HandModel { Done = true, Cards = game.HouseCards } };
						model.Dealer.HandValue = new List<HandInfo>();
					}

					var playerList = new List<DisplayGamePlayer>();

					foreach (var player in game.Players)
					{
						var playerToAdd = new DisplayGamePlayer()
						{
							DisplayName = player.DisplayName,
							IsMe = (player.Id == CurrentUser.Id),
							BetValue = player.BetValue,
							CashValue = player.CashValue
						};

						//if (player.Hands != null)
						//	player.Hands.HandList.ForEach(p => p.Remove("^done^"));
						if (player.Hands != null)
						{
							var handValues = new List<HandInfo>();
							foreach (var hand in player.Hands.HandList)
							{
								if (hand != null && hand.Cards.Any() && !hand.Folded)
									handValues.Add(GetValueOfCards(hand.Cards));
							}
							playerToAdd.HandValue = handValues;
							if (player.Hands.HandList.All(h => h != null && h.Cards.Any() && h.Folded))
								playerToAdd.Hands = new List<HandModel>();
							else
								playerToAdd.Hands = player.Hands.HandList;
						}

						playerList.Add(playerToAdd);
					}

					model.Players = playerList;
				}
			}

			return new InitTableResponse {Success = (!string.IsNullOrEmpty(model.CurrentGame)), Game = model };
		}

		[HttpPost]
		public TableActionResponse PlaceBet([FromBody] TableActionsModel model)
		{
			var success = false;
			var errors = new List<string>();
			string result = null;
			if (model.Bet <= 0)
			{
				result = "Please place a bet greater than $0.";
			}
			else
			{
				result = GameService.Instance.PlaceBetForPlayer(CurrentUser.Id, model.GameId, model.Bet);
				success = (result == "Bet placed successfully.");
			}
			
			if (!success)
				errors.Add(result);
			return new TableActionResponse { Success = success, Errors = errors };
		}

		[HttpPost]
		public void SplitHand([FromBody] TableActionsModel model)
		{
			var table = TableService.Instance.GetTableByIdForTableType(TableModel.PokerGameType.Blackjack, model.TableId);
			if (table != null)
			{
				if (GameService.Instance.CheckIsPlayersTurn(model.GameId, table.Id, TableModel.PokerGameType.Blackjack, CurrentUser.Id))
				{
					// Handle split hand
					var game = GameService.Instance.GetCurrentGameForTable(table.Id, TableModel.PokerGameType.Blackjack);
					if (game != null && game.Id == model.GameId)
					{
						var player = game.Players.SingleOrDefault(p => p.Id == CurrentUser.Id);
						if (player != null)
						{
							if (player.Hands.HandList.Count == 1)
							{
								var currentHand = player.Hands.HandList.SingleOrDefault(h => !h.Folded);
								if (currentHand != null && currentHand.Cards.Count == 2)
								{
									var newHands = new List<HandModel>();

									var newHand1 = new List<string>() { currentHand.Cards.ElementAt(0) };
									var newHand2 = new List<string>() { currentHand.Cards.ElementAt(1) };

									newHands.Add(new HandModel { Cards = newHand1 });
									newHands.Add(new HandModel { Cards = newHand2 });

									GameService.Instance.UpdatePlayerCards(CurrentUser.Id, game.Id, newHands);

									GameService.Instance.UpdatePlayerBetForSplitHand(CurrentUser.Id, game.Id);
								}
							}
						}
					}
				}
			}
		}

		[HttpPost]
		public void StandHand([FromBody] TableActionsModel model)
		{
			var table = TableService.Instance.GetTableByIdForTableType(TableModel.PokerGameType.Blackjack, model.TableId);
			if (table != null)
			{
				if (GameService.Instance.CheckIsPlayersTurn(model.GameId, table.Id, TableModel.PokerGameType.Blackjack, CurrentUser.Id))
				{
					var game = GameService.Instance.GetCurrentGameForTable(table.Id, TableModel.PokerGameType.Blackjack);
					if (game != null && game.Id == model.GameId)
					{
						var player = game.Players.SingleOrDefault(p => p.Id == CurrentUser.Id);
						if (player != null)
						{
							var currentHand = player.Hands.HandList.FirstOrDefault(h => !h.Done);
							if (currentHand != null)
							{
								//currentHand.Insert(0, "^done^");
								currentHand.Done = true;

								GameService.Instance.UpdatePlayerCards(CurrentUser.Id, model.GameId, player.Hands.HandList);
							}
						}
					}
				}
			}
		}

		[HttpPost]
		public void HitHand([FromBody] TableActionsModel model)
		{
			var table = TableService.Instance.GetTableByIdForTableType(TableModel.PokerGameType.Blackjack, model.TableId);
			if (table != null)
			{
				if (GameService.Instance.CheckIsPlayersTurn(model.GameId, table.Id, TableModel.PokerGameType.Blackjack, CurrentUser.Id))
				{
					var game = GameService.Instance.GetCurrentGameForTable(table.Id, TableModel.PokerGameType.Blackjack);
					if (game != null && game.Id == model.GameId)
					{
						var player = game.Players.SingleOrDefault(p => p.Id == CurrentUser.Id);
						if (player != null)
						{
							var currentHand = player.Hands.HandList.FirstOrDefault(h => !h.Done);
							if (currentHand != null)
							{
								var result = TableDeckManager.Instance.DealSingleCard(table.Deck);
								currentHand.Cards.Add(result.CardContainer[0]);
								table.Deck = result.NewDeck;

								TableService.Instance.UpdateTableCardDeck(table.Id, table.JsonDeck);
								if (GetValueOfCards(currentHand.Cards).HandValue >= 21)
								{
									//currentHand.Insert(0, "^done^");
									currentHand.Done = true;
								}
								GameService.Instance.UpdatePlayerCards(CurrentUser.Id, model.GameId, player.Hands.HandList);
							}
						}
					}
				}
			}
		}

		[HttpPost]
		public void LeaveTable([FromBody] TableActionsModel model)
		{
			//var table = TableService.Instance.GetTableByIdForTableType(TableModel.PokerGameType.Blackjack, model.TableId);
			//if (table != null)
			//{
			//	if (GameService.Instance.CheckIsPlayersTurn(model.GameId, table.Id, TableModel.PokerGameType.Blackjack, CurrentUser.Id))
			//	{
			//		var game = GameService.Instance.GetCurrentGameForTable(table.Id, TableModel.PokerGameType.Blackjack);
			//		if (game != null && game.Id == model.GameId)
			//		{
			//			var player = game.Players.SingleOrDefault(p => p.Id == CurrentUser.Id);
			//			if (player != null)
			//			{
			//				var currentHand = player.Hands.HandList.FirstOrDefault(h => !h.Done);
			//				if (currentHand != null)
			//				{
			//					currentHand.Done = true;
			//					GameService.Instance.UpdatePlayerCards(CurrentUser.Id, model.GameId, player.Hands.HandList);
			//				}
			//			}
			//		}
			//	}
			//}
			var table = TableService.Instance.GetTableByIdForTableType(TableModel.PokerGameType.Blackjack, model.TableId);
			if (table != null)
			{
				TableService.Instance.RemovePlayerFromTable(CurrentUser.Id, table.Id);
			}
		}

		private void DetermineCurrentStage(BlackjackGameModel model, GameModel game, TableModel table)
		{
			if (GameService.Instance.GetIsPlayerInGame(CurrentUser.Id, game.Id))
			{
				if (game.GameInfo.GameState == (int)BlackjackCurrentState.Betting)
				{
					// If the current player has not bet
					if (game.Players.Any(p => p.BetValue == 0 && p.Id == CurrentUser.Id))
					{
						model.CurrentStage = BlackjackGameStage.InitialBet;
					}
					else if (game.Players.Any(p => p.BetValue == 0 && p.Id != CurrentUser.Id))
					{
						model.CurrentStage = BlackjackGameStage.WaitingForOtherBets;
					}

					// If all players are done betting
					if (game.Players.All(p => p.BetValue != 0))
					{
						GameService.Instance.SetGameStage(game.Id, (int)BlackjackCurrentState.Dealing);
						game.GameInfo.GameState = (int)BlackjackCurrentState.Dealing;
					}
				}
				if (game.GameInfo.GameState == (int)BlackjackCurrentState.Dealing)
				{
					if (game.Players.All(p => p.Hands == null || !p.Hands.HandList.Any() ||
						p.Hands.HandList.First().Cards.Count == 0))
					{
						model.CurrentStage = BlackjackGameStage.DealCards;

						// Deal the cards
						if (table.Deck == null || table.Deck.Cards.Count <= 40)
						{
							table.Deck = TableDeckManager.Instance.CreateDeck(8);
						}

						var dealtCardsResult = TableDeckManager.Instance.DealCardsToPlayers(game.Players.Count + 1, 2,
							table.Deck);

						table.Deck = dealtCardsResult.NewDeck;
						game.HouseCards =
							dealtCardsResult.CardContainer.Last()
								.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
								.ToList();
						dealtCardsResult.CardContainer.RemoveAt(dealtCardsResult.CardContainer.Count - 1);

						for (int i = 0; i < game.Players.Count; i++)
						{
							var currentPlayer = game.Players.ElementAt(i);
							var handCollection = new List<HandModel>();
							var handModel = new HandModel();

							var cards = dealtCardsResult.CardContainer.ElementAt(i)
									.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
									.ToList();
							handModel.Cards = cards;
							if (GetValueOfCards(cards).HandValue == 21)
								handModel.Done = true;
							handCollection.Add(handModel);

							currentPlayer.Hands = new HandCollection { HandList = handCollection };
						}

						// update database with new cards
						TableService.Instance.UpdateTableCardDeck(table.Id, table.JsonDeck);
						GameService.Instance.UpdateHouseCards(game.Id, game.HouseCards);

						foreach (var player in game.Players)
						{
							GameService.Instance.UpdatePlayerCards(player.Id, game.Id, player.Hands.HandList);
						}

						GameService.Instance.SetGameStage(game.Id, (int)BlackjackCurrentState.RoundTwo);
						game.GameInfo.GameState = (int)BlackjackCurrentState.RoundTwo;
					}
				}
				if (game.GameInfo.GameState == (int)BlackjackCurrentState.RoundTwo)
				{
					if (game.Players.Any(p => p.Hands.HandList.Any(h => h.Cards.Any() && !h.Folded && !h.Done)))
					{
						// Deal secondary cards to next player in line
						var firstPlayer = game.Players.FirstOrDefault(p => p.Hands.HandList.Any(h => h.Cards.Any() && !h.Folded && !h.Done));

						if (firstPlayer != null)
						{
							if (firstPlayer.Id == CurrentUser.Id)
							{
								model.CurrentStage = BlackjackGameStage.MyTurn;
								if (firstPlayer.Hands.HandList.Count == 1)
								{
									model.CurrentPlayerCanSplitHand = CanPlayerSplitHand(firstPlayer.Hands.HandList.First().Cards);
								}
							}
							else
							{
								model.CurrentStage = BlackjackGameStage.WaitingForOtherPlayerTurns;
							}
						}
					}
					else if (game.HouseCards.Any() && game.HouseCards.ElementAt(0) != "^done^")
					{
						model.CurrentStage = BlackjackGameStage.DealersTurn;

						int dealerHandValue = GetValueOfCards(game.HouseCards).HandValue;
						// Deal secondary cards to dealer if dealer's hand value is less than 17
						if (dealerHandValue < 17)
						{
							// if any player hands are greater than the dealer's hand and less than 21, then deal cards to the dealer
							if (
								game.Players.Any(
									p =>
										p.Hands.HandList.Any(
											h => GetValueOfCards(h.Cards).HandValue > dealerHandValue &&
												GetValueOfCards(h.Cards).HandValue <= 21)))
							{
								while (GetValueOfCards(game.HouseCards).HandValue < 17)
								{
									var result = TableDeckManager.Instance.DealSingleCard(table.Deck);
									game.HouseCards.Add(result.CardContainer[0]);
									table.Deck = result.NewDeck;
								}
							}

							game.HouseCards.Insert(0, "^done^");

							// update database with dealer cards
							TableService.Instance.UpdateTableCardDeck(table.Id, table.JsonDeck);
							GameService.Instance.UpdateHouseCards(game.Id, game.HouseCards);
						}

						GameService.Instance.SetGameStage(game.Id, (int)BlackjackCurrentState.Finished);
						game.GameInfo.GameState = (int)BlackjackCurrentState.Finished;
					}
				}
				if (game.GameInfo.GameState == (int)BlackjackCurrentState.Finished)
				{
					model.CurrentStage = BlackjackGameStage.Finished;

					int houseValue = GetValueOfCards(game.HouseCards).HandValue;

					foreach (var player in game.Players)
					{
						int totalToAdd = 0;
						// update each account with appropriate cash
						foreach (var hand in player.Hands.HandList)
						{
							if (!hand.Folded)
							{
								// if hand did not bust and is better than dealer's hand (dealer did not bust) 
								// or dealer busted and hand did not bust
								if (houseValue > 21 && GetValueOfCards(hand.Cards).HandValue <= 21)
								{
									if (player.Hands.HandList.Count == 2)
										totalToAdd += (player.BetValue);
									else
										totalToAdd += (2 * player.BetValue);
								}
								else if (GetValueOfCards(hand.Cards).HandValue > houseValue && GetValueOfCards(hand.Cards).HandValue <= 21)
								{
									if (player.Hands.HandList.Count == 2)
										totalToAdd += (player.BetValue);
									else
										totalToAdd += (2 * player.BetValue);
								}
								else if (GetValueOfCards(hand.Cards).HandValue == houseValue)
								{
									if (player.Hands.HandList.Count == 2)
										totalToAdd += (player.BetValue / 2);
									else
										totalToAdd += (player.BetValue);
								}
							}
						}

						var success = GameService.Instance.UpdatePlayerWinnings(player.Id, game.Id, totalToAdd);
						if (success)
						{
							UserService.Instance.UpdateUserCashValue(player.Id, totalToAdd);
							player.CashValue += totalToAdd;
						}
					}
				}
			}
			else
			{
				model.CurrentStage = BlackjackGameStage.Observing;
			}
		}

		private HandInfo GetValueOfCards(List<string> cards)
		{
			var handInfo = new HandInfo();
			int value = 0;
			int altValue = 0;

			foreach (var card in cards)
			{
				if (card.ElementAt(2) == 'a') { value += 11; altValue += 1; }
				else if (card.ElementAt(2) == '2') { value += 2; altValue += 2; }
				else if (card.ElementAt(2) == '3') { value += 3; altValue += 3; }
				else if (card.ElementAt(2) == '4') { value += 4; altValue += 4; }
				else if (card.ElementAt(2) == '5') { value += 5; altValue += 5; }
				else if (card.ElementAt(2) == '6') { value += 6; altValue += 6; }
				else if (card.ElementAt(2) == '7') { value += 7; altValue += 7; }
				else if (card.ElementAt(2) == '8') { value += 8; altValue += 8; }
				else if (card.ElementAt(2) == '9') { value += 9; altValue += 9; }
				else if (card.ElementAt(2) == 't') { value += 10; altValue += 10; }
				else if (card.ElementAt(2) == 'j') { value += 10; altValue += 10; }
				else if (card.ElementAt(2) == 'q') { value += 10; altValue += 10; }
				else if (card.ElementAt(2) == 'k') { value += 10; altValue += 10; }
			}

			if (value == altValue)
				handInfo.HandValue = value;
			else if (value > altValue && value <= 21)
				handInfo.HandValue = value;
			else
				handInfo.HandValue = altValue;

			if (handInfo.HandValue == 21)
				handInfo.HandOutcome = "Blackjack";
			else if (handInfo.HandValue > 21)
				handInfo.HandOutcome = "Bust";
			else
				handInfo.HandOutcome = "";

			return handInfo;
		}

		private bool CanPlayerSplitHand(List<string> cards)
		{
			if (cards.Count == 2)
			{
				var firstCard = cards.ElementAt(0);
				var secondCard = cards.ElementAt(1);

				if (firstCard.ElementAt(2) == secondCard.ElementAt(2))
					return true;
			}
			return false;
		}
	}
}