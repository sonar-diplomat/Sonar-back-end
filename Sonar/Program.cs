using Application.Abstractions.Interfaces.Repository;
using Application.Abstractions.Interfaces.Repository.Access;
using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Exception;
using Entities.Models;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;
using Sonar.Infrastructure.Repository.Access;
using Sonar.Infrastructure.Repository.Chat;
using Sonar.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SonarContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SonarContext") ?? throw new InvalidOperationException("Connection string 'SonarContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Access Repositories
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<ISuspensionRepository, SuspensionRepository>();
builder.Services.AddScoped<IVisibilityStateRepository, VisibilityStateRepository>();
builder.Services.AddScoped<IVisibilityStatusRepository, VisibilityStatusRepository>();

// Chat Repositories
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();

// Client Settings Repositories
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();

// Distribution Repositories
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();

// File Repositories
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();

// Library Repositories
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();

// Music Repositories
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();

// Report Repositories
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();

// User Repositories
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();

// UserExperience Repositories
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();
builder.Services.AddScoped<IAccessFeatureRepository, AccessFeatureRepository>();

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
