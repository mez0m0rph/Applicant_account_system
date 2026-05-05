using ManagerService.Domain.Entities;

namespace ManagerService.Application.Interfaces;

public interface IManagerRepository
{
    Task<List<Manager>> GetAllAsync();
    Task<Manager?> GetByIdAsync(Guid id);
    Task<Manager?> GetByUserIdAsync(Guid userId);
    Task CreateAsync(Manager manager);
    Task UpdateAsync(Manager manager);
    Task DeleteAsync(Manager manager);
}
