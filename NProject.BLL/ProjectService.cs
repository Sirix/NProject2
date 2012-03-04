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
    public class ProjectService:BaseService
    {
        public IEnumerable<Project> GetProjectListForUserByRole(int userId)
        {
            IEnumerable<Project> projects = Enumerable.Empty<Project>();

            var user = Database.Users.Single(i => i.Id == userId);
            return projects = Database.Projects.ToList();
            ////;
            ////switch (user.Role)
            ////{
            ////    case UserRole.TopManager:
            ////        projects = Database.Projects.ToList();
            ////        break;

            ////    case UserRole.Customer:
            ////        projects = Database.Projects.Where(p => p.Customer.Id == user.Id).ToList();
            ////        break;

            ////    //case "PM":

            ////    //    projects = Database.Projects.ToList().Where(p => p.Team.Contains(user)).ToList();
            ////    //    break;

            ////    case UserRole.Programmer:
            ////    case UserRole.Manager:
            ////        projects = user.Projects.ToList();
            ////        break;
            ////}

            ////return projects;
        }

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
            var project = new Project()
                              {
                                  Name = projectName,
                                  Description = projectDescription,
                                  ProjectManager = Database.Users.First(u => u.Id == pmId)
                              };
            Database.Workspaces.First(w => w.Id == workspaceId).Projects.Add(project);
            project.Team.Add(new TeamMate {Project = project, UserId = pmId, AccessLevel = AccessLevel.Full});

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

            return project.ProjectManager.Id == userId ||
                   project.Workspace.Owner.Id == userId ||
                   project.Team.Any(tm => tm.Project.Id == projectId && tm.User.Id == userId);
        }

        public void UpdateTeamMate(TeamMate tm)
        {
            Database.ObjectContext.ApplyCurrentValues("TeamMates", tm);
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
            if (!usersInfo.ContainsKey(proj.ProjectManager.Id))
                usersInfo.Add(proj.ProjectManager.Id, proj.ProjectManager.Name);

            usersInfo = usersInfo.Except(curProjectTeam).ToDictionary(t => t.Key, t => t.Value);

            return usersInfo;
        }
    }
}
