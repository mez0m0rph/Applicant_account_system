using ManagerService.Domain.Entities;

namespace ManagerService.Application.Interfaces;

public interface IManagerRepository
{
    Task<List<Manager>> GetAllAsync();
    Task<Manager?> GetByIdAsync(Guid id);
    Task CreateAsync(Manager manager);
}