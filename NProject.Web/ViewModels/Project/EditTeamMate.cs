using System;
using System.Collections.Generic;
using NProject.BLL;
using NProject.Models.Domain;
using NProject.Web.ViewModels.Project;

namespace NProject.ViewModels.Project
{
    public class EditTeamMate
    {
        public TeamList.TeamMate TeamMate { get; set; }
        public Dictionary<string, Tuple<int, string>> Roles { get; set; }

        public EditTeamMate()
        {
            
        }
        public EditTeamMate(string userName, int teamMateId, int role)
        {
          //  TeamMate = new TeamList.TeamMate() {Id = teamMateId, Role = role, Username = userName};
        }
    }
}