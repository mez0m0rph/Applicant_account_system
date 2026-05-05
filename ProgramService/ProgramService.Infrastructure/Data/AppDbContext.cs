using Microsoft.EntityFrameworkCore;
using ProgramService.Domain.Entities;

namespace ProgramService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<StudyProgram> StudyPrograms => Set<StudyProgram>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudyProgram>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ExternalId).IsRequired();
            entity.Property(x => x.Code).IsRequired();
            entity.Property(x => x.Title).IsRequired();
            entity.Property(x => x.Description).IsRequired();
            entity.Property(x => x.Faculty).IsRequired();
            entity.Property(x => x.EducationLevel).IsRequired();
            entity.Property(x => x.EducationForm).IsRequired();
            entity.Property(x => x.Language).IsRequired();
            entity.Property(x => x.Degree).IsRequired();

            entity.HasIndex(x => x.ExternalId).IsUnique();
        });
    }
}
