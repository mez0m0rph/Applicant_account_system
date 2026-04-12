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
}