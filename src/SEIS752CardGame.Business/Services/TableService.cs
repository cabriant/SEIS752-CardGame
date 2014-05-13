using System;
using System.Collections.Generic;
using System.Linq;
using SEIS752CardGame.Business.Models;
using SEIS752CardGame.Business.Utilities;

namespace SEIS752CardGame.Business.Services
{
    public class TableService : BaseService<TableService, ITableService>, ITableService
    {
		public List<TableModel> GetTablesForTableType(TableModel.PokerGameType tableType)
		{
			var context = Database.GetContext();
			var tableList = (from t in context.poker_table
							 where t.table_game_type == (int)tableType
							 select t).ToList();

			var tables = new List<TableModel>();
			if (!tableList.Any()) 
				return tables;

			tables.AddRange(tableList.Select(table => new TableModel(table)));

			return tables;
		}

		public TableModel GetTableByIdForTableType(TableModel.PokerGameType tableType, string tableId)
		{
			var context = Database.GetContext();
			var table = (from t in context.poker_table
						 where t.table_game_type == (int)tableType
						 && t.table_id == tableId
						 select t).SingleOrDefault();

			return (table != null ? new TableModel(table) : null);
		}

		public TableModel GetCurrentTableForUser(string userId)
		{
			var context = Database.GetContext();
			var user = (from u in context.users
						  where u.user_id == userId
						  select u).SingleOrDefault();

			if (user == null || user.poker_table.SingleOrDefault() == null)
				return null;
			
			return new TableModel(user.poker_table.Single());
		}

		public bool CreateTable(TableModel table)
		{
			var newTable = new poker_table
			{
				table_id = Guid.NewGuid().ToString("N"),
				table_game_type = (int) table.GameType,
				table_disp_name = table.DisplayName,
				max_players = table.MaxPlayers
			};

			if (table.Ante.HasValue && table.Ante.Value > 0)
				newTable.ante = table.Ante;
			if (table.MaxRaise.HasValue && table.MaxRaise.Value > 0)
				newTable.max_raise = table.MaxRaise;

			var context = Database.GetContext();
			context.poker_table.Add(newTable);

			var success = false;
			try
			{
				 success = (1 == context.SaveChanges());
			}
			catch (InvalidOperationException e)
			{
				//throw;
			}

			return success;
		}

		public bool CheckUserIsAtTable(string userId, string tableId)
		{
			var context = Database.GetContext();
			var result = (from t in context.poker_table
						where t.table_id == tableId
								&& t.users.Any(u => u.user_id == userId)
						select t).SingleOrDefault();
			
			return (result != null);
		}

		public bool AddUserToTable(string userId, string tableId)
		{
			var context = Database.GetContext();
			var table = (from t in context.poker_table
						  where t.table_id == tableId
						  select t).SingleOrDefault();
			var user = (from u in context.users
						where u.user_id == userId
						select u).SingleOrDefault();

			if (table != null && user != null)
				table.users.Add(user);

			var success = false;
			try
			{
				success = (1 == context.SaveChanges());
			}
			catch (Exception)
			{
				
				throw;
			}
			
			return success;
		}

		public bool RemovePlayerFromTable(string userId, string tableId)
		{
			var context = Database.GetContext();
			var table = (from t in context.poker_table
						 where t.table_id == tableId
						 select t).SingleOrDefault();
			
			var user = (from u in context.users
						where u.user_id == userId
						select u).SingleOrDefault();

			if (table != null && user != null)
				table.users.Remove(user);

			var success = false;
			try
			{
				success = (1 == context.SaveChanges());
			}
			catch (Exception)
			{

				throw;
			}

			return success;
		}

		public bool UpdateTableCardDeck(string tableId, string newDeck)
		{
			var context = Database.GetContext();
			var table = (from t in context.poker_table
						 where t.table_id == tableId
						 select t).SingleOrDefault();

			if (table == null)
				return false;

			table.table_deck = newDeck;
			
			var success = false;
			try
			{
				success = (1 == context.SaveChanges());
			}
			catch (Exception)
			{

				throw;
			}

			return success;
		}
    }
}
