using AdmissionService.Application.DTOs.External;

namespace AdmissionService.Application.Interfaces;

public interface IDocumentCatalogClient
{
    Task<List<DocumentDetailsDto>> GetByApplicantUserIdAsync(Guid applicantUserId);
}
