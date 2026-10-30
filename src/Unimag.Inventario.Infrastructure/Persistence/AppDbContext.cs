using Microsoft.EntityFrameworkCore;
using Unimag.Inventario.Domain.Categories;
using Unimag.Inventario.Domain.Products;
using Unimag.Inventario.Domain.Sales;
using Unimag.Inventario.Domain.Tenants;
using Unimag.Inventario.Domain.Users;

namespace Unimag.Inventario.Infrastructure.Persistence;

/// <summary>
/// The only EF Core DbContext in the solution. Entity configurations live
/// next to the domain they belong to (see Samples/Persistence) and are
/// picked up automatically below.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Sale> Sales => Set<Sale>();

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
