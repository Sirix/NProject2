using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NProject.Web.ViewModels.Task
{
    public class Show : ViewModelBase
    {
        public NProject.Models.Domain.Task Task { get; set; }
        public bool CanUserAcceptTask { get; set; }
        public bool AssignedForCurrentUser { get; set; }
    }
}