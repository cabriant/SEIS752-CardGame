using System;
using System.Web;

namespace SEIS752CardGame.Business.Utilities
{
	public static class Database
	{
		private const string SESSION_DB_KEY = "SESSION_DB_KEY";

		/// <summary>
		/// Gets the database context.  Caches the context per HTTP Request.
		/// </summary>
		/// <returns>Database context</returns>
		public static CardGameDbEntities GetContext()
		{
			if (HttpContext.Current != null)
			{

				if (HttpContext.Current.Items[SESSION_DB_KEY] == null)
				{
					HttpContext.Current.Items.Add(SESSION_DB_KEY, new CardGameDbEntities());
				}
				else
				{
					//covers edge case where the ObjectContext was disposed (e.g. by a "using" statement)
					try
					{
						var chkDispose = ((CardGameDbEntities)HttpContext.Current.Items[SESSION_DB_KEY]);
					}
					catch (ObjectDisposedException)
					{
						HttpContext.Current.Items[SESSION_DB_KEY] = new CardGameDbEntities();
					}
				}
				return (CardGameDbEntities)HttpContext.Current.Items[SESSION_DB_KEY];
			}
			return new CardGameDbEntities();
		}
	}
}
