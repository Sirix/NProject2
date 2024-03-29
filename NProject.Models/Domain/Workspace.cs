//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NProject.Models.Domain
{
    public partial class Workspace
    {
        public Workspace()
        {
            this.Projects = new HashSet<Project>();
            CreationDate = DateTime.UtcNow;
        }

        [Required]
        [Key]
        public int Id { get; set; }
        public System.DateTime CreationDate { get; set; }

        [Required]
        [Display(Name = "Workspace_Name", ResourceType = typeof (Resources_Models))]
        public string Name { get; set; }
        [Display(Name = "Workspace_Description", ResourceType = typeof(Resources_Models))]
        public string Description { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
        public virtual User Owner { get; set; }
    }
}
