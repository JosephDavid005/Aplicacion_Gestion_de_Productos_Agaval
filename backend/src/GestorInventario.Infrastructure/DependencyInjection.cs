using GestorInventario.Application.Abstractions;
using GestorInventario.Infrastructure.Persistence;
using GestorInventario.Infrastructure.Repositories;
using GestorInventario.Infrastructure.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GestorInventario.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? "Server=localhost;Database=GestorInventarioDB;User Id=sa;Password=Password;TrustServerCertificate=True";

        services.AddDbContext<PersistenceContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<PersistenceContext>());
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<PersistenceContext>();
            if (context.Database.IsSqlServer())
            {
                await context.Database.EnsureCreatedAsync();
                await DataSeeder.SeedAsync(context);
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<PersistenceContext>>();
            logger.LogWarning(ex, "No se pudo conectar a la base de datos SQL Server durante el arranque (se reintentará en runtime).");
        }
    }
}
