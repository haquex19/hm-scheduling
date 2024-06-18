using Hm.Scheduling.Core.Entities;
using Hm.Scheduling.Core.Stores;
using Hm.Scheduling.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Hm.Scheduling.Infrastructure.Stores;

public class BaseStore<T> : IStore<T> where T : class, IEntity
{
    protected BaseStore(HmDbContext context)
    {
        Context = context;
        Table = context.Set<T>();
    }

    protected HmDbContext Context { get; }

    protected DbSet<T> Table { get; }

    public async Task<T?> FindByIdAsync(Guid id)
    {
        return await Table.FirstOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task CreateAsync(T entity)
    {
        Table.Add(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        await Context.SaveChangesAsync();
    }
}
