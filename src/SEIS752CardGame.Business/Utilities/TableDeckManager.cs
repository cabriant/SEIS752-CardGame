using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEIS752CardGame.Business.Models;
using SEIS752CardGame.Business.Services;

namespace SEIS752CardGame.Business.Utilities
{
	public class TableDeckManager : BaseService<TableDeckManager, ITableDeckManager>, ITableDeckManager
	{
		private static IEnumerable<string> SingleDeck
		{
			get
			{
				return new List<string>() { "c_ac", "c_2c", "c_3c", "c_4c", "c_5c", "c_6c", "c_7c", "c_8c", "c_9c", "c_tc", "c_jc", "c_qc", "c_kc",
                                            "c_ad", "c_2d", "c_3d", "c_4d", "c_5d", "c_6d", "c_7d", "c_8d", "c_9d", "c_td", "c_jd", "c_qd", "c_kd",
                                            "c_ah", "c_2h", "c_3h", "c_4h", "c_5h", "c_6h", "c_7h", "c_8h", "c_9h", "c_th", "c_jh", "c_qh", "c_kh",
                                            "c_as", "c_2s", "c_3s", "c_4s", "c_5s", "c_6s", "c_7s", "c_8s", "c_9s", "c_ts", "c_js", "c_qs", "c_ks"
                                        };
			}
		}

		public DeckModel CreateDeck(int numOfDecks)
		{
			var deck = new List<string>();

			for (int i = 0; i < numOfDecks; i++)
			{
				deck.AddRange(SingleDeck);
			}

			var totalCards = 52 * numOfDecks;
			var shuffledDeck = ShuffleCards(deck, totalCards);
			for (int i = 0; i < 3; i++)
			{
				shuffledDeck = ShuffleCards(shuffledDeck, totalCards);
			}

			return new DeckModel {Cards = shuffledDeck};
		}

		public DealResult DealCardsToPlayers(int numOfPlayers, int cardsToDeal, DeckModel deck)
		{
			var cards = new string[numOfPlayers];

			for (int cNum = 0; cNum < cardsToDeal; cNum++)
			{
				for (int i = 0; i < numOfPlayers; i++)
				{
					var result = DealSingleCard(deck);
					if (cNum == 0)
						cards[i] = result.CardContainer[0];
					else
						cards[i] += "," + result.CardContainer[0];
					deck = result.NewDeck;
				}
			}

			return new DealResult() { CardContainer = cards.ToList(), NewDeck = deck };
		}

		public DealResult DealSingleCard(DeckModel deck)
		{
			var cards = deck.Cards;
			var card = cards.ElementAt(0);
			cards.RemoveAt(0);
			deck.Cards = cards;

			var result = new DealResult()
			{
				CardContainer = new List<string> { card },
				NewDeck = deck
			};

			return result;
		}

		public DealResult DealMultipleCards(DeckModel deck, int number)
		{
			var cardContainer = new List<string>();
			var newDeck = deck;

			for (var i = 0; i < number; i++)
			{
				var temp = DealSingleCard(newDeck);
				cardContainer.Add(temp.CardContainer[0]);
				newDeck = temp.NewDeck;
			}

			var result = new DealResult
			{
				CardContainer = cardContainer,
				NewDeck = newDeck
			};

			return result;
		}

		private List<string> ShuffleCards(List<string> cards, int totalCards)
		{
			var shuffledCards = new List<string>();
			var random = new Random();

			while (cards.Count > 0)
			{
				var randomCard = (random.Next(0, totalCards - 1) % cards.Count);
				shuffledCards.Add(cards.ElementAt(randomCard));
				cards.RemoveAt(randomCard);
			}

			return shuffledCards;
		}
	}
}
