using System.Data.Entity;
using NProject.Models.Domain;

namespace NProject.Models.Infrastructure
{
    internal class NProjectDatabaseInitializer : DropCreateDatabaseAlways<NProjectEntities>
    {
        protected override void Seed(NProjectEntities context)
        {
            this.SetupDatabase(context);
        }
        protected void SetupDatabase(NProjectEntities context)
        {
            context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX IXU_Email ON Users (Email)");
        }
    }
}
