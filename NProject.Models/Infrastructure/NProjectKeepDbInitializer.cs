using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Transactions;
using NProject.Models.Domain;

namespace NProject.Models.Infrastructure
{
	internal class NProjectKeepDbInitializer : IDatabaseInitializer<NProjectEntities>
	{
		public void InitializeDatabase(NProjectEntities context)
		{
			bool flag;
			using (new TransactionScope(TransactionScopeOption.Suppress))
			{
				flag = context.Database.Exists();
			}
			if (flag)
			{
				bool throwIfNoMetadata = false;
			    context.Database.CompatibleWithModel(throwIfNoMetadata);
			}
		}

		private void DeleteExistingTables(DbContext dbContext)
		{
			var dropConstraintsScript =
				@"declare @cmd varchar(4000)
declare cmds cursor for 
			   select  'ALTER TABLE ' + so.TABLE_NAME + ' DROP CONSTRAINT ' + so.constraint_name  from INFORMATION_SCHEMA.TABLE_CONSTRAINTS so order by so.CONSTRAINT_TYPE
open cmds
	while 1=1
	   begin
		  fetch cmds into @cmd   
			  if @@fetch_status != 0 break
							   print @cmd
							   exec(@cmd)
	   end
close cmds
				deallocate cmds";
			string dropTablesScript =
				@"declare @cmd varchar(4000)
declare cmds cursor for 
			   Select 'drop table [' + Table_Name + ']' From INFORMATION_SCHEMA.TABLES
open cmds
	while 1=1
	   begin
		  fetch cmds into @cmd   
			  if @@fetch_status != 0 break
							   print @cmd
							   exec(@cmd)
	   end
close cmds
				deallocate cmds";
			dbContext.Database.ExecuteSqlCommand(dropConstraintsScript);
			dbContext.Database.ExecuteSqlCommand(dropTablesScript);
		}
	}
}
