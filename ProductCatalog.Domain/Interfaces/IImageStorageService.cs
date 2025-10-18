namespace ProductCatalog.Domain.Interfaces;

public interface IImageStorageService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType);
    Task<bool> DeleteImageAsync(string imageUrl);
    Task<Stream> GetImageAsync(string imageUrl);
}