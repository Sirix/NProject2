using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using NProject.Models.Domain;

namespace NProject.BLL
{
    internal class MessageService
    {
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
    }
}
