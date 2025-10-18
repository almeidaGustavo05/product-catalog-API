using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Mappings;
using ProductCatalog.Application.Services;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.Data;
using ProductCatalog.Infrastructure.Repositories;
using ProductCatalog.Infrastructure.Services;
using ProductCatalog.API.Middleware;
using ProductCatalog.Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(CreateProductDtoValidator).Assembly);

builder.Services.AddDbContext<ProductCatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<ProductMappingProfile>();
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IImageStorageService, LocalImageStorageService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Product Catalog API", 
        Version = "v1",
        Description = "API para gerenciamento de catálogo de produtos"
    });
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Catalog API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseStaticFiles();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
