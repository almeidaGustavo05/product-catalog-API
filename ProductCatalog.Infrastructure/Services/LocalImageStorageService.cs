using Microsoft.Extensions.Logging;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Infrastructure.Services;

public class LocalImageStorageService : IImageStorageService
{
    private readonly string _storagePath;
    private readonly string _baseUrl;
    private readonly ILogger<LocalImageStorageService> _logger;

    public LocalImageStorageService(ILogger<LocalImageStorageService> logger, string storagePath = "wwwroot/images", string baseUrl = "/images")
    {
        _logger = logger;
        _storagePath = storagePath;
        _baseUrl = baseUrl;
        
        if (!Directory.Exists(_storagePath))
        {
            _logger.LogInformation("Criando diretório de armazenamento de imagens: {StoragePath}", _storagePath);
            Directory.CreateDirectory(_storagePath);
        }
        
        _logger.LogInformation("LocalImageStorageService inicializado - StoragePath: {StoragePath}, BaseUrl: {BaseUrl}", 
            _storagePath, _baseUrl);
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType)
    {
        _logger.LogInformation("Iniciando upload de imagem: {FileName}, ContentType: {ContentType}", fileName, contentType);
        
        try
        {
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_storagePath, uniqueFileName);

            _logger.LogInformation("Salvando imagem no caminho: {FilePath}", filePath);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await imageStream.CopyToAsync(fileStream);

            var imageUrl = $"{_baseUrl}/{uniqueFileName}";
            
            _logger.LogInformation("Upload de imagem concluído com sucesso - URL: {ImageUrl}", imageUrl);
            
            return imageUrl;
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Erro ao fazer upload da imagem: {FileName}", fileName);
            throw new InvalidOperationException($"Erro ao fazer upload da imagem: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        _logger.LogInformation("Deletando imagem: {ImageUrl}", imageUrl);
        
        try
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                _logger.LogInformation("Tentativa de deletar imagem com URL vazia ou nula");
                return false;
            }

            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_storagePath, fileName);

            _logger.LogInformation("Verificando existência do arquivo: {FilePath}", filePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Imagem deletada com sucesso: {ImageUrl}", imageUrl);
                return true;
            }
            else
            {
                _logger.LogInformation("Arquivo de imagem não encontrado para deletar: {ImageUrl}", imageUrl);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Erro ao deletar imagem: {ImageUrl}", imageUrl);
            return false;
        }
    }

    public async Task<Stream> GetImageAsync(string imageUrl)
    {
        _logger.LogInformation("Recuperando imagem: {ImageUrl}", imageUrl);
        
        try
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                _logger.LogInformation("Tentativa de recuperar imagem com URL vazia ou nula");
                throw new ArgumentException("URL da imagem não pode ser vazia ou nula.");
            }

            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_storagePath, fileName);

            _logger.LogInformation("Verificando existência do arquivo: {FilePath}", filePath);

            if (!File.Exists(filePath))
            {
                _logger.LogInformation("Arquivo de imagem não encontrado: {ImageUrl}", imageUrl);
                throw new FileNotFoundException($"Imagem não encontrada: {imageUrl}");
            }

            _logger.LogInformation("Retornando stream da imagem: {ImageUrl}", imageUrl);
            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Erro ao recuperar imagem: {ImageUrl}", imageUrl);
            throw new InvalidOperationException($"Erro ao recuperar imagem: {ex.Message}", ex);
        }
    }
}