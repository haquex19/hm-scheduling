using Hm.Scheduling.Core.Entities;

namespace Hm.Scheduling.Core.Stores;

public interface IStore<T> where T : class, IEntity
{
    Task<T?> FindByIdAsync(Guid id);

    Task CreateAsync(T entity);

    Task UpdateAsync(T entity);
}
