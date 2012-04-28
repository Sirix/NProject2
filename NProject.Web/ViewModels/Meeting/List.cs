using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NProject.Web.ViewModels.Meeting
{
    public class List : ViewModelBase
    {
        public IEnumerable<NProject.Models.Domain.Meeting> Meetings { get; set; }

        public bool CanUserEditMeetings { get; set; }

        public string MeetingFilter { get; set; }
    }
}