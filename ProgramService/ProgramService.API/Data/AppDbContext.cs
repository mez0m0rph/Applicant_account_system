using Microsoft.EntityFrameworkCore;
using ProgramService.Models;

namespace ProgramService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<StudyProgram> StudyPrograms { get; set; } = null!;
}