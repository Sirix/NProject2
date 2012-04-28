using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using NProject.Models.Infrastructure;

// ReSharper disable CheckNamespace
namespace NProject.Models.Domain
// ReSharper restore CheckNamespace
{
    public interface INProjectEntities : IObjectContextAdapter
    {
    	IDbSet<User> Users { get; set; }
    	IDbSet<Workspace> Workspaces { get; set; }
    	IDbSet<Project> Projects { get; set; }
    	IDbSet<Task> Tasks { get; set; }
    	IDbSet<Meeting> Meetings { get; set; }
    	IDbSet<Invitation> Invitations { get; set; }
    	IDbSet<TaskComment> TaskComments { get; set; }
    	IDbSet<MeetingComment> MeetingComments { get; set; }
    	IDbSet<TeamMate> TeamMates { get; set; }
    
    	int SaveChanges();
    	DbChangeTracker ChangeTracker { get; }
    }
    
    public class NProjectEntities : DbContext, INProjectEntities
    {
        static NProjectEntities()
        {
            Database.SetInitializer(new NProjectTestDatabaseInitializer());
        }
        public NProjectEntities()
            : base("name=NProjectEntities")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasMany<Meeting>(p => p.Meetings).WithRequired(m => m.Project).HasForeignKey(
                m => m.ProjectId);

            //modelBuilder.Entity<Meeting>().HasRequired(m=>m.Project).WithRequiredDependent()  HasMany<Meeting>(p => p.Meetings).WithRequired(m => m.Project).HasForeignKey(
            //    m => m.ProjectId);

            //modelBuilder.Entity<Meeting>().HasRequired<User>(m => m.Organizer).WithRequiredDependent().Map(
            //    fk => fk.MapKey("OriganizerId").ToTable("Users")).WillCascadeOnDelete(false);

            //modelBuilder.Entity<Meeting>().HasMany<MeetingComment>(m => m.MeetingComments).WithRequired(i => i.Meeting).
            //    WillCascadeOnDelete(true);
            modelBuilder.Entity<MeetingComment>().HasRequired<Meeting>(m => m.Meeting).WithMany(m => m.MeetingComments).
                WillCascadeOnDelete(false);

            //modelBuilder.Entity<Meeting>().HasRequired<User>(m => m.Organizer).WithRequiredDependent().
            //    WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany<Meeting>(u => u.OrganizedMeetings).WithRequired(m => m.Organizer).
                HasForeignKey(
                    fk => fk.OrganizerId).WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>().HasMany<Meeting>(u => u.Meetings).WithMany().Map(
            //    t=>t.MapLeftKey())
            //    ithRequired(m => m.Organizer).
            //    HasForeignKey(
            //        fk => fk.OrganizerId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Meeting>().HasRequired<User>(m => m.Organizer).WithMany(u => u.OrganizedMeetings).
                HasForeignKey(m => m.OrganizerId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Meeting>().HasMany<User>(m => m.Participants).WithMany().Map(
                t => t.MapLeftKey("UserId").MapRightKey("MeetingId").ToTable("MeetingParticipants"));

            modelBuilder.Entity<User>().HasMany<Invitation>(u => u.ReceivedInvitations).WithRequired(i => i.Invitee);
        }

        public IDbSet<User> Users { get; set; }
        public IDbSet<Workspace> Workspaces { get; set; }
        public IDbSet<Project> Projects { get; set; }
        public IDbSet<Task> Tasks { get; set; }
        public IDbSet<Meeting> Meetings { get; set; }
        public IDbSet<Invitation> Invitations { get; set; }
        public IDbSet<TaskComment> TaskComments { get; set; }
        public IDbSet<MeetingComment> MeetingComments { get; set; }
        public IDbSet<TeamMate> TeamMates { get; set; }
    }
}
