using System.Web.Http;
using SEIS752CardGame.Models.Login;
using SEIS752CardGame.Utilities;

namespace SEIS752CardGame.Controllers
{
    public class UserServiceController : BaseApiController
    {
        [HttpPost]
        public AuthResponse Login([FromBody]LoginModel model)
        {
            var response = new AuthResponse() { Authenticated = true };

            SessionDataPersistor.Instance.StoreInSession("isAuth", "Y");

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
            var authVal = SessionDataPersistor.Instance.GetFromSession<string>("isAuth");

            var response = new AuthResponse() {Authenticated = ("Y".Equals(authVal)) };

            return response;
        }
    }
}