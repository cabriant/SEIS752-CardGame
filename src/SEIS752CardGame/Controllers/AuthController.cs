using System.Collections.Generic;
using System.Web.Mvc;
using SEIS752CardGame.Models.Login;
using SEIS752CardGame.Utilities;

namespace SEIS752CardGame.Controllers
{
    public class AuthController : BaseWebController
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (IsUserAuthenticated())
                return RedirectToAction("Index", "Main");

            var model = new LoginModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model)
        {
            SessionDataPersistor.Instance.StoreInSession("isAuth", "Y");

            return RedirectToAction("Index", "Main");
        }

        public void OAuth()
        {
            // TODO: Implement OAuth
        }

        [HttpPost]
        public ActionResult Logoff()
        {
            SessionDataPersistor.Instance.PurgeSession();

            return RedirectToAction("Index");
        }
    }
}