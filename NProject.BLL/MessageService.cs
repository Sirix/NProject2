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
    public class EmailDTO<T>
    {
        public User User { get; set; }
        public T Model { get; set; }
    }

    public class MessageService
    {
        private static ControllerContext context;

        public static void Initialize(ControllerContext ctx)
        {
            context = ctx;
        }

        private static string RenderEmailToString<T>(string viewName, EmailDTO<T> model)
        {
            ViewDataDictionary vd = new ViewDataDictionary(model);
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindView(context, "Emails/" + viewName,
                                                                               "Emails/_emailLayout");
                    ViewContext viewContext = new ViewContext(context, viewResult.View, vd, new TempDataDictionary(), sw);
                    viewResult.View.Render(viewContext, sw);

                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void SendEmail<T>(string email, string subject, string templateName, EmailDTO<T> model)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                MailMessage mm = new MailMessage("nproject.service@gmail.com", email)
                                     {
                                         IsBodyHtml = true,
                                         Subject = subject,
                                         Body = RenderEmailToString(templateName, model)
                                     };

                client.SendAsync(mm, null);
            }
            catch
            {
            }
        }

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
    }
}
