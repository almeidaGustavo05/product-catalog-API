using FluentAssertions;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Enums;

namespace ProductCatalog.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void Product_Constructor_ShouldCreateValidProduct()
    {
        var name = "Produto Teste";
        var description = "Descrição do produto teste";
        var price = 99.99m;
        var category = "Eletrônicos";

        var product = new Product(name, description, price, category);

        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Should().Be(price);
        product.Category.Should().Be(category);
        product.Status.Should().Be(ProductStatus.Active);
        product.ImageUrl.Should().BeNull();
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Product_Constructor_ShouldThrowException_WhenNameIsInvalid(string invalidName)
    {
        var act = () => new Product(invalidName, "Descrição", 99.99m, "Categoria");
        act.Should().Throw<ArgumentException>()
           .WithMessage("Product name cannot be empty*");
    }

    [Fact]
    public void Product_Constructor_ShouldThrowException_WhenNameIsNull()
    {
        var act = () => new Product(null, "Descrição", 99.99m, "Categoria");
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("name");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Product_Constructor_ShouldThrowException_WhenDescriptionIsInvalid(string invalidDescription)
    {
        var act = () => new Product("Nome", invalidDescription, 99.99m, "Categoria");
        act.Should().Throw<ArgumentException>()
           .WithMessage("Product description cannot be empty*");
    }

    [Fact]
    public void Product_Constructor_ShouldThrowException_WhenDescriptionIsNull()
    {
        var act = () => new Product("Nome", null, 99.99m, "Categoria");
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("description");
    }

    [Fact]
    public void Product_Constructor_ShouldThrowException_WhenPriceIsNegative()
    {
        var act = () => new Product("Nome", "Descrição", -1m, "Categoria");
        act.Should().Throw<ArgumentException>()
           .WithMessage("Product price cannot be negative*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Product_Constructor_ShouldThrowException_WhenCategoryIsInvalid(string invalidCategory)
    {
        var act = () => new Product("Nome", "Descrição", 99.99m, invalidCategory);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Product category cannot be empty*");
    }

    [Fact]
    public void Product_Constructor_ShouldThrowException_WhenCategoryIsNull()
    {
        var act = () => new Product("Nome", "Descrição", 99.99m, null);
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("category");
    }

    [Fact]
    public void Product_UpdateProduct_ShouldUpdateProductSuccessfully()
    {
        var product = new Product("Nome Original", "Descrição Original", 50m, "Categoria Original");
        var originalUpdatedAt = product.UpdatedAt;
        
        Thread.Sleep(10);

        var newName = "Nome Atualizado";
        var newDescription = "Descrição Atualizada";
        var newPrice = 75m;
        var newCategory = "Nova Categoria";

        product.UpdateProduct(newName, newDescription, newPrice, newCategory);

        product.Name.Should().Be(newName);
        product.Description.Should().Be(newDescription);
        product.Price.Should().Be(newPrice);
        product.Category.Should().Be(newCategory);
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Product_SetImageUrl_ShouldSetImageUrlSuccessfully()
    {
        var product = new Product("Nome", "Descrição", 99.99m, "Categoria");
        var imageUrl = "https://example.com/image.jpg";

        product.SetImageUrl(imageUrl);

        product.ImageUrl.Should().Be(imageUrl);
    }

    [Fact]
    public void Product_Activate_ShouldSetStatusToActive()
    {
        var product = new Product("Nome", "Descrição", 99.99m, "Categoria");
        product.Deactivate(); 

        product.Activate();

        product.Status.Should().Be(ProductStatus.Active);
    }

    [Fact]
    public void Product_Deactivate_ShouldSetStatusToInactive()
    {
        var product = new Product("Nome", "Descrição", 99.99m, "Categoria");

        product.Deactivate();

        product.Status.Should().Be(ProductStatus.Inactive);
    }
}