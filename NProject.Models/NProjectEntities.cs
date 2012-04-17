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
