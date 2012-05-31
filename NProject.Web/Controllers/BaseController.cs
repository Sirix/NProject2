using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NProject.BLL;
using NProject.Web.Helpers;

namespace NProject.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //initialize message service
            //we should pass controller context to message service because Razor depends on it - 
            //to get right urls, routes and so on...
            MessageService.Initialize(ControllerContext);

            //Check session
            if (Request.IsAuthenticated && SessionStorage.User == null)
            {
                Session["Expired"] = true;
                SetTempMessage("Your session expired. Please sign in again.", "Error");
                FormsAuthentication.SignOut();
                filterContext.Result =
                    new RedirectResult(string.Format("{0}?returnUrl={1}", FormsAuthentication.LoginUrl,
                                                     filterContext.HttpContext.Request.RawUrl));
            }

        }

        private const string DefaultCulture = "en-us";

        protected override void ExecuteCore()
        {
            string cultureName = null;
            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["NProject_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = DefaultCulture; 

            CultureInfo culture;
            try
            {
                culture = new CultureInfo(cultureName);
            }
            catch (CultureNotFoundException)
            {
                culture = new CultureInfo(DefaultCulture);
            }
            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

          
            base.ExecuteCore();
        }

        /// <summary>
        /// Outputs temporary message to user.
        /// </summary>
        /// <param name="controller">Controller instance</param>
        /// <param name="message">Message text</param>
        /// <param name="messageLevel">Message level - error, information</param>
        public void SetTempMessage(string message, string messageLevel)
        {
            TempData[messageLevel + "Message"] = message;
        }

        public string RenderEmailToString(string viewName, object model)
        {
            ViewData.Model = model;
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindView(ControllerContext, "Emails/" + viewName,
                                                                               "Emails/_emailLayout");
                    ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                    viewResult.View.Render(viewContext, sw);

                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        //protected override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    // Is it View ?
        //    ViewResultBase view = filterContext.Result as ViewResultBase;
        //    if (view == null) // if not exit
        //        return;

        //    string cultureName = Thread.CurrentThread.CurrentCulture.Name; // e.g. "en-US"

        //    //// Is it default culture? exit
        //    //if (cultureName == CultureHelper.GetDefaultCulture())
        //    //    return;


        //    //// Are views implemented separately for this culture?  if not exit
        //    //bool viewImplemented = CultureHelper.IsViewSeparate(cultureName);
        //    //if (viewImplemented == false)
        //    //    return;

        //    string viewName = view.ViewName;

        //    int i = 0;

        //    if (string.IsNullOrEmpty(viewName))
        //        viewName = filterContext.RouteData.Values["action"] + "." + cultureName; // Index.en-US
        //    else if ((i = viewName.IndexOf('.')) > 0)
        //    {
        //        // contains . like "Index.cshtml"                
        //        viewName = viewName.Substring(0, i + 1) + cultureName + viewName.Substring(i);
        //    }
        //    else
        //        viewName += "." + cultureName; // e.g. "Index" ==> "Index.en-Us"

        //    view.ViewName = viewName;

        //    filterContext.Controller.ViewBag._culture = "." + cultureName;

        //    base.OnActionExecuted(filterContext);
        //}
    }
}
