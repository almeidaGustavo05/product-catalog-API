namespace ProductCatalog.Application.Interfaces;

using ProductCatalog.Application.DTOs;

public interface IProductService
{
    Task<ProductDto> GetByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category);
    Task<IEnumerable<ProductDto>> GetFilteredAsync(ProductFilterDto filter);
    Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto updateProductDto);
    Task DeleteAsync(int id);
    Task<ProductDto> ActivateAsync(int id);
    Task<ProductDto> DeactivateAsync(int id);
    Task<ProductDto> UploadImageAsync(int id, Stream imageStream, string fileName, string contentType);
    Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm);
}