using AdmissionService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdmissionService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Admission> Admissions => Set<Admission>();
    public DbSet<AdmissionProgram> AdmissionPrograms => Set<AdmissionProgram>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admission>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ApplicantEmail)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);
        });

        modelBuilder.Entity<AdmissionProgram>(entity =>
        {
            entity.HasKey(x => x.Id);
        });
    }
}
