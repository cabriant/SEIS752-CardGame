using SEIS752CardGame.Business.Models;
using SEIS752CardGame.Utilities;
using System.Web.Mvc;

namespace SEIS752CardGame.Controllers
{
    public class BaseWebController : Controller
    {
        protected bool IsUserAuthenticated()
        {
            var user = SessionDataPersistor.Instance.GetFromSession<UserModel>(SessionDataPersistor.SessionKey.UserKey);
            return (user != null);
        }
    }
}