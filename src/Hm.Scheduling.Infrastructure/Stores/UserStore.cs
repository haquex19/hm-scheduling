using Hm.Scheduling.Core.Entities;
using Hm.Scheduling.Core.Stores;
using Hm.Scheduling.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Hm.Scheduling.Infrastructure.Stores;

public class UserStore(HmDbContext context) : BaseStore<User>(context), IUserStore
{
    public override Task CreateAsync(User entity)
    {
        entity.NormalizedEmail = entity.Email.ToUpperInvariant();
        return base.CreateAsync(entity);
    }

    public override Task UpdateAsync(User entity)
    {
        entity.NormalizedEmail = entity.Email.ToUpperInvariant();
        return base.UpdateAsync(entity);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        var normalizedEmail = email.ToUpperInvariant();
        return await Table.FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var normalizedEmail = email.ToUpperInvariant();
        return await Table.AnyAsync(x => x.NormalizedEmail == normalizedEmail);
    }
}
