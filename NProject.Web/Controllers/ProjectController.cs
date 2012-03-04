using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using NProject.BLL;
using NProject.Web.Helpers;
using NProject.Web.ViewModels.Project;

namespace NProject.Web.Controllers
{
    [HandleError]
    [Authorize]
    public class ProjectController : Controller
    {
        public ActionResult Create(int id)
        {
            var ws = new WorkspaceService();
            if (!ws.IsUserCanInteractWithWorkspace(SessionStorage.User.Id, id))
                return RedirectToAction("Index", "Workspace");

            return View(new CreateForm {WorkspaceId = id});
        }

        [HttpPost]
        public ActionResult Create(CreateForm p)
        {
            var ws = new WorkspaceService();
            if (!ws.IsUserCanInteractWithWorkspace(SessionStorage.User.Id, p.WorkspaceId))
                return RedirectToAction("Index", "Workspace");

            if (!ModelState.IsValid) return View(p);

            int projectId = new ProjectService().CreateProject(p.Name, p.WorkspaceId, SessionStorage.User.Id,
                                                               p.Description);

            return RedirectToAction("Show", "Project", new { id = projectId });
        }

        public ActionResult Show(int id)
        {
            var ps = new ProjectService();
            var project = ps.GetProjectById(id);
            if (project != null && ps.IsUserCanSeeProject(SessionStorage.User.Id, id))
                return View(project);

            return RedirectToAction("Index", "Workspace");
        }

        public ActionResult Team(int id)
        {
            var ps = new ProjectService();
            var project = ps.GetProjectById(id);
            if (project == null || !ps.IsUserCanSeeProject(SessionStorage.User.Id, id))
                return RedirectToAction("Index", "Workspace");

            var vm = new TeamList
                         {
                             ProjectManager = project.ProjectManager,
                             ProjectId = id,
                             ProjectName = project.Name,
                             Team = from t in project.Team
                                    select
                                        new TeamList.TeamMate {UserId = t.UserId, Username = t.User.Name},
                             CanChangePM = project.Workspace.Owner.Id == SessionStorage.User.Id
                         };

            return View(vm);
        }

        public ActionResult Invite(int id)
        {
            var ps = new ProjectService();
            var usersInfo = ps.GetUsersForInviteToProject(id);

            usersInfo.Add(41, "Mark");
            usersInfo.Add(54, "Alex");
            
            ViewBag.Id = id;
            return View(usersInfo);
        }

        [HttpPost]
        public ActionResult Invite(int id, string email = "", int[] userIds = null)
        {
            Thread.Sleep(1000);
            if (email != "")
            {
                var us = new UserService();
                int uid = us.GetUserIdByEmail(email);
                if (uid == -1)
                {
                    //send an e-mail
                }
                else
                {
                    us.SendInvite(uid, SessionStorage.User.Id, id);
                    return new ContentResult {Content = "Message sent!"};
                }
            }
                
            if (userIds != null)
            {
                return new ContentResult {Content = "count: " + userIds.Length};
            }
            return new ContentResult();
        }

