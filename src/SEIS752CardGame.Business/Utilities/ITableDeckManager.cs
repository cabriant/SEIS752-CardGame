using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Business.Utilities
{
	public interface ITableDeckManager
	{
		DeckModel CreateDeck(int numOfDecks);
		DealResult DealCardsToPlayers(int numOfPlayers, int cardsToDeal, DeckModel deck);
		DealResult DealSingleCard(DeckModel deck);
		DealResult DealMultipleCards(DeckModel deck, int number);
	}
}