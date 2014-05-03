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
	}
}