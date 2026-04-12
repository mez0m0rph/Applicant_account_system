using ManagerService.Domain.Entities;

namespace ManagerService.Application.Interfaces;

public interface IManagerRepository
{
    Task<List<Manager>> GetAllAsync();
    Task<Manager?> GetManagerByIdAsync(Guid id);
    Task CreateManagerAsync(Manager manager);
}