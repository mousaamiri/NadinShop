using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Common.Services;
using NadinShop.Infrastructure.Data;
using NadinShop.Infrastructure.Data.Entities;
using NadinShop.Infrastructure.Repositories;
using NadinShop.Infrastructure.Services;

namespace NadinShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddDbContext<NadinShopDbContext>(options => options.UseSqlServer(connectionString));

        var jwtOptions = JwtOptions.FromConfiguration(configuration);
        jwtOptions.Validate();
        services.AddSingleton(jwtOptions);
        services.TryAddSingleton(TimeProvider.System);

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<NadinShopDbContext>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IJwTokenService, JwtTokenService>();

        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
