using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NProject.BLL;
using NProject.Helpers;
using NProject.Models.Domain;
using NProject.Web.Helpers;

namespace NProject.Controllers
{
    [Authorize]
    [HandleError]
    public class WorkspaceController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ViewBag.ActiveMenu = "workspaces";
        }
        //
        // GET: /Workspace/
        //Show list of all user's workspaces
        [Authorize]
        public ActionResult Index()
        {
            var cs = new WorkspaceService();
            int userId = SessionStorage.User.Id;
           // int workspaceId = cs.GetDefaultWorkspaceForUser(userId);

            return View(cs.GetWorkspacesForUser(userId));
        }

        [Authorize]
        public ActionResult Create()
        {
            //TODO: Check for user account type, if it is free or not
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(Workspace workspace)
        {
            if (!ModelState.IsValid) return View(workspace);

            workspace.Owner = new UserService().GetUser(SessionStorage.User.Id);
            new WorkspaceService().AddWorkspace(workspace);

            //TODO: Check for user account type, if it is free or not
            return RedirectToAction("Index");
        }

        public ActionResult Show(int id)
        {
            var ws = new WorkspaceService();
            var workspace = ws.GetWorkspace(id);
            if (workspace != null && ws.IsUserCanInteractWithWorkspace(SessionStorage.User.Id, id))
                return View(workspace);

            this.SetTempMessage("You can't see this page.", "error");
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var ws = new WorkspaceService();
            var workspace = ws.GetWorkspace(id);
            if (workspace != null && ws.IsUserCanInteractWithWorkspace(SessionStorage.User.Id, id))
                return View(workspace);

            this.SetTempMessage("You can't see this page.", "error");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(Workspace workspace)
        {
            var ws = new WorkspaceService();
            if (ws.IsUserCanInteractWithWorkspace(SessionStorage.User.Id, workspace.Id))
            {
                ws.UpdateWorkspace(workspace);
                this.SetTempMessage("Workspace has been updated.", "success");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var ws = new WorkspaceService();
            if (ws.IsUserCanInteractWithWorkspace(SessionStorage.User.Id, id))
            {
                ws.DeleteWorkspace(id);
                this.SetTempMessage("Workspace has been deleted.", "success");
            }
            return RedirectToAction("Index");
        }
    }
}