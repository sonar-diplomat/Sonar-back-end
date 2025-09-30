using System.Text;
using Application.Abstractions.Interfaces.Exception;
using Application.Abstractions.Interfaces.Repository.Access;
using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Repository.Library;
using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Repository.Report;
using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Exception;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sonar.Infrastructure.Repository.Access;
using Sonar.Infrastructure.Repository.Chat;
using Sonar.Infrastructure.Repository.Client;
using Sonar.Infrastructure.Repository.Distribution;
using Sonar.Infrastructure.Repository.File;
using Sonar.Infrastructure.Repository.Library;
using Sonar.Infrastructure.Repository.Music;
using Sonar.Infrastructure.Repository.Report;
using Sonar.Infrastructure.Repository.UserCore;
using Sonar.Infrastructure.Repository.UserExperience;
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
builder.Services.AddScoped<IMessageReadRepository, MessageReadRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();

// Client Settings Repositories
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<INotificationTypeRepository, NotificationTypeRepository>();
builder.Services.AddScoped<IPlaybackQualityRepository, PlaybackQualityRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<IThemeRepository, ThemeRepository>();
builder.Services.AddScoped<IUserPrivacySettingsRepository, UserPrivacySettingsRepository>();

// Distribution Repositories
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<IDistributorRepository, DistributorRepository>();
builder.Services.AddScoped<IDistributorSessionRepository, DistributorSessionRepository>();
builder.Services.AddScoped<ILicenseRepository, LicenseRepository>();

// File Repositories
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFileTypeRepository, FileTypeRepository>();

// Library Repositories
builder.Services.AddScoped<IFolderRepository, FolderRepository>();
builder.Services.AddScoped<ILibraryRepository, LibraryRepository>();

// Music Repositories
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IBlendRepository, BlendRepository>();
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<ITrackRepository, TrackRepository>();

// Report Repositories
builder.Services.AddScoped<IReportableEntityTypeRepository, ReportableEntityTypeRepository>();
builder.Services.AddScoped<IReportReasonTypeRepository, ReportReasonTypeRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// User Repositories
builder.Services.AddScoped<IUserPrivacyGroupRepository, UserPrivacyGroupRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();
builder.Services.AddScoped<IUserStateRepository, UserStateRepository>();
builder.Services.AddScoped<IUserStatusRepository, UserStatusRepository>();

// UserExperience Repositories
builder.Services.AddScoped<IAchievementCategoryRepository, AchievementCategoryRepository>();
builder.Services.AddScoped<IAchievementProgressRepository, AchievementProgressRepository>();
builder.Services.AddScoped<IAchievementRepository, AchievementRepository>();
builder.Services.AddScoped<ICosmeticItemRepository, CosmeticItemRepository>();
builder.Services.AddScoped<ICosmeticItemTypeRepository, CosmeticItemTypeRepository>();
builder.Services.AddScoped<ICosmeticStickerRepository, CosmeticStickerRepository>();
builder.Services.AddScoped<IGiftRepository, GiftRepository>();
builder.Services.AddScoped<IGiftStyleRepository, GiftStyleRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<ISubscriptionFeatureRepository, SubscriptionFeatureRepository>();
builder.Services.AddScoped<ISubscriptionPackRepository, SubscriptionPackRepository>();
builder.Services.AddScoped<ISubscriptionPaymentRepository, SubscriptionPaymentRepository>();


// Exception Handling
builder.Services.AddSingleton<IAppExceptionFactory<IAppException>, AppExceptionFactory<IAppException>>();



var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();


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
