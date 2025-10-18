using ProductCatalog.Domain.Enums;

namespace ProductCatalog.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public string Category { get; private set; }
    public ProductStatus Status { get; private set; }
    public string? ImageUrl { get; private set; }

    private Product() { }

    public Product(string name, string description, decimal price, string category)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Price = price;
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Status = ProductStatus.Active;

        ValidateProduct();
    }

    public void UpdateProduct(string name, string description, decimal price, string category, ProductStatus status)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Price = price;
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Status = status;

        ValidateProduct();
    }

    public void SetImageUrl(string imageUrl)
    {
        ImageUrl = imageUrl;
    }

    public void Activate()
    {
        Status = ProductStatus.Active;
    }

    public void Deactivate()
    {
        Status = ProductStatus.Inactive;
    }

    private void ValidateProduct()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Product name cannot be empty", nameof(Name));

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Product description cannot be empty", nameof(Description));

        if (Price < 0)
            throw new ArgumentException("Product price cannot be negative", nameof(Price));

        if (string.IsNullOrWhiteSpace(Category))
            throw new ArgumentException("Product category cannot be empty", nameof(Category));
    }
}