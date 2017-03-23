using System.Web.Mvc;
using PerformAspNetMcv.Tips;

namespace PerformAspNetMcv.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WithParameters(string id)
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    public class IndexFastController : HomeController, IRouteUrl
    {
        public ActionResult Invoke()
        {
            return Index();
        }
    }
}