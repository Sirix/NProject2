using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NProject.Web.ViewModels.Home
{
    public class Settings
    {
        public string Locale { get; set; }

        public byte HoursOffset { get; set; }
    }
}