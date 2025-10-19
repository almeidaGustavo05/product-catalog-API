using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        IImageStorageService imageStorageService,
        IMapper mapper,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _imageStorageService = imageStorageService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        _logger.LogInformation("Buscando produto com ID: {ProductId}", id);
        
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogInformation("Produto com ID {ProductId} não encontrado", id);
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
        }

        _logger.LogInformation("Produto com ID {ProductId} encontrado: {ProductName}", id, product.Name);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        _logger.LogInformation("Buscando todos os produtos");
        
        var products = await _productRepository.GetAllAsync();
        
        _logger.LogInformation("Encontrados {ProductCount} produtos", products.Count());
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetFilteredAsync(ProductFilterDto filter)
    {
        _logger.LogInformation("Buscando produtos com filtros - Categoria: {Category}, PreçoMin: {MinPrice}, PreçoMax: {MaxPrice}, Status: {Status}", 
            filter.Category, filter.MinPrice, filter.MaxPrice, filter.Status);
        
        var products = await _productRepository.GetFilteredAsync(
            filter.Category,
            filter.MinPrice,
            filter.MaxPrice,
            filter.Status);

        _logger.LogInformation("Encontrados {ProductCount} produtos com os filtros aplicados", products.Count());
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
    {
        _logger.LogInformation("Criando novo produto: {ProductName}", createProductDto.Name);
        
        try
        {
            var product = new Product(
                createProductDto.Name,
                createProductDto.Description,
                createProductDto.Price,
                createProductDto.Category);

            var createdProduct = await _productRepository.AddAsync(product);
            
            _logger.LogInformation("Produto criado com sucesso - ID: {ProductId}, Nome: {ProductName}", 
                createdProduct.Id, createdProduct.Name);
            
            return _mapper.Map<ProductDto>(createdProduct);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Erro ao criar produto: {ProductName}", createProductDto.Name);
            throw;
        }
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto updateProductDto)
    {
        _logger.LogInformation("Atualizando produto com ID: {ProductId}", id);
        
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogInformation("Tentativa de atualizar produto inexistente - ID: {ProductId}", id);
                throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
            }

            product.UpdateProduct(
                updateProductDto.Name,
                updateProductDto.Description,
                updateProductDto.Price,
                updateProductDto.Category,
                updateProductDto.Status);

            await _productRepository.UpdateAsync(product);
            
            _logger.LogInformation("Produto atualizado com sucesso - ID: {ProductId}, Nome: {ProductName}", 
                id, product.Name);
            
            return _mapper.Map<ProductDto>(product);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Erro ao atualizar produto com ID: {ProductId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deletando produto com ID: {ProductId}", id);
        
        try
        {
            var exists = await _productRepository.ExistsAsync(id);
            if (!exists)
            {
                _logger.LogInformation("Tentativa de deletar produto inexistente - ID: {ProductId}", id);
                throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
            }

            var product = await _productRepository.GetByIdAsync(id);
            if (product?.ImageUrl != null)
            {
                _logger.LogInformation("Deletando imagem do produto: {ImageUrl}", product.ImageUrl);
                await _imageStorageService.DeleteImageAsync(product.ImageUrl);
            }

            await _productRepository.DeleteAsync(id);
            
            _logger.LogInformation("Produto deletado com sucesso - ID: {ProductId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Erro ao deletar produto com ID: {ProductId}", id);
            throw;
        }
    }

    public async Task<ProductDto> UploadImageAsync(int id, IFormFile image)
    {
        _logger.LogInformation("Fazendo upload de imagem para produto ID: {ProductId}, Arquivo: {FileName}", 
            id, image.FileName);
        
        try
        {
            var imageValidator = new Validators.ImageUploadValidator();
            var validationResult = await imageValidator.ValidateAsync(image);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                _logger.LogInformation("Imagem inválida para produto ID {ProductId}: {ValidationErrors}", id, errors);
                throw new ArgumentException($"Imagem inválida: {errors}");
            }

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogInformation("Tentativa de upload de imagem para produto inexistente - ID: {ProductId}", id);
                throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
            }

            if (product.ImageUrl != null)
            {
                _logger.LogInformation("Deletando imagem anterior do produto: {ImageUrl}", product.ImageUrl);
                await _imageStorageService.DeleteImageAsync(product.ImageUrl);
            }

            using var stream = image.OpenReadStream();
            var imageUrl = await _imageStorageService.UploadImageAsync(stream, image.FileName, image.ContentType);
            product.SetImageUrl(imageUrl);

            await _productRepository.UpdateAsync(product);
            
            _logger.LogInformation("Upload de imagem concluído com sucesso para produto ID: {ProductId}, Nova URL: {ImageUrl}", 
                id, imageUrl);
            
            return _mapper.Map<ProductDto>(product);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Erro ao fazer upload de imagem para produto ID: {ProductId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category)
    {
        _logger.LogInformation("Buscando produtos da categoria: {Category}", category);
        
        var products = await _productRepository.GetFilteredAsync(category, null, null, null);
        
        _logger.LogInformation("Encontrados {ProductCount} produtos na categoria {Category}", products.Count(), category);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm)
    {
        _logger.LogInformation("Realizando busca por termo: {SearchTerm}", searchTerm);
        
        var products = await _productRepository.GetAllAsync();
        var filteredProducts = products.Where(p => 
            p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            p.Category.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        
        _logger.LogInformation("Busca por '{SearchTerm}' retornou {ProductCount} produtos", searchTerm, filteredProducts.Count());
        return _mapper.Map<IEnumerable<ProductDto>>(filteredProducts);
    }
}