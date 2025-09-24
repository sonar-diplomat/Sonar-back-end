using Application.Exception;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Middleware;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SonarContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SonarContext") ?? throw new InvalidOperationException("Connection string 'SonarContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


app.UseMiddleware<ExceptionMiddleware>(new ExceptionHandler());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
