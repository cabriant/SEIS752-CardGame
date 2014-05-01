using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;

namespace SEIS752CardGame.Utilities
{
    public class SessionDataPersistor
    {
		public enum SessionKey
		{
			UserKey
		}

        private static SessionDataPersistor _instance;

        // Singleton
        private SessionDataPersistor() { }

        public static SessionDataPersistor Instance
        {
            get { return _instance ?? (_instance = new SessionDataPersistor()); }
        }

        public void StoreInSession<T>(SessionKey key, T value) where T : class
        {
            HttpContext.Current.Session[key.ToString()] = value;
        }

        public T GetFromSession<T>(SessionKey key) where T : class
        {
            return HttpContext.Current.Session[key.ToString()] as T;
        }

        public void PurgeSession()
        {
            HttpContext.Current.Session.Abandon();
        }
    }
}