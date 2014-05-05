using System.Web.Http;
using SEIS752CardGame.Business.Models;
using SEIS752CardGame.Business.Services;
using SEIS752CardGame.Models.Login;
using SEIS752CardGame.Models.User;
using SEIS752CardGame.Utilities;

namespace SEIS752CardGame.Controllers
{
    public class UserServiceController : BaseApiController
    {
        [HttpPost]
        public AuthResponse Login([FromBody]LoginModel model)
        {
	        var user = UserService.Instance.AuthenticateUser(model.Username, model.Password);

	        var response = new AuthResponse {Authenticated = (user != null)};
	        if (user != null)
	        {
				SessionDataPersistor.Instance.StoreInSession(SessionDataPersistor.SessionKey.UserKey, user);
		        response.User = new DisplayUserModel() {DisplayName = user.DisplayName, Id = user.Id};
	        }
	        else
	        {
		        response.Error = "The email or password does not match our records. Please try again.";
	        }
			
            return response;
        }

        [HttpPost]
        [AuthenticatedApiRequest]
        public AuthResponse Logout()
        {
            SessionDataPersistor.Instance.PurgeSession();

            return new AuthResponse() { Authenticated = false };
        }

        [HttpGet]
        public AuthResponse GetUser()
        {
            var user = SessionDataPersistor.Instance.GetFromSession<UserModel>(SessionDataPersistor.SessionKey.UserKey);

			// Session died or they were never authenticated, must re-login
			if (user == null)
				return new AuthResponse { Authenticated = false };

			var response = new AuthResponse()
			{
				Authenticated = true,
				User = new DisplayUserModel { DisplayName = user.DisplayName, Id = user.Id }
			};

            return response;
        }

	    [HttpGet, AuthenticatedApiRequest]
	    public AccessResponse UserAccess()
	    {
		    var user = SessionDataPersistor.Instance.GetFromSession<UserModel>(SessionDataPersistor.SessionKey.UserKey);

			var isAdmin = (user.Type == UserModel.UserType.Admin);
			return new AccessResponse { Success = true, Create = isAdmin, Edit = isAdmin, Delete = isAdmin, Read = true };
	    }
    }
}