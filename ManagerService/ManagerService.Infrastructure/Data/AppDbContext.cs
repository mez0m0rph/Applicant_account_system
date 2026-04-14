using Microsoft.EntityFrameworkCore;
using ManagerService.Domain.Entities;

namespace ManagerService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Manager> Managers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manager>(entity =>
        {
            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(x => x.Role)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(x => x.Faculty)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.HasIndex(x => x.UserId)
                .IsUnique();
        });
    }
}