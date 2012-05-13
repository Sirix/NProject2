using System.Linq;
using System.Web.Mvc;
using NProject.BLL;
using NProject.Models.Domain;
using NProject.Web.Helpers;
using NProject.Web.ViewModels.Task;

namespace NProject.Web.Controllers
{
    [HandleError]
    [Authorize]
    public class TaskController : BaseController
    {
        public ActionResult List(int id, string show = "active")
        {
            var ps = new ProjectService();
            if (!ps.IsUserAllowedToDo(id, SessionStorage.User.Id, ProjectAction.SeeTaskList))
                return RedirectToAction("Index", "Home");

            var project = ps.GetProjectById(id);
            var tasks = new TaskService().GetTasksForProject(id, show);
            var vm = new List
                         {
                             TaskFilter = show,
                             Tasks = tasks,
                             ProjectName = project.Name,
                             ProjectId = id,
                             CanUserEditTasks = ps.GetProjectById(id).Team.Any(
                                 t => t.UserId == SessionStorage.User.Id && t.AccessLevel == AccessLevel.ProjectManager)
                         };

            return View(vm);
        }
        #region Create
        
        public ActionResult Create(int id)
        {
            var ps = new ProjectService();
            if (!ps.IsUserAllowedToDo(id, SessionStorage.User.Id, ProjectAction.CreateOrEditTask))
                return RedirectToAction("Index", "Home");

            var model = new Form
                            {
                                CostTypes = UIHelper.CreateSelectListFromEnum<CostType>(),
                                Statuses = UIHelper.CreateSelectListFromEnum<TaskStatus>(),
                                Users = (from u in ps.GetProjectTeam(id)
                                         select new SelectListItem {Text = u.User.Name, Value = u.UserId.ToString()}).
                                    ToList(),
                                Task = new Task {ProjectId = id},
                                ProjectName = ps.GetProjectById(id).Name,
                                ProjectId = id,
                                IsCreation = true
                            };
            model.Users.Add(new SelectListItem {Text = "Free assigning", Value = "0"});

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Form f)
        {
            var ps = new ProjectService();
            if (!ps.IsUserAllowedToDo(f.Task.ProjectId, SessionStorage.User.Id, ProjectAction.CreateOrEditTask))
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid) return View(f);

            var ts = new TaskService();
            ts.AddTask(f.ProjectId, f.Task);

            return RedirectToAction("List", new {id = f.Task.ProjectId});
        }
        #endregion
        #region Edit
        
        public ActionResult Edit(int id)
        {
            var ts = new TaskService();
            var task = ts.GetTask(id);
            if (task == null)
                return RedirectToAction("Index", "Home");

            var ps = new ProjectService();
            if (!ps.IsUserAllowedToDo(task.ProjectId, SessionStorage.User.Id, ProjectAction.CreateOrEditTask))
                return RedirectToAction("List", new {id = task.ProjectId});

            var model = new Form
                            {
                                CostTypes = UIHelper.CreateSelectListFromEnum<CostType>(task.CostType),
                                Statuses = UIHelper.CreateSelectListFromEnum<TaskStatus>(task.Status),
                                Users = (from u in ps.GetProjectTeam(task.ProjectId)
                                         select new SelectListItem {Text = u.User.Name, Value = u.UserId.ToString()}).
                                    ToList(),
                                Task = task,
                                ProjectId = task.ProjectId,
                                ProjectName = task.Project.Name
                            };
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Form t)
        {
            var ts = new TaskService();
            var task = ts.GetTask(t.Task.Id);
            if (task == null)
                return RedirectToAction("Index", "Home");

            var ps = new ProjectService();
            if (!ps.IsUserAllowedToDo(t.Task.ProjectId, SessionStorage.User.Id, ProjectAction.CreateOrEditTask))
                return RedirectToAction("List", new { id = task.Id });


            if (!ModelState.IsValid) return View(t);

            ts.UpdateTask(t.Task);

            this.SetTempMessage(Resources.Task_Updated, "success");
            return RedirectToAction("List", new {id = t.Task.ProjectId});
        }
        #endregion
        public ActionResult Show(int id)
        {
            var ts = new TaskService();
            var task=ts.GetTask(id);
            var vm = new Show
                         {
                             Task = task,
                             ProjectId = task.ProjectId,
                             ProjectName = task.Project.Name,
                             CanUserAcceptTask = !task.ResponsibleId.HasValue,
                             AssignedForCurrentUser = task.ResponsibleId.GetValueOrDefault() == SessionStorage.User.Id
                         };

            return View(vm);
        }
        public ActionResult PostComment(int id, string message)
        {
            var ts = new TaskService();
            ts.PostComment(id, message, SessionStorage.User.Id);

            return RedirectToAction("Show", new {id = id});
        }

