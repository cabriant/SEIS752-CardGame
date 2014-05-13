using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEIS752CardGame.Models.Blackjack
{
	public class InitTableResponse : BaseApiResponse
	{
		public BlackjackGameModel Game { get; set; }
	}
}