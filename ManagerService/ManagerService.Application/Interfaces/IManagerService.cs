using ManagerService.Application.DTOs;

namespace ManagerService.Application.Interfaces;

public interface IManagerService
{
    Task<List<ManagerResponse>> GetAllAsync();
    Task<ManagerResponse?> GetByIdAsync(Guid id);
    Task CreateAsync(CreateManagerRequest request);
}