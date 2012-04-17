using System.Collections.Generic;
using NProject.Models.Domain;
using NProject.Web.ViewModels;

namespace NProject.ViewModels.Generic
{
    public class ProjectListViewModel : ViewModelBase
    {
        public string TableTitle { get; set; }
        public bool UserCanCreateAndDeleteProject { get; set; }
        public bool UserCanManageMeetings { get; set; }
        public bool UserIsCustomer { get; set; }

        public IEnumerable<NProject.Models.Domain.Project> Projects { get; set; }
    }
}