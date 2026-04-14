using ManagerService.Application.Interfaces;
using ManagerService.Domain.Entities;
using ManagerService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ManagerService.Infrastructure.Repositories;

public class ManagerRepository : IManagerRepository
{
    private readonly AppDbContext _context;
    public ManagerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Manager>> GetAllAsync()
    {
        return await _context.Managers.ToListAsync();
    }

    public async Task<Manager?> GetByIdAsync(Guid id)
    {
        return await _context.Managers.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task CreateAsync(Manager manager)
    {
        await _context.Managers.AddAsync(manager);
        await _context.SaveChangesAsync();
    }
}