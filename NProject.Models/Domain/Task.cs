using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NProject.Models.Domain
{
    public enum TaskStatus
    {
        Created = 1,
        Assigned = 2,
        Executing = 3,
        Completed = 4,
        Stopped = 5,
        Deleted = 6
    }

    public enum CostType
    {
        Free = 1,
        Fixed = 2,
        PerTime = 3
    }

    public partial class Task
    {
        public Task()
        {
            TaskComments = new HashSet<TaskComment>();
            CreationDate = DateTime.UtcNow;
            Status = TaskStatus.Created;
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public System.DateTime CreationDate { get; set; }
        public System.DateTime? BeginWorkDate { get; set; }
        public System.DateTime? EndWorkDate { get; set; }
        public int? SpentHours { get; set; }
        public int? EstimatedHours { get; set; }

        [DisplayFormat(NullDisplayText = "")]
        public decimal? CostValue { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public int? ResponsibleId { get; set; }
        public virtual User Responsible { get; set; }

        public virtual ICollection<TaskComment> TaskComments { get; set; }

        #region Status

        [Column("Status")]
        [Obsolete("Is used only by EF. Use Status instead of")]
        public int StatusValue { get; set; }

        [NotMapped]
        public TaskStatus Status
        {
#pragma warning disable 612,618
            get { return (TaskStatus) StatusValue; }
            set { StatusValue = (int) value; }
#pragma warning restore 612,618
        }

        #endregion

        #region CostType

        [Column("CostType")]
        [Obsolete("Is used only by EF. Use CostType instead of")]
        public byte CostTypeValue { get; set; }

        [NotMapped]
        public CostType CostType
        {
#pragma warning disable 612,618
            get { return (CostType) CostTypeValue; }
            set { CostTypeValue = (byte) value; }
#pragma warning restore 612,618
        }

        #endregion
    }

}
