using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Services;

public interface IProductService
{
    Task<ProductDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<IEnumerable<ProductDto>> GetFilteredAsync(ProductFilterDto filter);
    Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto updateProductDto);
    Task DeleteAsync(Guid id);
    Task<ProductDto> ActivateAsync(Guid id);
    Task<ProductDto> DeactivateAsync(Guid id);
    Task<ProductDto> UploadImageAsync(Guid id, Stream imageStream, string fileName, string contentType);
}