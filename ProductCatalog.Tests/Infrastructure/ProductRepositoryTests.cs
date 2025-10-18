using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Enums;
using ProductCatalog.Infrastructure.Data;
using ProductCatalog.Infrastructure.Repositories;

namespace ProductCatalog.Tests.Infrastructure;

public class ProductRepositoryTests : IDisposable
{
    private readonly ProductCatalogDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ProductCatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ProductCatalogDbContext(options);
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProduct_WhenValidProduct()
    {
        var product = new Product("Produto Teste", "Descrição", 99.99m, "Eletrônicos");

        var result = await _repository.AddAsync(product);
        await _context.SaveChangesAsync();

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        
        var savedProduct = await _context.Products.FindAsync(result.Id);
        savedProduct.Should().NotBeNull();
        savedProduct!.Name.Should().Be("Produto Teste");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        var product = new Product("Produto Teste", "Descrição", 99.99m, "Eletrônicos");
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(product.Id);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Produto Teste");
        result.Description.Should().Be("Descrição");
        result.Price.Should().Be(99.99m);
        result.Category.Should().Be("Eletrônicos");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        var nonExistentId = 999;

        var result = await _repository.GetByIdAsync(nonExistentId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        var products = new List<Product>
        {
            new Product("Produto 1", "Descrição 1", 50m, "Eletrônicos"),
            new Product("Produto 2", "Descrição 2", 100m, "Livros"),
            new Product("Produto 3", "Descrição 3", 75m, "Roupas")
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Name == "Produto 1");
        result.Should().Contain(p => p.Name == "Produto 2");
        result.Should().Contain(p => p.Name == "Produto 3");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct_WhenProductExists()
    {
        var product = new Product("Produto Original", "Descrição Original", 50m, "Categoria Original");
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        product.UpdateProduct("Produto Atualizado", "Descrição Atualizada", 75m, "Nova Categoria");
        await _repository.UpdateAsync(product);
        await _context.SaveChangesAsync();

        var updatedProduct = await _context.Products.FindAsync(product.Id);
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Name.Should().Be("Produto Atualizado");
        updatedProduct.Description.Should().Be("Descrição Atualizada");
        updatedProduct.Price.Should().Be(75m);
        updatedProduct.Category.Should().Be("Nova Categoria");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProduct_WhenProductExists()
    {
        var product = new Product("Produto para Deletar", "Descrição", 99.99m, "Categoria");
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(product.Id);
        await _context.SaveChangesAsync();

        var deletedProduct = await _context.Products.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == product.Id);
        deletedProduct.Should().NotBeNull();
        deletedProduct!.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenProductExists()
    {
        var product = new Product("Produto Existente", "Descrição", 99.99m, "Categoria");
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync(product.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        var nonExistentId = 999;

        var result = await _repository.ExistsAsync(nonExistentId);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByCategoryAsync_ShouldReturnProductsFromCategory()
    {
        var products = new List<Product>
        {
            new Product("Produto 1", "Descrição 1", 50m, "Eletrônicos"),
            new Product("Produto 2", "Descrição 2", 100m, "Eletrônicos"),
            new Product("Produto 3", "Descrição 3", 75m, "Livros")
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByCategoryAsync("Eletrônicos");

        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Category == "Eletrônicos");
    }

    [Fact]
    public async Task GetByPriceRangeAsync_ShouldReturnProductsInPriceRange()
    {
        var products = new List<Product>
        {
            new Product("Produto 1", "Descrição 1", 30m, "Categoria"),
            new Product("Produto 2", "Descrição 2", 50m, "Categoria"),
            new Product("Produto 3", "Descrição 3", 80m, "Categoria"),
            new Product("Produto 4", "Descrição 4", 120m, "Categoria")
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByPriceRangeAsync(40m, 100m);

        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Price == 50m);
        result.Should().Contain(p => p.Price == 80m);
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnProductsWithStatus()
    {
        var products = new List<Product>
        {
            new Product("Produto Ativo 1", "Descrição 1", 50m, "Categoria"),
            new Product("Produto Ativo 2", "Descrição 2", 100m, "Categoria"),
            new Product("Produto Inativo", "Descrição 3", 75m, "Categoria")
        };

        products[2].Deactivate(); 

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        var activeProducts = await _repository.GetByStatusAsync(ProductStatus.Active);
        var inactiveProducts = await _repository.GetByStatusAsync(ProductStatus.Inactive);

        activeProducts.Should().HaveCount(2);
        activeProducts.Should().OnlyContain(p => p.Status == ProductStatus.Active);

        inactiveProducts.Should().HaveCount(1);
        inactiveProducts.Should().OnlyContain(p => p.Status == ProductStatus.Inactive);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnFilteredProducts()
    {
        var products = new List<Product>
        {
            new Product("Produto 1", "Descrição 1", 50m, "Eletrônicos"),
            new Product("Produto 2", "Descrição 2", 100m, "Eletrônicos"),
            new Product("Produto 3", "Descrição 3", 75m, "Livros"),
            new Product("Produto 4", "Descrição 4", 200m, "Eletrônicos")
        };

        products[1].Deactivate();

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        var result = await _repository.GetFilteredAsync(
            category: "Eletrônicos",
            minPrice: 40m,
            maxPrice: 150m,
            status: ProductStatus.Active);

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Produto 1");
        result.First().Category.Should().Be("Eletrônicos");
        result.First().Status.Should().Be(ProductStatus.Active);
        result.First().Price.Should().Be(50m);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}