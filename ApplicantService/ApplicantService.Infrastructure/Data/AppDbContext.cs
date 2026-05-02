using ApplicantService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicantService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Applicant> Applicants { get; set; } = null!;
}