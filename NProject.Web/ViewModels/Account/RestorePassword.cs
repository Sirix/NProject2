using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAnnotationsExtensions;

namespace NProject.Web.ViewModels.Account
{
    public class RestorePassword
    {
        [Required]
        [Email]
        public string Email { get; set; }
        public string Token { get; set; }

        public bool IsValidToken { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        [Required]
        public string NewPasswordConfirmation { get; set; }
    }
}