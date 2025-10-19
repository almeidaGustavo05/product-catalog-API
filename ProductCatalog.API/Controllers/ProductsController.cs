using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Pagination;

namespace ProductCatalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
        [FromQuery] string? category = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] Domain.Enums.ProductStatus? status = null)
    {
        var filter = new ProductFilterDto
        {
            Category = category,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Status = status
        };

        var products = await _productService.GetFilteredAsync(filter);
        return Ok(products);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PageList<ProductDto>>> GetProductsPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var pageParams = new PageParams
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var pagedProducts = await _productService.GetPagedAsync(pageParams);
        return Ok(pagedProducts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        var product = await _productService.CreateAsync(createProductDto);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
    {
        var product = await _productService.UpdateAsync(id, updateProductDto);
        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/image")]
    public async Task<ActionResult<ProductDto>> UploadImage(int id, IFormFile image)
    {
        var product = await _productService.UploadImageAsync(id, image);
        return Ok(product);
    }
}