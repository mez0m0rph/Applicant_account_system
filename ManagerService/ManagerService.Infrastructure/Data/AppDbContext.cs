using ManagerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagerService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Manager> Managers => Set<Manager>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName).IsRequired().HasMaxLength(256);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(256);
            entity.Property(x => x.Role).IsRequired().HasMaxLength(64);
            entity.Property(x => x.Faculty).IsRequired().HasMaxLength(256);

            entity.HasIndex(x => x.UserId).IsUnique();
        });
    }
}
