using System;
using System.ComponentModel.DataAnnotations;

namespace NProject.Models.Domain
{
    public enum InvitationStatus
    {
        Sent = 1,
        Accepted = 2,
        Declined = 3,
        Blocked = 4
    }

    public class Invitation
    {
        public Invitation()
        {
            InvitationDate = DateTime.UtcNow;
            Status = InvitationStatus.Sent;
        }

        public int Id { get; set; }

        public string InviteeEmail { get; set; }
        public DateTime InvitationDate { get; set; }
        public DateTime? LastSentDate { get; set; }

        //[ForeignKey("Sender")]
        //public int SenderId { get; set; }

        public virtual User Sender { get; set; }

     //   [ForeignKey("Invitee")]
       // public int InviteeId { get; set; }

        public virtual User Invitee { get; set; }

       // [ForeignKey("Project")]
        //public int ProjectId { get; set; }

        public virtual Project Project { get; set; }

        [Column("Status")]
        public byte StatusValue { get; set; }

        [NotMapped]
        public InvitationStatus Status
        {
            get { return (InvitationStatus) StatusValue; }
            set { StatusValue = (byte) value; }
        }
    }
}
