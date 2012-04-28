using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using NProject.Models.Domain;

namespace NProject.BLL
{
    internal class MessageService
    {
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
