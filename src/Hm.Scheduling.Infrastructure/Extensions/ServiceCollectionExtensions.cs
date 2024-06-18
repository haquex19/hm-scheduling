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
            .AddDbContext<HmDbContext>(options => options.UseSqlite(settings.ConnectionString))
            .AddScoped<IAppointmentAvailabilityStore, AppointmentAvailabilityStore>()
            .AddScoped<IReservationStore, ReservationStore>()
            .AddScoped<IUserStore, UserStore>();

        return services;
    }
}
