using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using NProject.Models.Domain;
using NProject.Web.Helpers;

namespace NProject.Web.Controllers
{
    public class MessageController : Controller
    {
        //
        // GET: /Message/

        public ActionResult Index()
        {
            var data = ServiceLocator.Current.GetInstance<INProjectEntities>();
            var messages = from i in data.Invitations.Where(i => i.Invitee.Id == SessionStorage.User.Id).ToList()
                           select
                               string.Format("You have been invited to project \"{0}\" by {1}", i.Project.Name,
                                             i.Sender.Name);

            return View(messages.ToList());
        }

    }
}
