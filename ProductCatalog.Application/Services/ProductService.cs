using AutoMapper;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepository productRepository,
        IImageStorageService imageStorageService,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _imageStorageService = imageStorageService;
        _mapper = mapper;
    }

    public async Task<ProductDto> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetFilteredAsync(ProductFilterDto filter)
    {
        var products = await _productRepository.GetFilteredAsync(
            filter.Category,
            filter.MinPrice,
            filter.MaxPrice,
            filter.Status);

        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
    {
        var product = new Product(
            createProductDto.Name,
            createProductDto.Description,
            createProductDto.Price,
            createProductDto.Category);

        var createdProduct = await _productRepository.AddAsync(product);
        return _mapper.Map<ProductDto>(createdProduct);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto updateProductDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");

        product.UpdateProduct(
            updateProductDto.Name,
            updateProductDto.Description,
            updateProductDto.Price,
            updateProductDto.Category);

        await _productRepository.UpdateAsync(product);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task DeleteAsync(Guid id)
    {
        var exists = await _productRepository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");

        var product = await _productRepository.GetByIdAsync(id);
        if (product?.ImageUrl != null)
        {
            await _imageStorageService.DeleteImageAsync(product.ImageUrl);
        }

        await _productRepository.DeleteAsync(id);
    }

    public async Task<ProductDto> ActivateAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");

        product.Activate();
        await _productRepository.UpdateAsync(product);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> DeactivateAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");

        product.Deactivate();
        await _productRepository.UpdateAsync(product);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UploadImageAsync(Guid id, Stream imageStream, string fileName, string contentType)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");

        if (product.ImageUrl != null)
        {
            await _imageStorageService.DeleteImageAsync(product.ImageUrl);
        }

        var imageUrl = await _imageStorageService.UploadImageAsync(imageStream, fileName, contentType);
        product.SetImageUrl(imageUrl);

        await _productRepository.UpdateAsync(product);
        return _mapper.Map<ProductDto>(product);
    }
}