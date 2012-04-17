using System.Web.Mvc;
using NProject.BLL;
using NProject.Web.Helpers;
using NProject.Web.ViewModels.Home;

namespace NProject.Web.Controllers
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

        public ActionResult Settings()
        {
            var model = new Settings() {HoursOffset = SessionStorage.User.HoursOffsetFromUtc};
            return View(model);
        }

        [HttpPost]
        public ActionResult Settings(Settings model)
        {
            new UserService().SaveSettings(SessionStorage.User.Id, model.HoursOffset, model.Locale);
            SessionStorage.User.HoursOffsetFromUtc = model.HoursOffset;

            return RedirectToAction("Settings");
        }
    }
}