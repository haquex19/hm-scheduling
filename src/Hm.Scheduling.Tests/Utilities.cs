using Bogus;
using Hm.Scheduling.Core.Entities;
using Hm.Scheduling.Core.Extensions;
using Hm.Scheduling.Core.Services;
using Hm.Scheduling.Core.Settings;
using Hm.Scheduling.Infrastructure.Database;
using Hm.Scheduling.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hm.Scheduling.Tests;

public static class Utilities
{
    public static async Task<DatabaseDeletableServiceProvider> GetDefaultServiceProviderAsync(
        IHmClock? hmClockMock = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var applicationSettings = configuration
            .GetSection(nameof(ApplicationSettings))
            .Get<ApplicationSettings>()!;

        applicationSettings.ConnectionString = string.Format(
            applicationSettings.ConnectionString,
            $"ut{Guid.NewGuid().ToString().Replace("-", string.Empty)}");

        var services = new ServiceCollection()
            .AddInfrastructure(applicationSettings)
            .AddCore();

        if (hmClockMock is not null)
        {
            var descriptor = services.First(x => x.ServiceType == typeof(IHmClock));
            services.Remove(descriptor);
            services.AddSingleton(hmClockMock);
        }

        var serviceProvider = services.BuildServiceProvider(true);
        await using (var scope = serviceProvider.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<HmDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        return new DatabaseDeletableServiceProvider(serviceProvider);
    }

    public static async Task<User> CreateUserAsync(IServiceScope scope, UserType type = UserType.Client)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<HmDbContext>();

        var faker = new Faker();
        var email = faker.Internet.Email();
        var user = new User
        {
            FirstName = faker.Name.FirstName(),
            LastName = faker.Name.LastName(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            Type = type
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user;
    }
}
