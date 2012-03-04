using System.ComponentModel.DataAnnotations;

namespace NProject.Web.ViewModels.Project
{
    public class CreateForm
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int WorkspaceId { get; set; }
    }
}