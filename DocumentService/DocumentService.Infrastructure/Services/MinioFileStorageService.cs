using System.Text;
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

    public async Task<string> UploadAsync(string fileName, string content)
    {
        var bucketExists = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_options.BucketName));

        if (!bucketExists)
        {
            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_options.BucketName));
        }

        var bytes = Encoding.UTF8.GetBytes(content);
        await using var stream = new MemoryStream(bytes);

        await _minioClient.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType("text/plain"));

        return $"{_options.BucketName}/{fileName}";
    }
}