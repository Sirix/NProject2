﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NProject.Models.Domain;

namespace NProject.Models.Infrastructure
{
    internal class NProjectTestDatabaseInitializer : NProjectDatabaseInitializer
    {
        protected override void Seed(NProjectEntities context)
        {
            base.SetupDatabase(context);

            SeedTestData(context);
        }

        private void SeedTestData(NProjectEntities context)
        {
            var me = new User
                         {
                             Id = 1,
                             FirstName = "Ivan",
                             LastName = "Manzhos",
                             Email = "Sirix25@gmail.com",
                             PasswordHash = EncryptMD5("1234567890"),
                             HoursOffsetFromUtc = 2
                         };
            context.Users.Add(me);

            var otherUser = context.Users.Add(new User
                                                  {
                                                      Id = 2,
                                                      FirstName = "Mark",
                                                      LastName = "Johnson",
                                                      Email = "mark@site.com",
                                                      PasswordHash = EncryptMD5("1234567890"),
                                                      HoursOffsetFromUtc = 2
                                                  });

            var ws = context.Workspaces.Add(new Workspace {Name = "WS-1", Owner = me});
            var proj1 = context.Projects.Add(new Project {Id = 1, Name = "test-Proj", Workspace = ws});
            var tm1 =
                context.TeamMates.Add(new TeamMate
                                          {
                                              UserId = me.Id,
                                              ProjectId = proj1.Id,
                                              AccessLevel = AccessLevel.ProjectManager
                                          });

            context.TeamMates.Add(new TeamMate {UserId = otherUser.Id, ProjectId = proj1.Id});

            context.Tasks.Add(new Task
                                  {ProjectId = 1, ResponsibleId = 2, Name = "Test task", Description = "bla bla bla"});

            context.SaveChanges();
        }

        private static string EncryptMD5(string value)
        {
            var md5 = new MD5CryptoServiceProvider();
            var valueArray = Encoding.UTF8.GetBytes(value);
            valueArray = md5.ComputeHash(valueArray);
            var encrypted = "";
            for (var i = 0; i < valueArray.Length; i++)
                encrypted += valueArray[i].ToString("x2").ToLower();
            return encrypted;
        }
    }
}
