using Microsoft.EntityFrameworkCore;
using ManagerService.Domain.Entities;

namespace ManagerService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Manager> Managers { get; set; } = null!;
}