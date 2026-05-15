using AdmissionService.Application.DTOs.External;

namespace AdmissionService.Application.Interfaces;

public interface IProgramCatalogClient
{
    Task<ProgramDetailsDto?> GetByIdAsync(Guid programId);
}
