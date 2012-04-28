using System;
using System.Web;
using System.Web.Mvc;
using NProject.BLL;
using NProject.Web.Helpers;
using NProject.Web.ViewModels.Home;

namespace NProject.Web.Controllers
{
    [HandleError]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                //TODO: create here logic for default locations
                return RedirectToAction("Index", "Workspace");
            }
            else
            {
                if(!string.IsNullOrEmpty(Request["ReturnUrl"]))
                    this.SetTempMessage("Please, sign in to use the site", "error");
                return View();
            }
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Settings()
        {
            var us = new UserService().GetUser(SessionStorage.User.Id);

            var model = new Settings()
                            {
                                HoursOffsetFromUtc = us.HoursOffsetFromUtc,
                                FirstName = us.FirstName,
                                LastName = us.LastName,
                                Email = us.Email,
                                Language = us.Language
                            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Settings(Settings model)
        {
            HttpCookie c = new HttpCookie("NProject_culture", model.Language);
            c.Expires = DateTime.UtcNow.AddYears(1);
            Response.Cookies.Set(c);

            new UserService().UpdateUserProfile(SessionStorage.User.Id, model.FirstName, model.LastName, model.Password, model.HoursOffsetFromUtc, model.Language);

            SessionStorage.User.UserName = new UserService().GetUser(SessionStorage.User.Id).Name;
            SessionStorage.User.HoursOffsetFromUtc = model.HoursOffsetFromUtc;

            SetTempMessage("Settings saved", "success");
            return RedirectToAction("Settings");
        }
    }
}