        public ActionResult Start(int id)
        {
            var ts = new TaskService();
            if(!ts.IsUserAllowedToDo(id, SessionStorage.User.Id, TaskAction.Start))
                return RedirectToAction("Index", "Home");

            string res = ts.StartTask(id);

            return RedirectToAction("Show", new { id });
        }

        public ActionResult Complete(int id)
        {
            var ts = new TaskService();
            if (!ts.IsUserAllowedToDo(id, SessionStorage.User.Id, TaskAction.Complete))
                return RedirectToAction("Index", "Home");

            ts.CompleteTask(id);

            return RedirectToAction("Show", new { id });
        }

        public ActionResult Accept(int id)
        {
            var ts = new TaskService();
            if (!ts.IsUserAllowedToDo(id, SessionStorage.User.Id, TaskAction.Accept))
                return RedirectToAction("Index", "Home");

            ts.AcceptTask(id, SessionStorage.User.Id);
            return RedirectToAction("Show", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var ts = new TaskService();
            if (!ts.IsUserAllowedToDo(id, SessionStorage.User.Id, TaskAction.Delete))
                return RedirectToAction("Index", "Home");

            var task = ts.GetTask(id);
            if (task == null)
                return RedirectToAction("Index", "Home");

            ts.DeleteTask(id);
            return RedirectToAction("List", "Task", new {id});
        }

        #region OldCode-not working
        /*
         
        [Dependency]
        public TaskService TaskService { get; set; }

        [Dependency]
        public INProjectEntities AccessPoint { get; set; }

        private Project _project;
        /// <summary>
        /// Add task to project which specified by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        public ActionResult AddToProject(int id = 0)
        {
            var result = ValidateAccessToProject(id);
            if (result != null) return result;

            var model = GetTaskFormViewModel(id);
            
            return View(model);
        }
        //[Authorize(Roles="PM")]
        
        public ActionResult AtProject(int id)
        {
            var tasks = TaskService.GetTasksForProject(id);
            var vm = new TaskListViewModel {Tasks = tasks};

            var project = (new ProjectService()).GetProjectById(id);
            //ValidateAccessToProject(project, "PM", "You're not eligible to view tasks of this project.");

            ViewData["ProjectId"] = id;
            ViewData["ProjectTitle"] = project.Name;
            ViewData["CanCreateTasks"] = SessionStorage.User.Role == UserRole.Manager;
            ViewData["CanExecuteTask"] = SessionStorage.User.Role == UserRole.Programmer;
            return View(vm);
        }


        /// <summary>
        /// Validates currently logged user access to project, which specified by id.
        /// </summary>
        /// <param name="id">Project id</param>
        private ActionResult ValidateAccessToProject(int id)
        {
            var user = AccessPoint.Users.First(u => u.Name == User.Identity.Name);
            _project = AccessPoint.Projects.FirstOrDefault(p => p.Id == id);

            if (_project == null)
            {
                TempData["ErrorMessage"] = "Selected project does not exist.";
                //RedirectToAction("List", "Project").ExecuteResult(ControllerContext);
                return RedirectToAction("List", "Project");
            }

            if (!_project.Team.Select(t => t.User).Contains(user))
            {
                TempData["ErrorMessage"] = "You're not eligible to manage tasks of this project.";
                //RedirectToAction("List", "Project").ExecuteResult(ControllerContext);
                return RedirectToAction("List", "Project");
            }
            return null;
        }

        /// <summary>
        /// Creates and fills view model to addToProject view.
        /// </summary>
        /// <param name="projectId">Target project id</param>
        /// <param name="modelToRefresh">If specified, view model will be refreshed instead of creating new</param>
        /// <returns></returns>
        private TaskFormViewModel GetTaskFormViewModel(int projectId, TaskFormViewModel modelToRefresh = null)
        {
            var model = modelToRefresh ?? new TaskFormViewModel();
            var project = _project ?? AccessPoint.Projects.First(p => p.Id == projectId);

            model.ProjectId = projectId;
            model.ProjectTitle = project.Name;
            model.Statuses = UIHelper.CreateSelectListFromEnum<ItemStatus>();

                //AccessPoint.ProjectStatuses.Select(
                //    p => new SelectListItem {Text = p.Name, Value = SqlFunctions.StringConvert((double) p.Id)}).
                //    ToList();

            model.Programmers =
                project.Team.Where(u => u.Role == UserRole.Programmer).Select(
                    u =>
                    new SelectListItem
                        {
                            Text = u.User.Name,
                            Value = u.User.Id.ToString(),
                        });

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToProject(TaskFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //if model has no info about target project, we can't refill it, so show error
                if (model.ProjectId == 0)
                {
                    TempData["ErrorMessage"] = "Unable to detect target project.";
                    return RedirectToAction("List", "Project");
                }
                model = GetTaskFormViewModel(model.ProjectId, model);
                return View(model);
            }
            var result = ValidateAccessToProject(model.ProjectId);
            if (result != null) return result;

            var t = model.Task;
            //_project already selected by ValidateAccessToProject
            t.Project = _project;

            t.Status = AccessPoint.ProjectStatuses.First(s => s.Id == model.StatusId);
            //t.Status = (ItemStatus)model.StatusId;
                
            var responsible = AccessPoint.Users.FirstOrDefault(u => u.Id == model.ResponsibleUserId);
            if (responsible == null)
            {
                TempData["ErrorMessage"] = "Unable to detect target responsible user.";
                model = GetTaskFormViewModel(model.ProjectId, model);
                return View(model);
            }
            t.Responsible = responsible;
            t.CreationDate = DateTime.Now;
            _project.Tasks.Add(t);

            AccessPoint.SaveChanges();
            TempData["InformMessage"] = "Task created.";
            return RedirectToAction("AtProject", "Task", new {id = t.Project.Id});
        }
        //
        // GET: /Task/Edit/5

        
        public ActionResult Edit(int id)
        {
            var user = AccessPoint.Users.First(u => u.Name == User.Identity.Name);
            var task = AccessPoint.Tasks.FirstOrDefault(i => i.Id == id);
            if (task == null)
            {
                TempData["ErrorMessage"] = "No task found.";
                return RedirectToAction("List", "Project");
            }
            //if this is a pm of a project
            if (!task.Project.Team.Select(t => t.User).Contains(user))
            {
                TempData["ErrorMessage"] = "You're not eligible to modify this task.";
                return RedirectToAction("AtProject", "Task", new {id = task.Project.Id});
            }
            var model = GetTaskFormViewModel(task.Project.Id);
            model.Task = task;
            model.Statuses.First(i => int.Parse(i.Value) == task.Status.Id).Selected = true;
            model.Programmers.First(i => int.Parse(i.Value) == task.Responsible.Id).Selected = true;

            return View(model);
        }

        //
        // POST: /Task/Edit/5

        [HttpPost]
        
        [ ValidateAntiForgeryToken]
        public ActionResult Edit(TaskFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = AccessPoint.Users.First(u => u.Name == User.Identity.Name);
                var task = AccessPoint.Tasks.First(i => i.Id == model.Task.Id);
                if (task == null)
                {
                    TempData["ErrorMessage"] = "No task found.";
                    return RedirectToAction("List", "Project");
                }
                //if current user not a pm of the project
                if (!task.Project.Team.Any(t => t.User.Id == user.Id))
                {
                    TempData["ErrorMessage"] = "You're not eligible to edit this task.";
                    return RedirectToAction("AtProject", "Task", new {id = task.Project.Id});
                }
                //check for a new responsible user
                var respUser = AccessPoint.Users.FirstOrDefault(i => i.Id == model.ResponsibleUserId);
                if (respUser == null || !task.Project.Team.Any(t => t.User.Id == respUser.Id))
                {
                    TempData["ErrorMessage"] = "Selected responsible user is not in this project's team.";
                    return RedirectToAction("AtProject", "Task", new { id = task.Project.Id });
                }

                AccessPoint.ObjectContext.ApplyCurrentValues("Tasks", model.Task);
                task.Responsible = respUser;
                var status = AccessPoint.ProjectStatuses.First(i => i.Id == model.StatusId);
                task.Status = status;
                AccessPoint.SaveChanges();

                TempData["InformMessage"] = "Task has been updated.";
                return RedirectToAction("AtProject", "Task", new { id = task.Project.Id });
            }
            else
            {
                //if model has no info about task, we can't refill it, so show error
                if (model.Task.Id == 0)
                {
                    TempData["ErrorMessage"] = "Unable to detect target task.";
                    return RedirectToAction("List", "Project");
                }
                model = GetTaskFormViewModel(model.ProjectId, model);
                model.Statuses.First(i => int.Parse(i.Value) == model.StatusId).Selected = true;
                model.Programmers.First(i => int.Parse(i.Value) == model.ResponsibleUserId).Selected = true;

                return View(model);
            }
        }

        //
        // GET: /Task/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Task/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Take(int id)
        {
            var user = AccessPoint.Users.First(u => u.Name == User.Identity.Name);
            var task = AccessPoint.Tasks.First(t => t.Id == id);
            //check for user is responsible for this task
            if (task.Responsible.Id != user.Id)
            {
                TempData["ErrorMessage"] = "You're not eligible to take this task";
                return RedirectToAction("Tasks", "Project", new {id = task.Project.Id});
            }
            //check for already taken task
            if (task.Status.Name == "Developing")
            {
                TempData["ErrorMessage"] = "You already took this task.";
                return RedirectToAction("Tasks", "Project", new {id = task.Project.Id});
            }
            task.Status = AccessPoint.ProjectStatuses.First(p => p.Name == "Developing");
            AccessPoint.SaveChanges();
            TempData["InformMessage"] = "You have taken this task";
            return RedirectToAction("Tasks", "Project", new {id = task.Project.Id});
        }

        public ActionResult Complete(int id)
        {
            CheckConditionsForCompleteTask(id);
            var task = AccessPoint.Tasks.First(t => t.Id == id);

            ViewData["Statuses"] =
               AccessPoint.ProjectStatuses.Select(
                   u => new SelectListItem { Text = u.Name, Value = SqlFunctions.StringConvert((double)u.Id) });
            return View(task);
        }

        [HttpPost]
        public ActionResult Complete(int id, int statusId, int spentTime = 0)
        {
            CheckConditionsForCompleteTask(id);
            var task = AccessPoint.Tasks.First(t => t.Id == id);
            if (spentTime <= 0)
            {
                TempData["ErrorMessage"] = "Spent time in hours must be greater than zero.";
                ViewData["Statuses"] =
                    AccessPoint.ProjectStatuses.Select(
                        u => new SelectListItem {Text = u.Name, Value = SqlFunctions.StringConvert((double) u.Id)});
                return View(task);
            }
            //task.EstimatedTime = (task.EndDate - task.BeginDate);
            //task.EndDate = DateTime.Now;
            task.SpentTime = spentTime;
            task.Status = AccessPoint.ProjectStatuses.First(p => p.Id == statusId);
            AccessPoint.SaveChanges();

            TempData["InformMessage"] = "Task has been updated.";
            return RedirectToAction("Tasks", "Project", new {id = task.Project.Id});
        }

        private void CheckConditionsForCompleteTask(int taskId)
        {
            var user = AccessPoint.Users.First(u => u.Name == User.Identity.Name);
            var task = AccessPoint.Tasks.First(t => t.Id == taskId);
            //check for user is responsible for this task
            if (task.Responsible.Id != user.Id)
            {
                TempData["ErrorMessage"] = "You're not eligible to complete this task";
                RedirectToAction("Tasks", "Project", new {id = task.Project.Id}).ExecuteResult(ControllerContext);
            }
            //check for already finished task
            if (task.Status.Name == "Finished")
            {
                TempData["ErrorMessage"] = "This task already finished";
                RedirectToAction("Tasks", "Project", new {id = task.Project.Id}).ExecuteResult(ControllerContext);
            }
        }
         */
        #endregion


    }
}
