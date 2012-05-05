using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Microsoft.Practices.Unity;
using NProject.BLL;
using NProject.Models;
using NProject.Models.Domain;
using NProject.Web.Helpers;
using Microsoft.Practices.ServiceLocation;
using NProject.Web.ViewModels.Account;

namespace NProject.Controllers
{
    [HandleError]
    public class AccountController : Controller
    {
        public UserService UserService { get; set; }

        public AccountController()
        {
            UserService = new UserService();
        }


        //public IFormsAuthenticationService FormsService { get; set; }

        //[Dependency]
        //public IMembershipService MembershipService { get; set; }

        //[Dependency]
        //public IAccessPoint AccessPoint { get; set; }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

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
            return View();
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = 2;
            return View();
        }

        //[Authorize]
        //[HttpPost]
        //public ActionResult ChangePassword(ChangePasswordModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (UserService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
        //        {
        //            return RedirectToAction("ChangePasswordSuccess");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    ViewData["PasswordLength"] = 2;
        //    return View(model);
        //}

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
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
                SessionStorage.User = new UserSessionInfo()
                                          {
                                              Id = user.Id,
                                              UserName = user.Name,
                                              HoursOffsetFromUtc = user.HoursOffsetFromUtc
                                          };
                
                //create first default company
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
