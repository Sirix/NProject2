using System.Web.Mvc;

namespace NProject.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                //TODO: create here logic for default locations
                return RedirectToAction("Index", "Workspace");
            }
            else
                return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}