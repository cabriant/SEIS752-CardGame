using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;

namespace SEIS752CardGame.Utilities
{
    public class SessionDataPersistor
    {
        private static SessionDataPersistor _instance;

        // Singleton
        private SessionDataPersistor() { }

        public static SessionDataPersistor Instance
        {
            get { return _instance ?? (_instance = new SessionDataPersistor()); }
        }

        public void StoreInSession<T>(string key, T value) where T : class
        {
            HttpContext.Current.Session[key] = value;
        }

        public T GetFromSession<T>(string key) where T : class
        {
            return HttpContext.Current.Session[key] as T;
        }

        public void PurgeSession()
        {
            HttpContext.Current.Session.Abandon();
        }
    }
}