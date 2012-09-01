using System;
using Microsoft.Practices.ServiceLocation;
using NProject.Models.Domain;

namespace NProject.BLL
{
    public abstract class BaseService
    {
        protected INProjectEntities Database { get; private set; }

        protected BaseService()
        {
            Database = ServiceLocator.Current.GetInstance<INProjectEntities>();
        }
    }

    /// <summary>
    /// Exception is thrown by any of Service classes.
    /// This exception is used only in case of logic(validation, business rules) errors, but not 
    /// in database connection lost or etc..
    /// </summary>
    public class ServiceException : Exception
    {
        public enum ExceptionLevel
        {
            Warning,
            Error,
        }

        /// <summary>
        /// Describes level of exception. This property will be used to show a correct error box to end user.
        /// </summary>
        public ExceptionLevel Level { get; private set; }

        /// <summary>
        /// Key of message, used during localization process
        /// </summary>
        public string ResourceMessageKey { get; private set; }


        public ServiceException(string resourceKey, string message = "", ExceptionLevel level = ExceptionLevel.Error)
            : base(string.Format("({0}) {1}", resourceKey, message))
        {
            ResourceMessageKey = resourceKey;
            Level = level;
        }
    }
}
