using System;
using System.Web.Mvc;
using System.Web.Security;
using NProject.BLL;
using NProject.Models;
using NProject.Web.Helpers;
using NProject.Web.ViewModels.Account;

namespace NProject.Web.Controllers
{
    [HandleError]
    public class AccountController : BaseController
    {
        public UserService UserService { get; set; }

        public AccountController()
        {
            UserService = new UserService();
        }

        public ActionResult LogOn()
        {
            if(Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View(new LogOnModel {Email = "manager", Password = "manager"});
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = UserService.IsUserExists(model.Email, model.Password);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                    SessionStorage.User = new UserSessionInfo
                                              {
                                                  Id = user.Id,
                                                  UserName = user.Name,
                                                  HoursOffsetFromUtc = user.HoursOffsetFromUtc
                                              };

                    if (!String.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);

                }
                else
                {
                    this.TempData["ErrorMessage"] = "We can't find such user :(";
                    //ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult SimpleRegistration(string email)
        {
            //TODO: send an e-mail

            return View(new SimpleRegistration {Email = email});
        }

        [HttpPost]
        public ActionResult SimpleRegistrationFinish(SimpleRegistration model)
        {
            if (!ModelState.IsValid)
                return View("SimpleRegistration", model);

            try
            {
                var user = UserService.CreateUser(model.Name, model.LastName, model.Email, model.Password,
                                                  model.TimeShiftFromUtc);

                FormsAuthentication.SetAuthCookie(model.Email, true);
                //store user data
                SessionStorage.User = new UserSessionInfo
                                          {
                                              Id = user.Id,
                                              UserName = user.Name,
                                              HoursOffsetFromUtc = user.HoursOffsetFromUtc
                                          };
                
                //create first default workspace
                new WorkspaceService().Create(user.Email + "-workspace", user);

                TempData["SuccessMessage"] = "Your account has been created!";
                return RedirectToAction("Index", "Workspace");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("SimpleRegistration", model);
            }
        }
    }
}
