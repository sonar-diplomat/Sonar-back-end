using System.Text;
using Application.Abstractions.Interfaces.Repository.Access;
using Application.Abstractions.Interfaces.Repository.Chat;
using Application.ExceptionHandling;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sonar.Infrastructure.Repository.Access;
using Sonar.Infrastructure.Repository.Chat;
using Sonar.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SonarContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SonarContext") ?? throw new InvalidOperationException("Connection string 'SonarContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<SonarContext>()
.AddDefaultTokenProviders();

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});



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

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
