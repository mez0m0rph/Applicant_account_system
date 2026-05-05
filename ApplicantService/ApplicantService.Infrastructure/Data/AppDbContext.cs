using ApplicantService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicantService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Applicant> Applicants => Set<Applicant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Applicant>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName).IsRequired().HasMaxLength(256);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(256);
            entity.Property(x => x.Phone).IsRequired().HasMaxLength(64);
            entity.Property(x => x.Citizenship).IsRequired().HasMaxLength(128);

            entity.HasIndex(x => x.UserId).IsUnique();
        });
    }
}
