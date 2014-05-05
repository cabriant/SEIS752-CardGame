using System.Collections.Generic;
using SEIS752CardGame.Business.Models;
using SEIS752CardGame.Business.Services;
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

		[HttpGet]
		public BlackjackTableResponse Table(string tableId)
		{
			var table = TableService.Instance.GetTableByIdForTableType(TableModel.PokerGameType.Blackjack, tableId);

			return new BlackjackTableResponse
			{
				Success = (table != null),
				Tables = new List<TableModel> { table }
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
	}
}