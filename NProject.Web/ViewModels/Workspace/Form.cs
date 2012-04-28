using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NProject.Web.ViewModels.Workspace
{
    public class Form
    {
        public NProject.Models.Domain.Workspace Workspace { get; set; }
        public bool IsEditing { get; set; }
    }
}