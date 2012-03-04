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
}
