﻿using System;
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
        public IEnumerable<Task> GetTasksForProject(int projectId, string show = "all")
        {
            var query = Database.Tasks.Where(t => t.Project.Id == projectId);
            switch (show)
            {
                case "all":
                    query = query.Where(t => t.StatusValue != (int)TaskStatus.Deleted);
                    break;

                case "active":
                    query =
                        query.Where(
                            t =>
                            t.StatusValue == (int) TaskStatus.Created ||
                            t.StatusValue == (int) TaskStatus.Assigned ||
                            t.StatusValue == (int) TaskStatus.Executing);
                    break;

                case "completed":
                    query = query.Where(t => t.StatusValue == (int)TaskStatus.Completed);
                    break;
            }
            return query.ToList();
        }

        public Task AddTask(int projectId, Task task)
        {
            bool taskAssigned = task.ResponsibleId.GetValueOrDefault() > 0;

            if (taskAssigned)
                task.Status = TaskStatus.Assigned;
            else
                task.ResponsibleId = null;

            Database.Tasks.Add(task);
            Database.SaveChanges();

            //send emails
            if (taskAssigned)
                MessageService.SendEmail(task.Responsible.Email, "New task", "NewTask",
                                         new EmailDTO<Task> {User = task.Responsible, Model = task});
                    //send only one email
            else
            {
                //send email to everybody, except partial time programmers
                foreach (var teamMate in task.Project.Team.Where(tm => tm.AccessLevelValue != (int) AccessLevel.Partial)
                    )
                {
                    MessageService.SendEmail(teamMate.User.Email, "New task", "NewTask",
                                             new EmailDTO<Task> {User = teamMate.User, Model = task});
                }
            }
            return task;
        }

        public Task GetTask(int id)
        {
            return Database.Tasks.FirstOrDefault(t => t.Id == id);
        }

        public void UpdateTask(Task t)
        {
            if (t.CostType == CostType.Free) t.CostValue = null;

            Database.ObjectContext.ApplyCurrentValues("Tasks", t);
            Database.SaveChanges();
        }

        public void PostComment(int taskId, string message, int authorId)
        {
            Database.TaskComments.Add(new TaskComment {AuthorId = authorId, Text = message, Task = GetTask(taskId)});
            Database.SaveChanges();
        }

        public bool IsUserAllowedToDo(int taskId, int userId, TaskAction action)
        {
            bool result = false;
            var task = GetTask(taskId);
            switch (action)
            {
                case TaskAction.Show:
                    result = task.Project.Workspace.Owner.Id == userId ||
                             task.Project.Team.Any(t => t.UserId == userId && t.AccessLevel != AccessLevel.Partial) ||
                             task.ResponsibleId.GetValueOrDefault() == userId;
                    break;

                case TaskAction.Accept:
                    result = task.Project.Team.Any(t => t.UserId == userId && t.AccessLevel != AccessLevel.Partial);
                    break;
                case TaskAction.Start:
                case TaskAction.Complete:
                    result = task.ResponsibleId == userId;
                    break;
                case TaskAction.Delete:
                    result =
                        task.Project.Team.Any(t => t.UserId == userId && t.AccessLevel == AccessLevel.ProjectManager);
                    break;
            }
            return result;
        }

        public string StartTask(int taskId)
        {
            string msg="";
            var task = GetTask(taskId);
            if (task.CostType == CostType.PerTime)
                msg = "Time tracking for this task has been turned on";

            task.BeginWorkDate = DateTime.UtcNow;
            task.Status = TaskStatus.Executing;

            Database.ObjectContext.ApplyCurrentValues("Tasks", task);
            Database.SaveChanges();

            return msg;
        }

        public void CompleteTask(int taskId)
        {
            var task = GetTask(taskId);
            if (task.Status != TaskStatus.Executing)
                throw new Exception("Task should be started before completing");

            task.EndWorkDate = DateTime.UtcNow;
            task.Status = TaskStatus.Completed;

            Database.ObjectContext.ApplyCurrentValues("Tasks", task);
            Database.SaveChanges();
        }

        public void AcceptTask(int taskId, int userId)
        {
            var task = GetTask(taskId);

            task.Status = TaskStatus.Assigned;
            task.ResponsibleId = userId;

            Database.ObjectContext.ApplyCurrentValues("Tasks", task);
            Database.SaveChanges();
        }

        public void DeleteTask(int id)
        {
            Database.Tasks.FirstOrDefault(t => t.Id == id).Status = TaskStatus.Deleted;
            Database.SaveChanges();
        }
    }

    public enum TaskAction
    {
        Show,
        Accept,
        Start,
        Complete,
        Delete
    }
}
