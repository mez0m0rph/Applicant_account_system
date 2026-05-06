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

            entity.HasIndex(x => new { x.AdmissionId, x.ProgramId }).IsUnique();
        });
    }
}
