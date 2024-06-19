using Duende.IdentityServer.Models;
using Hm.Scheduling.Core.Settings;
using Hm.Scheduling.Core.Stores;
using Hm.Scheduling.Infrastructure.Database;
using Hm.Scheduling.Infrastructure.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hm.Scheduling.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, ApplicationSettings settings)
    {
        services
            .AddSingleton(Options.Create(settings))
            .AddDbContext<HmDbContext>(options => options.UseNpgsql(settings.ConnectionString))
            .AddAuth()
            .AddScoped<IAppointmentAvailabilityStore, AppointmentAvailabilityStore>()
            .AddScoped<IReservationStore, ReservationStore>()
            .AddScoped<IUserStore, UserStore>();

        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services
            .AddIdentityServer()
            .AddInMemoryApiScopes(
                new List<ApiScope>
                {
                    new ApiScope("api1", "My API")
                })
            .AddInMemoryClients(
                new List<Client>
                {
                    new Client
                    {
                        ClientId = "Client1",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets = new List<Secret>
                        {
                            new Secret("MySecret".Sha256())
                        },
                        AllowedScopes = new List<string>
                        {
                            "api1"
                        }
                    }
                });

        services
            .AddAuthentication()
            .AddJwtBearer(
                options =>
                {
                    options.Authority = "https://localhost:7096";
                    options.TokenValidationParameters.ValidateAudience = false;
                });

        services.AddAuthorization();
        return services;
    }
}
