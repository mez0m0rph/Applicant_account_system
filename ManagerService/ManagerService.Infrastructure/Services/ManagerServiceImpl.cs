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

        return managers.Select(m => new ManagerResponse
        {
            Id = m.Id,
            UserId = m.UserId,
            Email = m.Email,
            FullName = m.FullName,
            Role = m.Role.ToString(),
            Faculty = m.Faculty,
            CreatedAt = m.CreatedAt
        }).ToList();
    }

    public async Task<ManagerResponse?> GetByIdAsync(Guid id)
    {
        var manager = await _repository.GetByIdAsync(id);

        if (manager == null)
            throw new Exception("Такого мененджера не существует");

        return new ManagerResponse
        {
            Id = manager.Id,
            UserId = manager.UserId,
            Email = manager.Email,
            FullName = manager.FullName,
            Role = manager.Role.ToString(),
            Faculty = manager.Faculty,
            CreatedAt = manager.CreatedAt
        };
    }

    public async Task CreateAsync(CreateManagerRequest request)
    {
        var manager = new Manager
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Email = request.Email,
            FullName = request.FullName,
            Role = request.Role,
            Faculty = request.Faculty,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(manager);
    }
}
