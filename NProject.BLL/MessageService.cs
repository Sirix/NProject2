using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using NProject.Models.Domain;

namespace NProject.BLL
{
    internal class MessageService
    {
        public static void SendEmail(string email, string subject, string message)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                MailMessage mm = new MailMessage("nproject.service@gmail.com", email)
                                     {
                                         IsBodyHtml = true,
                                         Subject = subject,
                                         Body = message
                                     };

                client.SendAsync(mm, null);
            }
            catch
            {
            }

        }
        private static string SiteUrl
        {
            get { return ConfigurationManager.AppSettings["SiteUrl"]; }
        }
        public void SendMeetingInvitation(string userEmail, Meeting meeting, bool alreadyregistered = true)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                MailMessage mm = new MailMessage("nproject.service@gmail.com", userEmail);
                mm.IsBodyHtml = true;
                if (alreadyregistered)
                    mm.Body =
                        string.Format(
                            "<h2>NProject</h2> Hello, <br> You have been invited to join meeting {0} on {1}. <br>" +
                            "<hr><br> NProject allows people to communicate meetings in realtime", meeting.Name,
                            meeting.EventDate);
                else
                    mm.Body =
                        string.Format(
                            "<h2>NProject</h2> Hello, <br> You have been invited to join meeting {0} on {1}. <br>" +
                            "You can login with the following credentials: Login and password - {2}" +

                            "<hr><br> NProject allows people to communicate meetings in realtime", meeting.Name,
                            meeting.EventDate, userEmail);
                mm.Subject = "You have received a new meeting invitation";
                client.Send(mm);
            }
            catch (Exception)
            {

            }
        }
        public static void SendRegistrationGreetings(string email, string userName, string password)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                MailMessage mm = new MailMessage("nproject.service@gmail.com", email);
                mm.IsBodyHtml = true;
                mm.Subject = "Welcome to NProject!";
                mm.Body =
                    string.Format(
                        "<h2>NProject</h2> Hello, {0}!<br><br> You have been successfully registered on NProject." +
                        " Now you can take all advantages of our service. <br><br>" +
                        "Your login data:<br>" +
                        "<b>Login:</b> {1}<br>" +
                        "<b>Password:</b> {2} (only you know your password)<br><br>" +
                        "Use this data to sign in on <a href=\"{4}\">NProject</a><br><br><br>" +
                        "<hr> NProject Service &copy; {3}", userName, email, password, DateTime.UtcNow.Year, SiteUrl);

                client.Send(mm);
            }
            catch
            {
            }
        }

        internal static void SendProjectInvite(string email, string inviteeName, string senderName, string projectName, string projectDescription, bool registeredUser)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                MailMessage mm = new MailMessage("nproject.service@gmail.com", email);
                mm.IsBodyHtml = true;
                mm.Subject = "Invitation to project.";
                if (registeredUser)
                {
                    mm.Body = string.Format(
                        "<h2>NProject</h2> Hello, {0}!<br><br> You have been invited by {1} to join project {2}{3} on NProject.<br><br>" +
                        "<hr> NProject Service &copy; {4}", inviteeName, senderName, projectName,
                        !string.IsNullOrEmpty(projectDescription) ? string.Format("({0})", projectDescription) : "", SiteUrl);

                    client.Send(mm);
                }
            }
            catch (Exception)
            {

            }
        }
        public static string RenderPartialViewToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                    ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                    viewResult.View.Render(viewContext, sw);

                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
