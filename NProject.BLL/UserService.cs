using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NProject.Models;
using NProject.Models.Domain;

namespace NProject.BLL
{
    public class UserService : BaseService
    {
        /// <summary>
        /// Retrieves the user instance from the database by passed id.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User instance, if exists; otherwise, null</returns>
        public User GetUser(int userId)
        {
            return Database.Users.FirstOrDefault(u => u.Id == userId);
        }
        public User IsUserExists(string email, string password)
        {
            string hash = MD5.EncryptMD5(password);
            var user = Database.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hash);
            return user;
        }

        public User CreateUser(string name,string lastName, string email, string password, byte timeshift)
        {
            //we have unique email constraint in db, so no need to check it here
            try
            {
                User user = Database.Users.Add(new User
                                                   {
                                                       Email = email,
                                                       FirstName = name,
                                                       LastName = lastName,
                                                       HoursOffsetFromUtc = timeshift,
                                                       PasswordHash = MD5.EncryptMD5(password),
                                                       RegistrationDate = DateTime.UtcNow
                                                   });
                Database.SaveChanges();

                //send email
                MessageService.SendEmail(user.Email, "Welcome to NProject!", "RegistrationGreetings",
                                         new EmailDTO<string> {User = user, Model = password});
                return user;
            }
            catch (Exception ex)
            {
                var be = ex.GetBaseException();

                if (be.Message.IndexOf("IXU_Email") > -1)
                    throw new Exception("This email is already in use!");

                throw be;
            }
        }

        public int GetUserIdByEmail(string email)
        {
            var user = Database.Users.FirstOrDefault(u => u.Email == email);

            return user == null ? -1 : user.Id;
        }

        public Invitation SendInvite(string inviteeEmail, int senderId, int projectId)
        {
            var sender = Database.Users.First(u => u.Id == senderId);
            var i = new Invitation
                        {
                            Sender = sender,
                            ProjectId = projectId
                        };
            if (string.Compare(sender.Email, inviteeEmail) == 0)
                throw new Exception("You can't invite yourself.");

            Invitation blocked;

            var possibleInvitee = Database.Users.FirstOrDefault(u => u.Email == inviteeEmail);
            if (possibleInvitee != null)
            {
                i.Invitee = possibleInvitee;
                blocked = Database.Invitations.FirstOrDefault(o => o.Sender.Id == senderId &&
                                                                   o.Invitee != null &&
                                                                   o.Invitee.Id == possibleInvitee.Id);
            }
            else
            {
                i.InviteeEmail = inviteeEmail;
                blocked = Database.Invitations.FirstOrDefault(o => o.Sender.Id == senderId &&
                                                                   o.InviteeEmail == inviteeEmail);
            }

            if (blocked != null && blocked.Status == InvitationStatus.Blocked)
                throw new Exception("This user blocked invitations from you.");

            //check already existing invitations
            if (blocked != null && blocked.ProjectId == projectId)
            {
                switch (blocked.Status)
                {
                    case InvitationStatus.Sent:
                        throw new Exception("You have already sent invitation to this user.");
                        break;
                    case InvitationStatus.Accepted:
                        throw new Exception("This user is already in your team.");
                        break;
                    case InvitationStatus.Declined:
                        throw new Exception("The user declined your invitation.");
                        break;
                }
            }

            Database.Invitations.Add(i);
            Database.SaveChanges();
            return i;
        }

        public Invitation SendInvite(int inviteeId, int senderId, int projectId)
        {
            var sender = Database.Users.First(u => u.Id == senderId);
            var invitee = Database.Users.First(u => u.Id == inviteeId);

            var i = new Invitation
                        {
                            Invitee = invitee,
                            Sender = sender,
                            ProjectId = projectId
                        };
            Database.Invitations.Add(i);
            Database.SaveChanges();
            return i;
        }

        public Invitation GetInvitation(int id)
        {
            return Database.Invitations.FirstOrDefault(i => i.Id == id);
        }

        public string ProcessInvitation(Invitation invitation, string verb, int userId)
        {
            string result = "Error";
            switch (verb)
            {
                case "accept":
                    Database.TeamMates.Add(new TeamMate
                                               {
                                                   ProjectId = invitation.ProjectId,
                                                   UserId = userId,
                                                   AccessLevel = invitation.AccessLevel
                                               });
                    invitation.Status = InvitationStatus.Accepted;
                    result = "You have successfully accepted the invitation";
                    break;
                case "decline":
                    invitation.Status = InvitationStatus.Declined;
                    result = "You have declined the invitation";
                    break;
                case "block":
                    invitation.Status = InvitationStatus.Blocked;
                    result = "You have blocked invitations from this user.";
                    break;

            }
            Database.SaveChanges();
            return result;
        }

        public void UpdateUserProfile(int userId,string firstName, string lastName, string password, byte hoursOffset, string locale)
        {
            var user = Database.Users.FirstOrDefault(u => u.Id == userId);
            user.FirstName = firstName;
            user.LastName = lastName;
            if (!string.IsNullOrEmpty(password))
                user.PasswordHash = MD5.EncryptMD5(password);
            user.HoursOffsetFromUtc = hoursOffset;
            user.Language = locale;

            Database.ObjectContext.ApplyCurrentValues("Users", user);
            Database.SaveChanges();
        }
    }
    public class ServiceException:Exception
    {

    }
}



