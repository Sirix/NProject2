using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NProject.Models.Domain;

namespace NProject.Web.ViewModels.Task
{
    public class List : ViewModelBase
    {
        public IEnumerable<NProject.Models.Domain.Task> Tasks { get; set; }
        public bool CanUserEditTasks { get; set; }

        public string TaskFilter { get; set; }
    }
}