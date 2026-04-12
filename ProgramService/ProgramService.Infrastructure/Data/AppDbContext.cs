using Microsoft.EntityFrameworkCore;
using ProgramService.Domain.Entities;

namespace ProgramService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<StudyProgram> StudyPrograms { get; set; } = null!;
}