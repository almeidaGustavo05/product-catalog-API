using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Mappings;
using ProductCatalog.Application.Services;
using ProductCatalog.Application.Validators;

namespace ProductCatalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // FluentValidation
        services.AddScoped<IValidator<CreateProductDto>, CreateProductDtoValidator>();
        services.AddScoped<IValidator<UpdateProductDto>, UpdateProductDtoValidator>();

        // AutoMapper
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<ProductMappingProfile>();
        });

        // Application Services
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}