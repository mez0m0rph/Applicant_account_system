namespace DocumentService.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(string fileName, string contentType, byte[] content);
    Task<byte[]> DownloadAsync(string storagePath);
    Task DeleteAsync(string storagePath);
}
