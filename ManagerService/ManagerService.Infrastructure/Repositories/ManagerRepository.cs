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
        return await _context.Managers.OrderBy(x => x.FullName).ToListAsync();
    }

    public async Task<Manager?> GetByIdAsync(Guid id)
    {
        return await _context.Managers.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Manager?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Managers.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task CreateAsync(Manager manager)
    {
        await _context.Managers.AddAsync(manager);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Manager manager)
    {
        _context.Managers.Update(manager);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Manager manager)
    {
        _context.Managers.Remove(manager);
        await _context.SaveChangesAsync();
    }
}
