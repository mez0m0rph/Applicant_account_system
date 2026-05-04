namespace DocumentService.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(string fileName, string content);
}