using Hm.Scheduling.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Hm.Scheduling.Tests;

// This service will be used so that we can create a new database for each unit test. This allows us to test it in isolation.
public class DatabaseDeletableServiceProvider(ServiceProvider serviceProvider) : IServiceProvider,
    IDisposable,
    IAsyncDisposable
{
    private bool _disposed;

    public object? GetService(Type serviceType)
    {
        return serviceProvider.GetService(serviceType);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();

        Dispose(false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HmDbContext>();
                dbContext.Database.EnsureDeleted();
            }

            serviceProvider.Dispose();
        }

        _disposed = true;
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<HmDbContext>();
        await dbContext.Database.EnsureDeletedAsync();

        await serviceProvider.DisposeAsync();
    }
}
