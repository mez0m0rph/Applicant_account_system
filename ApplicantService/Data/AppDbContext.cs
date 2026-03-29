using Microsoft.EntityFrameworkCore;
using ApplicantService.Models;

namespace ApplicantService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
        
    }

    public DbSet<Applicant> Applicants { get; set; } = null!;
}