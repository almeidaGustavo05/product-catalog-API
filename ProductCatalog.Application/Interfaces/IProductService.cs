using Microsoft.AspNetCore.Http;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Domain.Pagination;

namespace ProductCatalog.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto> GetByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<PageList<ProductDto>> GetPagedAsync(PageParams pageParams);
    Task<IEnumerable<ProductDto>> GetFilteredAsync(ProductFilterDto filter);
    Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto updateProductDto);
    Task DeleteAsync(int id);
    Task<ProductDto> UploadImageAsync(int id, IFormFile image);
    Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category);
}