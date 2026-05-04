using WebApp.Models.Common;
using WebApp.Models.Document;

namespace WebApp.Services;

public interface IDocumentApiService
{
    Task<ApiResult<string>> UploadAsync(UploadDocumentApiModel model);
    Task<ApiResult<List<DocumentViewModel>>> GetMyAsync();
}
