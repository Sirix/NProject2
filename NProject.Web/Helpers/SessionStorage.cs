using System;
using System.Web;
using NProject.Models.Domain;

namespace NProject.Web.Helpers
{
    [Serializable]
    public class UserSessionInfo
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte HoursOffsetFromUtc { get; set; }
        public UserAccountType AccountType { get; set; }
    }

    public static class SessionStorage
    {
        public static UserSessionInfo User
        {
            get { return (UserSessionInfo) HttpContext.Current.Session["UserSessionInfo"]; }
            set { HttpContext.Current.Session["UserSessionInfo"] = value; }
        }
    }
}