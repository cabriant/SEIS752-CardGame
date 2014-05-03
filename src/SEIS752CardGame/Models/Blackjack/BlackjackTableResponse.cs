using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Models.Blackjack
{
	public class BlackjackTableResponse : BaseApiResponse
	{
		public List<TableModel> Tables;
	}
}