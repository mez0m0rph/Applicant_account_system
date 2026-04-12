using AdmissionService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdmissionService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Admission> Admissions { get; set; } = null!;
    public DbSet<AdmissionProgram> AdmissionPrograms { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admission>(entity =>
        {
            entity.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            entity.HasIndex(x => x.ApplicantUserId)
                .IsUnique();
        });

        modelBuilder.Entity<AdmissionProgram>(entity =>
        {
            entity.Property(x => x.Priority)
                .IsRequired();
        });
    }
}