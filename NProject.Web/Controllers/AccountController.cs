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
                                                  HoursOffsetFromUtc = user.HoursOffsetFromUtc,
                                                  AccountType = user.AccountType
                                              };

                    if (!String.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                {
                    this.TempData["ErrorMessage"] = "We can't find such user :(";
                    //ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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
                                              HoursOffsetFromUtc = user.HoursOffsetFromUtc,
                                              AccountType = user.AccountType
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

        public ActionResult ForgotPassword(string email = "", string token = "")
        {
            var vm = new RestorePassword {Email = email};

            if (email == "") return View(vm);

            //try
            //{
            vm.IsValidToken = UserService.IsPasswordRestoreTokenValid(email, token);
            //}
            //catch (ServiceException ex)
            //{
            //    this.SetTempMessage(ex.Message, "Error");
            //}

            //show form for update password here or to get token
            return View(vm);
        }

        [HttpPost]
        public ActionResult ForgotPassword(RestorePassword model)
        {
            try
            {
                //have only email
                if (string.IsNullOrEmpty(model.Token))
                {
                    string token = UserService.CreateRestoreToken(model.Email);

                    MessageService.SendEmail<string>(model.Email, "Change password", "PasswordRestoreRequest",
                                                     new EmailDTO<string>
                                                         {Model = token, User = UserService.GetUser(model.Email)});
                    this.SetTempMessage("Please check your email for next instructions", "success");

                    return RedirectToAction("Index", "Home");
                }

                model.IsValidToken = UserService.IsPasswordRestoreTokenValid(model.Email, model.Token);
                if (!model.IsValidToken)
                {
                    this.SetTempMessage("Invalid token", "error");
                    return RedirectToAction("Index", "Home");
                }

                //update password here
                UserService.UpdateUserPassword(model.Email, model.NewPassword);
                this.SetTempMessage("You have changed your password", "success");
                return RedirectToAction("Index", "Home");
            }

            catch (ServiceException ex)
            {
                this.SetTempMessage(ex.Message, ex.Level);
            }
            return View(model);
        }
    }
}
