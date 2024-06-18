using Hm.Scheduling.Core.Entities;

namespace Hm.Scheduling.Core.Stores;

public interface IUserStore : IStore<User>
{
    Task<User?> FindByEmailAsync(string email);

    Task<bool> ExistsByEmailAsync(string email);
}
