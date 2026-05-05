using ManagerService.Application.DTOs;
using ManagerService.Application.Interfaces;
using ManagerService.Domain.Entities;

namespace ManagerService.Infrastructure.Services;

public class ManagerServiceImpl : IManagerService
{
    private readonly IManagerRepository _repository;

    public ManagerServiceImpl(IManagerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ManagerResponse>> GetAllAsync()
    {
        var managers = await _repository.GetAllAsync();
        return managers.Select(Map).ToList();
    }

    public async Task<ManagerResponse?> GetByIdAsync(Guid id)
    {
        var manager = await _repository.GetByIdAsync(id);
        return manager == null ? null : Map(manager);
    }

    public async Task<Guid> CreateAsync(CreateManagerRequest request)
    {
        var existing = await _repository.GetByUserIdAsync(request.UserId);
        if (existing != null)
            throw new Exception("Менеджер для этого userId уже существует");

        var manager = new Manager
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            FullName = request.FullName,
            Email = request.Email,
            Role = request.Role,
            Faculty = request.Faculty,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(manager);
        return manager.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateManagerRequest request)
    {
        var manager = await _repository.GetByIdAsync(id);
        if (manager == null)
            throw new Exception("Менеджер не найден");

        manager.FullName = request.FullName;
        manager.Email = request.Email;
        manager.Role = request.Role;
        manager.Faculty = request.Faculty;

        await _repository.UpdateAsync(manager);
    }

    public async Task DeleteAsync(Guid id)
    {
        var manager = await _repository.GetByIdAsync(id);
        if (manager == null)
            throw new Exception("Менеджер не найден");

        await _repository.DeleteAsync(manager);
    }

    private static ManagerResponse Map(Manager manager) => new()
    {
        Id = manager.Id,
        UserId = manager.UserId,
        FullName = manager.FullName,
        Email = manager.Email,
        Role = manager.Role,
        Faculty = manager.Faculty,
        CreatedAt = manager.CreatedAt
    };
}
