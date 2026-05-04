namespace DocumentService.Infrastructure.Services;

public class MinioOptions
{
    public string Endpoint { get; set; } = "localhost:9000";
    public string AccessKey { get; set; } = "minioadmin";
    public string SecretKey { get; set; } = "minioadmin";
    public string BucketName { get; set; } = "documents";
    public bool UseSsl { get; set; } = false;
}