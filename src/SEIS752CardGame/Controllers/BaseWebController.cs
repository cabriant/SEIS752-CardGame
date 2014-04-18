using SEIS752CardGame.Utilities;
using System.Web.Mvc;

namespace SEIS752CardGame.Controllers
{
    public class BaseWebController : Controller
    {
        protected bool IsUserAuthenticated()
        {
            var authVal = SessionDataPersistor.Instance.GetFromSession<string>("isAuth");
            return ("Y".Equals(authVal));
        }
    }
}