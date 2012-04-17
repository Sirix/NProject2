using System;
using System.Web.Mvc;
using NProject.BLL;
using NProject.Web.Helpers;
using NProject.Web.ViewModels.Meeting;

namespace NProject.Web.Controllers
{
    [HandleError]
    [Authorize]
    public class MeetingController : Controller
    {
        private ProjectService ps = new ProjectService();


        public ActionResult Create(int id)
        {    
            var team = ps.GetProjectTeam(id);

            var meeting = new Models.Domain.Meeting();
            meeting.EventDate = meeting.EventDate.AddHours(SessionStorage.User.HoursOffsetFromUtc);
            meeting.ProjectId = id;
            

            return View(new Create {ProjectId = id, Team = team, Meeting = meeting});
        }

        [HttpPost]
        public ActionResult Create(Create model)
        {
            ps.AddMeeting(model.Meeting, model.TeamParticipants, model.OtherParticipants);
            
            return RedirectToAction("Show", "Project", new {model.Meeting.ProjectId});
        }
    }
}
