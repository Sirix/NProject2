using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NProject.Web.ViewModels.Project
{
    public class Report : ViewModelBase
    {
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<NProject.Models.Domain.Task> Tasks { get; set; }
    }
}