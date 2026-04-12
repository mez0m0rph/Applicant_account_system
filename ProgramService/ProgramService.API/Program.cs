using Microsoft.EntityFrameworkCore;
using ProgramService.Data;
using ProgramService.Repositories;
using ProgramService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStudyProgramRepository, StudyProgramRepository>();
builder.Services.AddScoped<IStudyProgramService, StudyProgramService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) // миддлвейр часть
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();