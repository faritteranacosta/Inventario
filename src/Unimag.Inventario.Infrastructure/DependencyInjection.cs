using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Categories.Contracts;
using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Products.Contracts;
using Unimag.Inventario.Domain.Sales.Contracts;
using Unimag.Inventario.Domain.Tenants.Contracts;
using Unimag.Inventario.Domain.Users.Contracts;
using Unimag.Inventario.Infrastructure.Auth;
using Unimag.Inventario.Infrastructure.Categories.Repositories;
using Unimag.Inventario.Infrastructure.Persistence;
using Unimag.Inventario.Infrastructure.Products.Repositories;
using Unimag.Inventario.Infrastructure.Sales.Repositories;
using Unimag.Inventario.Infrastructure.Storage;
using Unimag.Inventario.Infrastructure.Tenants.Repositories;
using Unimag.Inventario.Infrastructure.Users.Repositories;

namespace Unimag.Inventario.Infrastructure;

/// <summary>
/// Composition entry point for this layer. This is the only place in the
/// solution allowed to know about concrete adapters (EF Core, HTTP clients,
/// message brokers, etc).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Sample uses EF Core InMemory so the service runs with zero setup.
        // Swap this for UseSqlServer/UseNpgsql/etc. when wiring a real database.
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InventarioDb"));

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IFileStorage, LocalFileStorage>();

        return services;
    }
}