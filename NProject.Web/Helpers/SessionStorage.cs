using System;
using System.Web;

namespace NProject.Web.Helpers
{
    [Serializable]
    public class UserSessionInfo
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte HoursOffsetFromUtc { get; set; }
    }

    public static class SessionStorage
    {
        public static UserSessionInfo User
        {
            get { return HttpContext.Current.Session["UserSessionInfo"] as UserSessionInfo; }
            set { HttpContext.Current.Session["UserSessionInfo"] = value; }
        }
    }
}