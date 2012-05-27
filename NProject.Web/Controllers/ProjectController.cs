using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web.Mvc;
using NProject.BLL;
using NProject.Models.Domain;
using NProject.Web.Helpers;
using NProject.Web.ViewModels.Project;

namespace NProject.Web.Controllers
{
    [HandleError]
    [Authorize]
    public class ProjectController : BaseController
    {
        public ActionResult Create(int id)
        {
            var ws = new WorkspaceService();
            if (!ws.IsUserCanInteractWithWorkspace(SessionStorage.User.Id, id))
                return RedirectToAction("Index", "Workspace");

            return View(new CreateForm {WorkspaceId = id});
        }

        [HttpPost]
        public ActionResult Create(CreateForm p)
        {
            var ws = new WorkspaceService();
            if (!ws.IsUserCanInteractWithWorkspace(SessionStorage.User.Id, p.WorkspaceId))
                return RedirectToAction("Index", "Workspace");

            if (!ModelState.IsValid) return View(p);

            int projectId = new ProjectService().CreateProject(p.Name, p.WorkspaceId, SessionStorage.User.Id,
                                                               p.Description);

            return RedirectToAction("Show", "Project", new { id = projectId });
        }

        public ActionResult Show(int id)
        {
            var ps = new ProjectService();
            var project = ps.GetProjectById(id);
            if (project != null && ps.IsUserCanSeeProject(SessionStorage.User.Id, id))
                return View(project);

            return RedirectToAction("Index", "Workspace");
        }

        public ActionResult Team(int id)
        {
            var ps = new ProjectService();
            var project = ps.GetProjectById(id);
            if (project == null || !ps.IsUserCanSeeProject(SessionStorage.User.Id, id))
                return RedirectToAction("Index", "Workspace");

            var vm = new TeamList
                         {
                             ProjectId = id,
                             ProjectName = project.Name,
                             Team = from t in project.Team
                                    select
                                        new TeamList.TeamMate
                                            {UserId = t.UserId, Username = t.User.Name, UserLevel = t.AccessLevel},
                             CanChangePM = project.Workspace.Owner.Id == SessionStorage.User.Id,
                             CanEditTeam =
                                 project.Team.Any(
                                     t =>
                                     t.AccessLevel == Models.Domain.AccessLevel.ProjectManager &&
                                     t.UserId == SessionStorage.User.Id)
                         };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeamMate(TeamMate tm, string action)
        {
            var ps = new ProjectService();
            if (!ps.IsUserAllowedToDo(tm.ProjectId, SessionStorage.User.Id, ProjectAction.EditTeam))
                return RedirectToAction("Index", "Home");

            if (action == "update")
            {
                ps.UpdateTeamMate(tm);
                this.SetTempMessage(Resources.Global_ChangesSaved, "success");
            }
            if (action == "remove")
            {
                ps.RemoveTeamMate(tm);
                this.SetTempMessage(Resources.Project_Team_UserRemoved, "success");
            }

            return RedirectToAction("Team", new {id = tm.ProjectId});
        }

        public ActionResult Invite(int id)
        {
            var ps = new ProjectService();
            var usersInfo = ps.GetUsersForInviteToProject(id);


            ViewBag.Id = id;
            return View(usersInfo);
        }

        [HttpPost]
        public ActionResult Invite(int id, string email = "", int[] userIds = null)
        {
            var us = new UserService();
            if (email != "")
            {
                try
                {
                    var i = us.SendInvite(email, SessionStorage.User.Id, id);
                    var html = RenderEmailToString("InviteToProject", i);
                    MessageService.SendEmail(email, "Invitation", html);
                    return new JsonResult {Data = "Message sent!"};
                }
                catch (Exception ex)
                {
                    return new JsonResult {Data = ex.Message};
                }
            }

            if (userIds != null)
            {
                foreach (var userId in userIds)
                {
                    if (userId != SessionStorage.User.Id)
                    {
                        var i = us.SendInvite(userId, SessionStorage.User.Id, id);
                        var html = RenderEmailToString("InviteToProject", i);
                        MessageService.SendEmail(i.Invitee.Email, "Invitation", html);
                    }
                }
                return new JsonResult {Data = "Invitations sent!"};
            }
            return new ContentResult();
        }

        public ActionResult ProcessInvite(int id, string verb)
        {
            string result;
            var us = new UserService();
            var invitation = us.GetInvitation(id);
            if (invitation == null) return RedirectToAction("Index", "Home");

            //invitation sent to already resgistered user
            if (invitation.Invitee != null)
            {
                //current logged user is target user
                if (invitation.Invitee.Id == SessionStorage.User.Id)
                    result = us.ProcessInvitation(invitation, verb, SessionStorage.User.Id);
                    //non-authorized
                else
                    return RedirectToAction("Index", "Home");
            }
            //we look for email in database
            else
            {
                var userId = us.GetUserIdByEmail(invitation.InviteeEmail);
                if (userId == -1)
                {
                    if (verb == "block")
                        result = us.ProcessInvitation(invitation, "block", 0);
                    else
                    {
                        TempData["InformationMessage"] = "You should register before processing";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                    result = us.ProcessInvitation(invitation, verb, userId);
            }
            TempData["InformationMessage"] = result;
            return RedirectToAction("Shared", "Project");
        }


        public ActionResult Shared()
        {
            var projects=new ProjectService().GetSharedProjectsForUser(SessionStorage.User.Id);

            return View(new Shared {Projects = projects});
        }

        public ActionResult Report(int id)
        {
            var ps = new ProjectService();
            if (!ps.IsUserAllowedToDo(id, SessionStorage.User.Id, ProjectAction.ViewReport))
                return RedirectToAction("Index", "Home");

            var project = ps.GetProjectById(id);
            Report model = new Report
                               {
                                   ProjectId = id,
                                   ProjectName = project.Name,
                                   BeginDate = DateTime.UtcNow.AddDays(-DateTime.UtcNow.Day + 1)
                               };
            model.EndDate =
                model.BeginDate.AddDays(DateTime.DaysInMonth(model.BeginDate.Year, model.BeginDate.Month) -
                                        model.BeginDate.Day);
            //we want here 1st dy of the  month and the last day of the month
            model.Tasks = ps.GetCompletedTasksInRange(model.BeginDate, model.EndDate);

            return View(model);
        }
        
    }
}

