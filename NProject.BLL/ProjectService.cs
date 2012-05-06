using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NProject.Models;
using NProject.Models.Domain;

namespace NProject.BLL
{
    public class ProjectService : BaseService
    {
        public Project GetProjectById(int id)
        {
            return Database.Projects.SingleOrDefault(p => p.Id == id);
        }

        public IEnumerable<Project> GetProjectsInWorkspace(int workspaceId)
        {
            throw new NotImplementedException();
        }

        public int CreateProject(string projectName, int workspaceId, int pmId, string projectDescription = "")
        {
            var project = new Project
                              {
                                  Name = projectName,
                                  Description = projectDescription,
                              };
            Database.Workspaces.First(w => w.Id == workspaceId).Projects.Add(project);
            project.Team.Add(new TeamMate {Project = project, UserId = pmId, AccessLevel = AccessLevel.ProjectManager});

            Database.SaveChanges();
            return project.Id;
        }

        public IEnumerable<TeamMate> GetProjectTeam(int id)
        {
            return Database.Projects.FirstOrDefault(p => p.Id == id).Team.ToList();
        }

        public TeamMate GetTeamMate(int projectId, int userId)
        {
            return Database.TeamMates.SingleOrDefault(t => t.ProjectId == projectId && t.UserId == userId);
        }

        public bool IsUserCanEditTeamMate(int userId, int teamMateId)
        {
            return true;
        }

        /// <summary>
        /// The following users can see project information - TeamMates(with FullAccess), PM, WS owner
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public bool IsUserCanSeeProject(int userId, int projectId)
        {
            var project = Database.Projects.FirstOrDefault(p => p.Id == projectId);

            //return project.ProjectManager.Id == userId ||
            return
                project.Workspace.Owner.Id == userId ||
                project.Team.Any(tm => tm.Project.Id == projectId && tm.User.Id == userId);
        }

        public void UpdateTeamMate(TeamMate tm)
        {
            Database.ObjectContext.ApplyCurrentValues("TeamMates", tm);
            Database.SaveChanges();
        }

        public void RemoveTeamMate(TeamMate tm)
        {
            //set current tasks(assigned and executing) status of this user to created(remove assigning) and remove responsible user
            Database.Tasks.Where(
                t =>
                t.ProjectId == tm.ProjectId && t.ResponsibleId == tm.UserId &&
                (t.StatusValue == (int) TaskStatus.Assigned || t.StatusValue == (int) TaskStatus.Executing))
                .ToList().ForEach(t =>
                                      {
                                          t.Status = TaskStatus.Created;
                                          t.ResponsibleId = null;
                                      });


            Database.ObjectContext.DeleteObject(
                Database.TeamMates.FirstOrDefault(t => t.UserId == tm.UserId && t.ProjectId == tm.ProjectId));
            Database.SaveChanges();
        }

        /// <summary>
        /// This method suggests list of users which can be invited to project.
        /// This list consists of project pm, others programmers from the same workspace
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetUsersForInviteToProject(int projectId)
        {
            var ps = new ProjectService();
            var proj = ps.GetProjectById(projectId);
            int workspaceId = proj.Workspace.Id;
            var currentProjectTeam = ps.GetProjectTeam(projectId);
            var curProjectTeam = currentProjectTeam.ToDictionary(t => t.User.Id, t => t.User.Name);

            var usersInfo = new WorkspaceService().GetUsersInWorkspaceProjects(workspaceId);

            usersInfo = usersInfo.Except(curProjectTeam).ToDictionary(t => t.Key, t => t.Value);

            return usersInfo;
        }

        public bool IsUserAllowedToDo(int projectId, int userId, ProjectAction action)
        {
            var project = GetProjectById(projectId);
            switch (action)
            {
                case ProjectAction.SeeTaskList:
                    return project.Workspace.Owner.Id == userId ||
                           project.Team.Any(t => t.UserId == userId && t.AccessLevel != AccessLevel.Partial);

                case ProjectAction.CreateOrEditTask:
                case ProjectAction.EditTeam:
                case ProjectAction.ViewReport:
                    return project.Team.Any(t => t.UserId == userId && t.AccessLevel == AccessLevel.ProjectManager);
            }
            return false;
        }



