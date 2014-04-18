using System.Web.Mvc;
using SEIS752CardGame.Utilities;

namespace SEIS752CardGame.Controllers
{
    //[AuthenticatedWebRequest]
    public class MainController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}