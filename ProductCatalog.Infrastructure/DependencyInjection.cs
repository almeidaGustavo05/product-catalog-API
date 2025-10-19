using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        services.AddDbContext<ProductCatalogDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();

        // Infrastructure Services
        services.AddScoped<IImageStorageService, LocalImageStorageService>();

        return services;
    }
}