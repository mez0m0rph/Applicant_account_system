using ManagerService.Application.DTOs;

namespace ManagerService.Application.Interfaces;

public interface IManagerService
{
    Task<List<ManagerResponse>> GetAllAsync();
    Task<ManagerResponse?> GetByIdAsync(Guid id);
    Task<ManagerResponse?> GetByUserIdAsync(Guid userId);
    Task<Guid> CreateAsync(CreateManagerRequest request);
    Task UpdateAsync(Guid id, UpdateManagerRequest request);
    Task DeleteAsync(Guid id);
}
