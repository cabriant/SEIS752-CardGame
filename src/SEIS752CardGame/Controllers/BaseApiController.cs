using System.Web.Http;
using SEIS752CardGame.Business.Models;
using SEIS752CardGame.Utilities;

namespace SEIS752CardGame.Controllers
{
    public abstract class BaseApiController : ApiController
    {
	    public UserModel CurrentUser
	    {
		    get { return SessionDataPersistor.Instance.GetFromSession<UserModel>(SessionDataPersistor.SessionKey.UserKey); }
	    }
    }
}