        public List<Project> GetSharedProjectsForUser(int userId)
        {
            return
                Database.Projects.Where(p => p.Workspace.Owner.Id != userId && p.Team.Any(t => t.UserId == userId)).
                    ToList();
        }

        public void AddMeeting(Meeting meeting, string userParticipants, string otherParticipants)
        {
            var ms = new MessageService();
            if (!String.IsNullOrEmpty(userParticipants))
            {
                var users =
                    userParticipants.Trim().Split(' ').Select(int.Parse).Select(
                        id => Database.Users.FirstOrDefault(u => u.Id == id));
                users.ToList().ForEach((user) =>
                                           {
                                               meeting.Participants.Add(user);
                                               ms.SendMeetingInvitation(user.Email, meeting);
                                           });
            }
            if (!string.IsNullOrEmpty(otherParticipants))
            {
                var emails = otherParticipants.Trim().Split(' ').ToList();
                foreach (var email in emails)
                {
                    var trygetUser = Database.Users.FirstOrDefault(u => u.Email == email);
                    if (trygetUser == null)
                    {
                        trygetUser = new User
                                         {
                                             Email = email,
                                             AccountType = UserAccountType.MeetingParticipant,
                                             PasswordHash = MD5.EncryptMD5(email)
                                         };
                        Database.Users.Add(trygetUser);
                        ms.SendMeetingInvitation(trygetUser.Email, meeting, false);
                    }
                    else
                        ms.SendMeetingInvitation(trygetUser.Email, meeting);

                    meeting.Participants.Add(trygetUser);
                }
            }
            Database.Meetings.Add(meeting);
            Database.SaveChanges();
        }

        public IEnumerable<Meeting> GetMeetings(int id, string filter)
        {
            var meetings = Database.Meetings.AsQueryable();
            switch (filter)
            {
                case "past":
                    meetings = meetings.Where(m => m.EventDate <= DateTime.UtcNow);
                    break;
                case "future":
                    meetings = meetings.Where(m => m.EventDate > DateTime.UtcNow);
                    break;
            }
            return meetings;
        }

        public Meeting GetMeeting(int id)
        {
            //Database.ObjectContext.Detach()
            return Database.Meetings.FirstOrDefault(m => m.Id == id);
        }

        public void UpdateMeeting(Meeting meeting, string teamPrticipants, string otherParticipants)
        {
            var met = Database.Meetings.FirstOrDefault(m => m.Id == meeting.Id);
            Database.ObjectContext.ApplyCurrentValues("Meetings", meeting);
            Database.SaveChanges();
            //var users =
            //   teamPrticipants.Trim().Split(' ').Select(int.Parse).Select(
            //       id => Database.Users.FirstOrDefault(u => u.Id == id));

            //var emails = otherParticipants.Trim().Split(' ').ToList();

        }
        public void PostComment(int meetingId, string message, int authorId)
        {
            Database.MeetingComments.Add(new MeetingComment { AuthorId = authorId, Text = message, MeetingId = meetingId });
            Database.SaveChanges();
        }

        public Meeting FinishMeeting(int id)
        {
            var meeting = Database.Meetings.FirstOrDefault(m => m.Id == id);
            meeting.Finished = true;
            Database.SaveChanges();
            return meeting;
        }

        public IEnumerable<Task> GetCompletedTasksInRange(DateTime beginDate, DateTime endDate)
        {
            return
                Database.Tasks.Where(
                    t =>
                    t.StatusValue == (int) TaskStatus.Completed && t.EndWorkDate >= beginDate &&
                    t.EndWorkDate <= endDate);
        }
    }
    public enum ProjectAction
    {
        SeeTaskList,
        CreateOrEditTask,
        EditTeam,
        ViewReport
    }
}
