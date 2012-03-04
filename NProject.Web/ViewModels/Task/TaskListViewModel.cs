using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NProject.Models.Domain;

namespace NProject.Web.ViewModels.Task
{
    public class ListInProject
    {
        public IEnumerable<NProject.Models.Domain.Task> Tasks { get; set; }

        public string ProjectTitle { get; set; }
    }
}