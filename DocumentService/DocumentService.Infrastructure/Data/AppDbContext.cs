using DocumentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Document> Documents { get; set; } = null!;
    public DbSet<StoredFile> StoredFiles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.Property(x => x.Type)
                .HasConversion<string>()
                .IsRequired();

            entity.HasIndex(x => x.ApplicantUserId);
        });

        modelBuilder.Entity<StoredFile>(entity =>
        {
            entity.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(x => x.ContentType)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(x => x.StoragePath)
                .IsRequired()
                .HasMaxLength(512);
        });
    }
}