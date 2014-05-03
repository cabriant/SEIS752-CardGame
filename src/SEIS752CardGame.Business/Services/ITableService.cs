using System.Collections.Generic;
using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Business.Services
{
	public interface ITableService
	{
		List<TableModel> GetTablesForTableType(TableModel.PokerGameType tableType);
		TableModel GetTableByIdForTableType(TableModel.PokerGameType tableType, string tableId);
		TableModel GetCurrentTableForUser(string userId);
		bool CreateTable(TableModel table);
		bool CheckUserIsAtTable(string userId, string tableId);
		bool AddUserToTable(string userId, string tableId);
		bool RemovePlayerFromTable(string userId, string tableId);
		bool UpdateTableCardDeck(string tableId, string newDeck);
	}
}