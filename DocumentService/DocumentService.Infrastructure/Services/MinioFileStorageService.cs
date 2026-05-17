using DocumentService.Application.Interfaces;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace DocumentService.Infrastructure.Services;

public class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;

    public MinioFileStorageService(IOptions<MinioOptions> options)
    {
        _options = options.Value;
        _minioClient = new MinioClient()
            .WithEndpoint(_options.Endpoint)
            .WithCredentials(_options.AccessKey, _options.SecretKey)
            .WithSSL(_options.UseSsl)
            .Build();
    }

    public async Task<string> UploadAsync(string fileName, string contentType, byte[] content)
    {
        var bucketExists = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_options.BucketName));

        if (!bucketExists)
        {
            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_options.BucketName));
        }

        await using var stream = new MemoryStream(content);

        await _minioClient.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType));

        return $"{_options.BucketName}/{fileName}";
    }

    public async Task<byte[]> DownloadAsync(string storagePath)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
            throw new Exception("Путь к файлу не указан");

        var parts = storagePath.Split('/', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            throw new Exception("Некорректный путь к файлу");

        var bucketName = parts[0];
        var objectName = parts[1];

        await using var output = new MemoryStream();

        await _minioClient.GetObjectAsync(
            new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => stream.CopyTo(output)));

        return output.ToArray();
    }

    public async Task DeleteAsync(string storagePath)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
            throw new Exception("Путь к файлу не указан");

        var parts = storagePath.Split('/', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            throw new Exception("Некорректный путь к файлу");

        var bucketName = parts[0];
        var objectName = parts[1];

        await _minioClient.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName));
    }
}