        #region Old code-not working
        /*
         
        private ProjectService ProjectService { get; set; }

        [Dependency]
        public INProjectEntities AccessPoint { get; set; }

        public ProjectController()
        {
            ProjectService = new ProjectService();
        }

        [Authorize]
        public ActionResult Show(int id)
        {
            var ps = new ProjectService();
            var project = ps.GetProjectById(id);
            if (project != null && ps.IsUserCanSeeProject(SessionStorage.User.Id, id))
                return View(project);

            this.SetTempMessage("You can't see this page.", "error");
            return RedirectToAction("Index", "Home");
        }



        //
        // GET: /Projects/
        public ActionResult List()
        {
            var model = new ProjectListViewModel();
            int userId = SessionStorage.User.Id;
            var projects = ProjectService.GetProjectListForUserByRole(userId);

          
            model.Projects = projects;
          
            return View(model);
        }

        private void ValidateAccessToProject(Project project, string role, string errorMessage)
        {
            if (SessionStorage.User.Role != UserRole.TopManager &&
                !project.Team.Select(u => u.User.Id).Contains(SessionStorage.User.Id))
            {
                TempData["ErrorMessage"] = errorMessage;
                RedirectToAction("List").ExecuteResult(ControllerContext);
            }
        }

        /// <summary>
        /// Output form for add staff to project team
        /// </summary>
        /// <param name="id">Project id</param>
        /// <returns></returns>
        public ActionResult AddStaff(int id)
        {
            ViewData["ProjectId"] = id;
            var project = ProjectService.GetProjectById(id);
            ViewData["ProjectTitle"] = project.Name;
            ValidateAccessToProject(project, "PM", "You are not eligible to manage staff of this project");

            ////get active programmers
            //ViewData["Users"] = new SelectList(
            //    AccessPoint.Users.Where(
            //    u =>
            //    u.role == (int) UserRole.Programmer &&
            //    //u.state == (int) (UserState.Working) &&
            //    !u.Projects.Select(i => i.Id).Contains(project.Id)).ToList(), "Id", "Username");

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddStaff(int id, int userId)
        {
            //var project = ProjectService.GetProjectById(id);
            //ValidateAccessToProject(project, "PM", "You are not eligible to manage staff of this project.");
            //var user = AccessPoint.Users.First(u => u.Id == userId);
            //if (user.Role != UserRole.Programmer || user.UserState != UserState.Working)
            //{
            //    TempData["ErrorMessage"] = "You can add only working programmers to project.";
            //    return RedirectToAction("Team", new {id = id});
            //}

            //project.Team.Add(user);
            //AccessPoint.SaveChanges();
            TempData["InformMessage"] = "Programmer has been added to the project's team";
            return RedirectToAction("Team", new {id = id});
        }


        //
        // GET: /Project/Create

        [Authorize]
        public ActionResult Create(int id)
        {
            // TODO:Ensure user can use this workspace
            return View(new CreateForm {WorkspaceId = id});
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(CreateForm p)
        {
            if (!ModelState.IsValid) return View(p);

            int projectId = new ProjectService().CreateProject(p.Name, p.WorkspaceId, SessionStorage.User.Id);

            return RedirectToAction("Show", "Project", new {id = projectId});
        }



        //
        // POST: /Project/Create

        [HttpPut]
        public ActionResult Create(int statusId, int pmId, int customerId, Project p)
        {
            try
            {
                if (!ModelState.IsValid) throw new ArgumentException();

                var pmUser = AccessPoint.Users.First(i => i.Id == pmId);
                // if (pmUser.Role  != UserRole.Manager)
                //{
                //    TempData["ErrorMessage"] = "You must select PM in PM field.";
                //    throw new ArgumentException();

                //}
                var customUser = AccessPoint.Users.First(i => i.Id == customerId);
                //if (customUser.Role != UserRole.Customer)
                //{
                //    TempData["ErrorMessage"] = "You must select customer in customer field.";
                //    throw new ArgumentException();

                //}
                p.Status = AccessPoint.ProjectStatuses.First(i => i.Id == statusId);
                p.Team.Add(new TeamMate {Role = UserRole.Manager, User = pmUser});
                // p.Customer = customUser;
                AccessPoint.Projects.Add(p);
                AccessPoint.SaveChanges();

                return RedirectToAction("List");
            }
            catch (ArgumentException)
            {
                FillCreateEditForm();
                ViewData["PageTitle"] = "Create";
                return View();
            }
        }

        private void FillCreateEditForm()
        {
            //ViewData["PM"] =
            //  AccessPoint.Users.Where(u => u.role== (int)UserRole.Manager).Select(
            //      u => new SelectListItem { Text = u.Name, Value = SqlFunctions.StringConvert((double)u.Id) }).ToList();
            //ViewData["Customer"] =
            //    AccessPoint.Users.Where(u => u.role == (int)UserRole.Customer).Select(
            //        u => new SelectListItem { Text = u.Name, Value = SqlFunctions.StringConvert((double)u.Id) }).ToList();
            ViewData["Statuses"] =
                AccessPoint.ProjectStatuses.Select(
                    u => new SelectListItem {Text = u.Name, Value = SqlFunctions.StringConvert((double) u.Id)}).ToList();
        }

        //
        // GET: /Project/Edit/5

        
        public ActionResult Edit(int id)
        {
            var project = ProjectService.GetProjectById(id);
            ViewData["PageTitle"] = "Edit";
            FillCreateEditForm();
            //(ViewData["PM"] as IEnumerable<SelectListItem>).First(i => i.Text == project.Team.First(u => u.Role== UserRole.Manager).Name).
            //     Selected = true;
            //   (ViewData["Customer"] as IEnumerable<SelectListItem>).First(i => i.Text == project.Customer.Name).
            //      Selected = true;
            (ViewData["Statuses"] as IEnumerable<SelectListItem>).First(i => i.Text == project.Status.Name).
                Selected = true;

            return View("Create", project);
        }

        //
        // POST: /Project/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int statusId, int pmId, int customerId, Project p)
        {
            try
            {
                var project = ProjectService.GetProjectById(p.Id);
                if (!ModelState.IsValid) throw new ArgumentException();

                var pmUser = AccessPoint.Users.First(i => i.Id == pmId);
                //if (pmUser.Role != UserRole.Manager)
                //{
                //    TempData["ErrorMessage"] = "You must select PM in PM field.";
                //    throw new ArgumentException();

                //}
                var customUser = AccessPoint.Users.First(i => i.Id == customerId);
                //if (customUser.Role != UserRole.Customer)
                //{
                //    TempData["ErrorMessage"] = "You must select customer in customer field.";
                //    throw new ArgumentException();

                //}
                p.Status = AccessPoint.ProjectStatuses.First(i => i.Id == statusId);
                p.Team = project.Team;
                p.Tasks = project.Tasks;
                p.Team.Remove(p.Team.First(u => u.Role == UserRole.Manager));
                p.Team.Add(new TeamMate {Role = UserRole.Manager, User = pmUser});
                // p.Customer = customUser;
                var pref = AccessPoint.Projects.First(pr => pr.Id == p.Id);
                AccessPoint.ObjectContext.ApplyCurrentValues("Project", p);
                AccessPoint.SaveChanges();

                TempData["InformMessage"] = "Project has been updated.";
                return RedirectToAction("List");
            }
            catch (ArgumentException)
            {
                FillCreateEditForm();
                ViewData["PageTitle"] = "Edit";
                return View("Create", p);
            }
        }

        //
        // GET: /Project/Delete/5

        [Authorize(Roles = "Director")]
        public ActionResult Delete(int id)
        {
            var project = ProjectService.GetProjectById(id);
            return View(project);
        }

        //
        // POST: /Project/Delete/5

        [Authorize(Roles = "Director")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection values)
        {
            var project = ProjectService.GetProjectById(id);
            project.Team.Clear();

            AccessPoint.Projects.Remove(project);
            AccessPoint.SaveChanges();

            TempData["InformMessage"] = "Project removed";
            return RedirectToAction("List");
        }

        /// <summary>
        /// Outputs team on project with specified id
        /// </summary>
        /// <param name="id">Project id</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Team(int id)
        {
            var ps = new ProjectService();
            var project = ps.GetProjectById(id);
            if (project != null && ps.IsUserCanSeeProject(SessionStorage.User.Id, id))
            {
                var vm = new TeamList {ProjectId = id, ProjectName = project.Name};
                vm.Team = from t in project.Team
                          select
                              new TeamList.TeamMate
                                  {Id = t.Id, Username = t.User.Name, RoleDescriptor = t.Role.ToString()};

                return View(vm);
            }
            this.SetTempMessage("You can't see this page.", "error");
            return RedirectToAction("Index", "Home");
        }

        private void CheckRemoveStaffConditions(Project project, User user)
        {
            if (!project.Team.Any(t => t.User.Id == user.Id))
            {
                TempData["ErrorMessage"] =
                    "This user is not in the team of this project, so you can't remove himself from project team.";
                RedirectToAction("Team", new {id = project.Id}).ExecuteResult(ControllerContext);
            }
            if ((new UserService().GetUserRoleInProject(user, project) == UserRole.Manager))
            {
                TempData["ErrorMessage"] = "You can't remove yourself from project team.";
                RedirectToAction("Team", new {id = project.Id}).ExecuteResult(ControllerContext);
            }
            //user has unfinished tasks
            if (project.Tasks.Where(t => t.Responsible.Id == user.Id && t.Status.Name != "Finished").Any())
            {
                TempData["ErrorMessage"] =
                    "Please, transfer tasks of this programmer to other before his removing.\nOr, just complete them.";
                RedirectToAction("Team", new {id = project.Id}).ExecuteResult(ControllerContext);
            }
        }

        /// <summary>
        /// Removes user from project team
        /// </summary>
        /// <param name="id">Project id</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public ActionResult RemoveStaff(int id, int userId)
        {
            var project = ProjectService.GetProjectById(id);
            ValidateAccessToProject(project, "PM", "You are not eligible to manage team of this project.");
            var user = AccessPoint.Users.First(i => i.Id == userId);
            CheckRemoveStaffConditions(project, user);

            ViewData["UserName"] = user.FirstName;
            ViewData["UserId"] = user.Id;
            ViewData["ProjectName"] = project.Name;
            ViewData["ProjectId"] = project.Id;
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RemoveStaff(int id, int userId, FormCollection values)
        {
            var project = ProjectService.GetProjectById(id);
            ValidateAccessToProject(project, "PM", "You are not eligible to manage team of this project.");
            var user = AccessPoint.Users.First(i => i.Id == userId);
            CheckRemoveStaffConditions(project, user);

            project.Team.Remove(project.Team.First(t => t.User.Id == user.Id));
            AccessPoint.SaveChanges();
            TempData["InformMessage"] = string.Format("User {0} has been removed from \"{1}\"'s team", user.Name,
                                                      project.Name);

            return RedirectToAction("Team", new {id = project.Id});
        }

        public ActionResult Details(int id)
        {
            var project = ProjectService.GetProjectById(id);
            return View(project);


            var user = AccessPoint.Users.First(u => u.Id == SessionStorage.User.Id);
            var role = SessionStorage.User.Role;
            ViewData["ShowEditAction"] = role == UserRole.TopManager;
            switch (role)
            {
                case UserRole.TopManager:


                    //case UserRole.Customer:
                    //    if (project.Customer.Id == user.Id)
                    //        return View(project);
                    //    break;

                case UserRole.Manager:
                case UserRole.Programmer:
                    if (project.Team.Any(t => t.User.Id == user.Id))
                        return View(project);
                    break;
            }
            TempData["ErrorMessage"] = "You can't view information about this project";
            return RedirectToAction("List");
        }

        //teamMate id
        [Authorize]
        public ActionResult EditTeamMate(int id)
        {
            var ps = new ProjectService();
            TeamMate tm = ps.GetTeamMate(id);
            if (tm == null || !ps.IsUserCanEditTeamMate(SessionStorage.User.Id, tm.Id))
            {
                this.SetTempMessage("You can't see this page.", "error");
                return RedirectToAction("Index", "Home");
            }
            var vm = new EditTeamMate(tm.User.Name, tm.Id, (int)tm.Role);

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditTeamMate(EditTeamMate model)
        {
            var ps = new ProjectService();
            TeamMate tm = ps.GetTeamMate(model.TeamMate.Id);
            if (tm == null || !ps.IsUserCanEditTeamMate(SessionStorage.User.Id, tm.Id))
            {
                this.SetTempMessage("You can't see this page.", "error");
                return RedirectToAction("Index", "Home");
            }

            if (model.TeamMate.Role == 0)
            {
                ModelState.AddModelError("", "Role should be > 0");
                return View(model);
            }
            tm.Role = (UserRole)model.TeamMate.Role;
            ps.UpdateTeamMate(tm);
            this.SetTempMessage("Rights have been changed.", "Success");
            return RedirectToAction("Team", new {id = tm.Project.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Invite(int id)
        {
            int workspaceId = ProjectService.GetProjectById(id).Workspace.Id;
            var currentProjectTeam = ProjectService.GetProjectTeam(id);
            var curProjectTeam = currentProjectTeam.ToDictionary(t => t.User.Email, t => t.User.FirstName);
            var usersInfo = new WorkspaceService().GetUsersInWorkspaceProjects(workspaceId);
            usersInfo = usersInfo.Except(curProjectTeam).ToDictionary(t => t.Key, t => t.Value);

            usersInfo.Add("Mark", "Mark@mn.com");
            usersInfo.Add("Alex", "fr3@mai.ru");
            ViewBag.Id = id;
            return View(usersInfo);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Invite(int id, params string[] emails)
        {
          //  if (Request.IsAjaxRequest())
            //    return new JsonResult() {  };
            //else
                return RedirectToAction("Team", new {id = id});
        }*/
        #endregion
    }
}

