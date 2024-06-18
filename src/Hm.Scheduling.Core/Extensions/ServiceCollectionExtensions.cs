using FluentValidation;
using Hm.Scheduling.Core.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Hm.Scheduling.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;
        services
            .AddMediatR(
                config => config
                    .RegisterServicesFromAssembly(assembly)
                    .AddOpenBehavior(typeof(ValidationBehavior<,>)))
            .AddAutoMapper(assembly)
            .AddValidatorsFromAssembly(assembly);

        return services;
    }
}
