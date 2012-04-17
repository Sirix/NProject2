using System.Linq;
using System.Collections.Generic;
using NProject.Models.Domain;

namespace NProject.Web.ViewModels.Meeting
{
    public class Create : ViewModelBase
    {
        public NProject.Models.Domain.Meeting Meeting { get; set; }
        public IEnumerable<TeamMate> Team { get; set; }
        public string TeamParticipants { get; set; }
        public string OtherParticipants { get; set; }

        public string TeamDataSource
        {
            get { return string.Format("[{0}]", string.Join(",", Team.Select(t => string.Format("\"{0}\"", t.User.Name)))); }
        }
    }
}