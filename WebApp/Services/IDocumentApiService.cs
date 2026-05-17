using WebApp.Models.Common;
using WebApp.Models.Document;

namespace WebApp.Services;

public interface IDocumentApiService
{
    Task<ApiResult<string>> UploadAsync(UploadDocumentApiModel model);
    Task<ApiResult<List<DocumentViewModel>>> GetMyAsync();
    Task<ApiResult<DownloadedFileViewModel>> DownloadAsync(Guid documentId);
    Task<ApiResult<string>> DeleteAsync(Guid documentId);
    Task<ApiResult<string>> UpdateAsync(Guid documentId, UpdateDocumentApiModel model);
    Task<ApiResult<string>> ReplaceFileAsync(Guid documentId, ReplaceDocumentFileApiModel model);
}
