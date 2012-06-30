using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using NProject.BLL;
using NProject.Models.Domain;
using NProject.Web.Helpers;

namespace NProject.Web.Areas.Admin.Controllers
{
    [Administrator]
    public class UserController : Controller
    {
        //
        // GET: /Admin/User/

        public ActionResult Index()
        {
            var us = new UserService();

            return View(us.GetUsers());
        }
    }

    public class AdministratorAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Request.IsAuthenticated && SessionStorage.User.AccountType == UserAccountType.Administrator;
        }
    }
}
