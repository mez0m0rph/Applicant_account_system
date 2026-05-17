using AdmissionService.Application.DTOs.External;

namespace AdmissionService.Application.Interfaces;

public interface IManagerCatalogClient
{
    Task<ManagerCatalogItemDto?> GetByUserIdAsync(Guid userId);
}
