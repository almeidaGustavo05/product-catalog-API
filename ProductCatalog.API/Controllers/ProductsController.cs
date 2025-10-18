using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Validators;

namespace ProductCatalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ImageUploadValidator _imageValidator;

    public ProductsController(IProductService productService, ImageUploadValidator imageValidator)
    {
        _productService = productService;
        _imageValidator = imageValidator;
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

    [HttpPatch("{id}/activate")]
    public async Task<ActionResult<ProductDto>> ActivateProduct(int id)
    {
        var product = await _productService.ActivateAsync(id);
        return Ok(product);
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<ActionResult<ProductDto>> DeactivateProduct(int id)
    {
        var product = await _productService.DeactivateAsync(id);
        return Ok(product);
    }

    [HttpPost("{id}/image")]
    public async Task<ActionResult<ProductDto>> UploadImage(int id, IFormFile image)
    {
        var validationResult = await _imageValidator.ValidateAsync(image);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new { message = e.ErrorMessage }));
        }

        using var stream = image.OpenReadStream();
        var product = await _productService.UploadImageAsync(id, stream, image.FileName, image.ContentType);
        
        return Ok(product);
    }
}