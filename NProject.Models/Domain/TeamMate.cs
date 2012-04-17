using System.ComponentModel.DataAnnotations;

namespace NProject.Models.Domain
{
    public enum AccessLevel
    {
        Partial = 1,
        Full = 2,
        ProjectManager = 3
    }

    public partial class TeamMate
    {
        public TeamMate()
        {
            AccessLevel = AccessLevel.Full;
        }
        [Key, Column(Order = 0)]
        public int UserId { get; set; }

        [Key, Column(Order = 1)]
        public int ProjectId { get; set; }

        public virtual User User { get; set; }

        public virtual Project Project { get; set; }

        [Column("AccessLevel")]
        public byte AccessLevelValue { get; set; }

        [NotMapped]
        public AccessLevel AccessLevel
        {
            get { return (AccessLevel) AccessLevelValue; }
            set { AccessLevelValue = (byte) value; }
        }
    }

}
