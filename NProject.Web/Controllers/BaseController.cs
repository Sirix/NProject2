﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace NProject.Web.Controllers
{
    public abstract class BaseController : Controller
    {
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