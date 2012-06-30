using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NProject.Web.ViewModels.Home
{
    public class Settings
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [ReadOnly(true)]
        public string Email { get; set; }

        public byte HoursOffsetFromUtc { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        public string Language { get; set; }
    }
}