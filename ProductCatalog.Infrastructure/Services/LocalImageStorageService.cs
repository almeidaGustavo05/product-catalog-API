using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Infrastructure.Services;

public class LocalImageStorageService : IImageStorageService
{
    private readonly string _storagePath;
    private readonly string _baseUrl;

    public LocalImageStorageService(string storagePath = "wwwroot/images", string baseUrl = "/images")
    {
        _storagePath = storagePath;
        _baseUrl = baseUrl;
        
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType)
    {
        try
        {
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_storagePath, uniqueFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await imageStream.CopyToAsync(fileStream);

            return $"{_baseUrl}/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao fazer upload da imagem: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(imageUrl))
                return false;

            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_storagePath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Stream> GetImageAsync(string imageUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(imageUrl))
                throw new ArgumentException("URL da imagem não pode ser vazia");

            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_storagePath, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Imagem não encontrada");

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao recuperar imagem: {ex.Message}", ex);
        }
    }
}