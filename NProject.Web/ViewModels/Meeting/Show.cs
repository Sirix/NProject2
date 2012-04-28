using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NProject.Web.ViewModels.Meeting
{
    public class Show : ViewModelBase
    {
        public NProject.Models.Domain.Meeting Meeting { get; set; }

        public bool HasNotYetStarted { get { return (Meeting.EventDate - DateTime.UtcNow).TotalMinutes > 5; } }
        //public bool IsMeetingPassed{get{ return Meeting.EventDate < }}
    }
}