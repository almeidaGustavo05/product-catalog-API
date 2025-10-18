using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Mappings;
using ProductCatalog.Application.Services;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Enums;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Tests.Application;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IImageStorageService> _mockImageStorageService;
    private readonly IMapper _mapper;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockImageStorageService = new Mock<IImageStorageService>();
        
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductMappingProfile>();
        }, loggerFactory);
        _mapper = configuration.CreateMapper();

        _productService = new ProductService(
            _mockProductRepository.Object,
            _mockImageStorageService.Object,
            _mapper);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        var productId = Guid.NewGuid();
        var product = new Product("Produto Teste", "Descrição", 99.99m, "Eletrônicos");
        
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        var result = await _productService.GetByIdAsync(productId);

        result.Should().NotBeNull();
        result.Name.Should().Be("Produto Teste");
        result.Description.Should().Be("Descrição");
        result.Price.Should().Be(99.99m);
        result.Category.Should().Be("Eletrônicos");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        var act = async () => await _productService.GetByIdAsync(productId);
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Produto com ID {productId} não encontrado.");
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct_WhenValidData()
    {
        var createDto = new CreateProductDto
        {
            Name = "Novo Produto",
            Description = "Nova Descrição",
            Price = 149.99m,
            Category = "Categoria Teste"
        };

        _mockProductRepository.Setup(x => x.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);
         
        var result = await _productService.CreateAsync(createDto);

        result.Should().NotBeNull();
        result.Name.Should().Be(createDto.Name);
        result.Description.Should().Be(createDto.Description);
        result.Price.Should().Be(createDto.Price);
        result.Category.Should().Be(createDto.Category);
        result.Status.Should().Be(ProductStatus.Active);

        _mockProductRepository.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct_WhenProductExists()
    {
        var productId = Guid.NewGuid();
        var existingProduct = new Product("Produto Original", "Descrição Original", 50m, "Categoria Original");
        var updateDto = new UpdateProductDto
        {
            Name = "Produto Atualizado",
            Description = "Descrição Atualizada",
            Price = 75m,
            Category = "Nova Categoria"
        };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        var result = await _productService.UpdateAsync(productId, updateDto);

        result.Should().NotBeNull();
        result.Name.Should().Be(updateDto.Name);
        result.Description.Should().Be(updateDto.Description);
        result.Price.Should().Be(updateDto.Price);
        result.Category.Should().Be(updateDto.Category);

        _mockProductRepository.Verify(x => x.UpdateAsync(existingProduct), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();
        var updateDto = new UpdateProductDto
        {
            Name = "Produto Atualizado",
            Description = "Descrição Atualizada",
            Price = 75m,
            Category = "Nova Categoria"
        };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        var act = async () => await _productService.UpdateAsync(productId, updateDto);
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Produto com ID {productId} não encontrado.");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProduct_WhenProductExists()
    {
        var productId = Guid.NewGuid();
        var product = new Product("Produto", "Descrição", 99.99m, "Categoria");

        _mockProductRepository.Setup(x => x.ExistsAsync(productId))
            .ReturnsAsync(true);
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        await _productService.DeleteAsync(productId);

        _mockProductRepository.Verify(x => x.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();

        _mockProductRepository.Setup(x => x.ExistsAsync(productId))
            .ReturnsAsync(false);

        var act = async () => await _productService.DeleteAsync(productId);
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Produto com ID {productId} não encontrado.");
    }

    [Fact]
    public async Task ActivateAsync_ShouldActivateProduct_WhenProductExists()
    {
        var productId = Guid.NewGuid();
        var product = new Product("Produto", "Descrição", 99.99m, "Categoria");
        product.Deactivate();

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        var result = await _productService.ActivateAsync(productId);

        result.Status.Should().Be(ProductStatus.Active);
        _mockProductRepository.Verify(x => x.UpdateAsync(product), Times.Once);
    }

    [Fact]
    public async Task DeactivateAsync_ShouldDeactivateProduct_WhenProductExists()
    {
        var productId = Guid.NewGuid();
        var product = new Product("Produto", "Descrição", 99.99m, "Categoria");

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        var result = await _productService.DeactivateAsync(productId);

        result.Status.Should().Be(ProductStatus.Inactive);
        _mockProductRepository.Verify(x => x.UpdateAsync(product), Times.Once);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnFilteredProducts()
    {
        var products = new List<Product>
        {
            new Product("Produto 1", "Descrição 1", 50m, "Eletrônicos"),
            new Product("Produto 2", "Descrição 2", 100m, "Livros")
        };

        var filter = new ProductFilterDto
        {
            Category = "Eletrônicos",
            MinPrice = 40m,
            MaxPrice = 60m,
            Status = ProductStatus.Active
        };

        _mockProductRepository.Setup(x => x.GetFilteredAsync(
            filter.Category, filter.MinPrice, filter.MaxPrice, filter.Status))
            .ReturnsAsync(products.Where(p => p.Category == "Eletrônicos"));

        var result = await _productService.GetFilteredAsync(filter);

        result.Should().HaveCount(1);
        result.First().Category.Should().Be("Eletrônicos");
    }
}