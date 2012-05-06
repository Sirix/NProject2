using System;
using System.Linq;
using System.Web.Mvc;
using NProject.BLL;
using NProject.Web.Helpers;
using NProject.Web.ViewModels.Meeting;

namespace NProject.Web.Controllers
{
    [HandleError]
    [Authorize]
    public class MeetingController : BaseController
    {
        private ProjectService ps = new ProjectService();


        public ActionResult List(int id, string show = "future")
        {
            var project = ps.GetProjectById(id);
            List l = new List
                         {
                             Meetings = ps.GetMeetings(id, show),
                             ProjectId = id,
                             ProjectName = project.Name,
                             CanUserEditMeetings = project.Team.Any(
                                 t =>
                                 t.AccessLevel == Models.Domain.AccessLevel.ProjectManager &&
                                 t.UserId == SessionStorage.User.Id),
                             MeetingFilter = show
                         };

            return View(l);
        }

        public ActionResult Create(int id)
        {    
            var team = ps.GetProjectTeam(id);

            var meeting = new Models.Domain.Meeting();
            meeting.EventDate = meeting.EventDate.AddHours(SessionStorage.User.HoursOffsetFromUtc);
            meeting.ProjectId = id;
            

            return View(new Form {ProjectId = id, Team = team, Meeting = meeting});
        }

        [HttpPost]
        public ActionResult Create(Form model)
        {
            model.Meeting.OrganizerId = SessionStorage.User.Id;
            ps.AddMeeting(model.Meeting, model.TeamParticipants, model.OtherParticipants);
            
            return RedirectToAction("Show", "Project", new {model.Meeting.ProjectId});
        }

        public ActionResult Edit(int id)
        {
            var meeting = ps.GetMeeting(id);
            meeting.EventDate = meeting.EventDate.AddHours(SessionStorage.User.HoursOffsetFromUtc);
     

            var team = ps.GetProjectTeam(meeting.ProjectId);
            var model = new Form
                            {
                                ProjectId = id,
                                Team = team,
                                Meeting = meeting,
                                TeamParticipants = string.Join(" ",
                                                               meeting.Participants.Where(
                                                                   p => meeting.Project.Team.Any(t => t.UserId == p.Id))
                                                                   .Select(
                                                                       p => p.Id)),
                                OtherParticipants =
                                    string.Join(" ",
                                                meeting.Participants.Where(
                                                    p => !meeting.Project.Team.Any(t => t.UserId == p.Id)).Select(
                                                        p => p.Email))
                            };

            return View("Create", model);
        }
        [HttpPost]
        public ActionResult Edit(Form model)
        {
            //set time to UTC
            model.Meeting.EventDate = model.Meeting.EventDate.AddHours(-SessionStorage.User.HoursOffsetFromUtc);

            ps.UpdateMeeting(model.Meeting, model.TeamParticipants, model.OtherParticipants);

            return RedirectToAction("List", new {id = model.Meeting.ProjectId});
        }

        public ActionResult Show(int id, string accessToken = "")
        {
            var meeting = ps.GetMeeting(id);

            var model = new Show()
                            {Meeting = meeting, ProjectId = meeting.ProjectId, ProjectName = meeting.Project.Name};
            return View(model);
        }

        [HttpPost]
        public ActionResult PostComment(int id, string message)
        {
            ps.PostComment(id, message, SessionStorage.User.Id);

            return RedirectToAction("Show", new {id});
        }

        [HttpPost]
        public ActionResult FinishMeeting(int id)
        {
            var m = ps.FinishMeeting(id);

            return RedirectToAction("List", new {id = m.ProjectId});
        }
    }
}