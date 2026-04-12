using Microsoft.EntityFrameworkCore;
using ProgramService.Application.Interfaces;
using ProgramService.Infrastructure.Data;
using ProgramService.Infrastructure.Repositories;
using ProgramService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStudyProgramRepository, StudyProgramRepository>();
builder.Services.AddScoped<IStudyProgramService, StudyProgramService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
