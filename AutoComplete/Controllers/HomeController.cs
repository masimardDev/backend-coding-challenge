using System.Web.Mvc;

namespace AutoComplete.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Coveo Backend Coding Challenge";

            return View();
        }
    }
}
