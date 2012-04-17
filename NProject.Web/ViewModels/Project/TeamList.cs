using System.Collections.Generic;
using NProject.Models.Domain;

namespace NProject.Web.ViewModels.Project
{
    public class TeamList
    {
        public class TeamMate
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public AccessLevel UserLevel { get; set; }
        }

        public string ProjectName { get; set; }
        public int ProjectId { get; set; }

        public IEnumerable<TeamMate> Team { get; set; }

        public User ProjectManager { get; set; }

        public bool CanChangePM { get; set; }
        public bool CanEditTeam { get; set; }
    }
}