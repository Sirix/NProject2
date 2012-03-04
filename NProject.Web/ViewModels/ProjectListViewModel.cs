using System.Collections.Generic;
using NProject.Models.Domain;

namespace NProject.ViewModels.Generic
{
    public class ProjectListViewModel : SiteMasterViewModel
    {
        public string TableTitle { get; set; }
        public bool UserCanCreateAndDeleteProject { get; set; }
        public bool UserCanManageMeetings { get; set; }
        public bool UserIsCustomer { get; set; }

        public IEnumerable<NProject.Models.Domain.Project> Projects { get; set; }
    }
}