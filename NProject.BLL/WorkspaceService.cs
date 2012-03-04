using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NProject.Models;
using NProject.Models.Domain;

namespace NProject.BLL
{
    public class WorkspaceService : BaseService
    {
        public Workspace GetWorkspace(int id)
        {
            return Database.Workspaces.SingleOrDefault(w => w.Id == id);
        }
        public Workspace Create(string name, User owner)
        {
            var ws = new Workspace {Name = name, Owner = owner};
            return Database.Workspaces.Add(ws);
        }

        public IEnumerable<Project> GetProjectsInWorkspace(int workspaceId)
        {
            return Database.Workspaces.FirstOrDefault(i => i.Id == workspaceId).Projects.ToList();
        }

        public int GetDefaultWorkspaceForUser(int userId)
        {
            var us = new UserService();
            //TODO: Add support for default and non-default
            return us.GetUser(userId).OwnedWorkspaces.FirstOrDefault().Id;
        }

        public IEnumerable<Workspace> GetWorkspacesForUser(int userId)
        {
            return Database.Workspaces.Where(w => w.Owner.Id == userId);
        }

        public void AddWorkspace(Workspace workspace)
        {
            Database.Workspaces.Add(workspace);
            Database.SaveChanges();
        }

        public Dictionary<int, string> GetUsersInWorkspaceProjects(int workspaceId)
        {
            var i = from p in Database.Workspaces.First(w => w.Id == workspaceId).Projects
                    from t in p.Team
                    select new
                               {
                                   Name = t.User.Name,
                                   Id = t.UserId
                               };

            return i.ToDictionary(e => e.Id, e => e.Name);
        }

        public bool IsUserCanInteractWithWorkspace(int userId, int workspaceId)
        {
            return this.GetWorkspace(workspaceId).Owner.Id == userId;
        }
        public bool IsWorkspaceExists(int workspaceId)
        {
            return Database.Workspaces.Any(w => w.Id == workspaceId);
        }
        public void UpdateWorkspace(Workspace ws)
        {
            Database.ObjectContext.ApplyCurrentValues("Workspaces", ws);
            Database.SaveChanges();
        }

        public void DeleteWorkspace(int id)
        {
            Database.ObjectContext.DeleteObject(Database.Workspaces.FirstOrDefault(w => w.Id == id));
            Database.SaveChanges();
        }
    }
}
