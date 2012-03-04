namespace NProject.Models.Infrastructure
{
    public interface IEntityWithIntegerKey
    {
        int Id { get; set; }
    }
}

namespace NProject.Models.Domain
{
    using System.Linq;
    using Infrastructure;

    public static class QueryableExtension
    {
        public static T GetByKey<T>(this IQueryable<T> list, int key) where T : IEntityWithIntegerKey
        {
            return list.FirstOrDefault(i => i.Id == key);
        }
    }
}
