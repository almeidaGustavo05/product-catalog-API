using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.Data;
using ProductCatalog.Infrastructure.Repositories;
using ProductCatalog.Infrastructure.Services;

namespace ProductCatalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Entity Framework
        var connectionString = configuration.GetConnectionString("DbConfig") ?? configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ProductCatalogDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();

        // Infrastructure Services
        services.AddScoped<IImageStorageService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<LocalImageStorageService>>();
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            return new LocalImageStorageService(logger, webRootPath, "/images");
        });

        return services;
    }
}