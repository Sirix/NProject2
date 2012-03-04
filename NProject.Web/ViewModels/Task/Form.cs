using System.Collections.Generic;
using System.Web.Mvc;

namespace NProject.Web.ViewModels.Task
{
    public class Form
    {
        public NProject.Models.Domain.Task Task { get; set; }

        public IEnumerable<SelectListItem> Statuses { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; }
        public IEnumerable<SelectListItem> CostTypes { get; set; }

        public int ProjectId { get; set; }
    }
}