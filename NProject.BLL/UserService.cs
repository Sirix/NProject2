﻿using System;
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

        public User CreateUser(string name, string email, string password, byte timeshift)
        {
            //we have unique email constraint in db, so no need to check it here
            try
            {
                User user = Database.Users.Add(new User
                                                   {
                                                       Email = email,
                                                       FirstName = name,
                                                       LastName = "",
                                                       HoursOffsetFromUtc = timeshift,
                                                       PasswordHash = MD5.EncryptMD5(password),
                                                       RegistrationDate = DateTime.UtcNow
                                                   });
                Database.SaveChanges();
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

        public void SendInvite(int inviteeId, int senderId, int projectId)
        {
            Database.Invitations.Add(new Invitation
                                         {
                                             Invitee = Database.Users.First(u => u.Id == inviteeId),
                                             Sender = Database.Users.First(u => u.Id == senderId),
                                             ProjectId = projectId
                                         });
            Database.SaveChanges();
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
                    result = "You have blocked invitations to this project";
                    break;

            }
            Database.SaveChanges();
            return result;
        }

        public void SaveSettings(int userId, byte hoursOffset, string locale)
        {
            var user = Database.Users.FirstOrDefault(u => u.Id == userId);
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



