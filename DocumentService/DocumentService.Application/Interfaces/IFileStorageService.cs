namespace DocumentService.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(string fileName, string contentType, byte[] content);
}
