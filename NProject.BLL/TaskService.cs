using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using NProject.Models;
using NProject.Models.Domain;

namespace NProject.BLL
{
    public class TaskService : BaseService
    {
        public IEnumerable<Task> GetTasksForProject(int projectId)
        {
            return Database.Tasks.Where(t => t.Project.Id == projectId).ToList();
        }

        public void AddTask(int projectId, Task task)
        {
            Database.Tasks.Add(task);

            Database.SaveChanges();
        }
    }
}
