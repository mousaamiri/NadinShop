using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NadinShop.Application.Common.Behaviors;

namespace NadinShop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(ApplicationAssemblyReference.Assembly));

        //Pipeline (Order is important)
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
        //Register FluentValidation
        services.AddValidatorsFromAssembly(ApplicationAssemblyReference.Assembly,includeInternalTypes:true);
        //Register Mapster mappings
        TypeAdapterConfig.GlobalSettings.Scan(ApplicationAssemblyReference.Assembly);

        return services;
    }
}
