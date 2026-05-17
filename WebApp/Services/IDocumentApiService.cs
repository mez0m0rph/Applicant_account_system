using WebApp.Models.Common;
using WebApp.Models.Document;

namespace WebApp.Services;

public interface IDocumentApiService
{
    Task<ApiResult<string>> UploadAsync(UploadDocumentApiModel model);
    Task<ApiResult<string>> UploadForApplicantAsync(Guid applicantUserId, UploadDocumentApiModel model);

    Task<ApiResult<List<DocumentViewModel>>> GetMyAsync();
    Task<ApiResult<List<DocumentViewModel>>> GetByApplicantUserIdAsync(Guid applicantUserId);

    Task<ApiResult<DownloadedFileViewModel>> DownloadAsync(Guid documentId);
    Task<ApiResult<DownloadedFileViewModel>> DownloadForApplicantAsync(Guid applicantUserId, Guid documentId);

    Task<ApiResult<string>> DeleteAsync(Guid documentId);
    Task<ApiResult<string>> DeleteForApplicantAsync(Guid applicantUserId, Guid documentId);

    Task<ApiResult<string>> UpdateAsync(Guid documentId, UpdateDocumentApiModel model);
    Task<ApiResult<string>> UpdateForApplicantAsync(Guid applicantUserId, Guid documentId, UpdateDocumentApiModel model);

    Task<ApiResult<string>> ReplaceFileAsync(Guid documentId, ReplaceDocumentFileApiModel model);
    Task<ApiResult<string>> ReplaceFileForApplicantAsync(Guid applicantUserId, Guid documentId, ReplaceDocumentFileApiModel model);
}
