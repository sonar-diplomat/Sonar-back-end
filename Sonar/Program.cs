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
using Application.Abstractions.Interfaces.Services;
using Application.Exception;
using Application.Services.Access;
using Application.Services.Chat;
using Application.Services.ClientSettings;
using Application.Services.Distribution;
using Application.Services.File;
using Application.Services.Library;
using Application.Services.Music;
using Application.Services.Report;
using Application.Services.UserCore;
using Application.Services.UserExperience;
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
using System.Text;

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

#region RegisterRepositories
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
#endregion
#region RegisterServices
// Access Repositories
builder.Services.AddScoped<IAccessFeatureService, AccessFeatureService>();
builder.Services.AddScoped<ISuspensionService, SuspensionService>();
builder.Services.AddScoped<IVisibilityStateService, VisibilityStateService>();
builder.Services.AddScoped<IVisibilityStatusService, VisibilityStatusService>();

// Chat Repositories
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageReadService, MessageReadService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IPostService, PostService>();

// Client Settings Repositories
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<INotificationTypeService, NotificationTypeService>();
builder.Services.AddScoped<IPlaybackQualityService, PlaybackQualityService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<IUserPrivacySettingsService, UserPrivacySettingsService>();

// Distribution Repositories
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IDistributorService, DistributorService>();
builder.Services.AddScoped<IDistributorSessionService, DistributorSessionService>();
builder.Services.AddScoped<ILicenseService, LicenseService>();

// File Repositories
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IFileTypeService, FileTypeService>();

// Library Repositories
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();

// Music Repositories
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IBlendService, BlendService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<ITrackService, TrackService>();

// Report Repositories
builder.Services.AddScoped<IReportableEntityTypeService, ReportableEntityTypeService>();
builder.Services.AddScoped<IReportReasonTypeService, ReportReasonTypeService>();
builder.Services.AddScoped<IReportService, ReportService>();

// User Repositories
builder.Services.AddScoped<IUserPrivacyGroupService, UserPrivacyGroupService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<IUserStateService, UserStateService>();
builder.Services.AddScoped<IUserStatusService, UserStatusService>();

// UserExperience Repositories
builder.Services.AddScoped<IAchievementCategoryService, AchievementCategoryService>();
builder.Services.AddScoped<IAchievementProgressService, AchievementProgressService>();
builder.Services.AddScoped<IAchievementService, AchievementService>();
builder.Services.AddScoped<ICosmeticItemService, CosmeticItemService>();
builder.Services.AddScoped<ICosmeticItemTypeService, CosmeticItemTypeService>();
builder.Services.AddScoped<ICosmeticStickerService, CosmeticStickerService>();
builder.Services.AddScoped<IGiftService, GiftService>();
builder.Services.AddScoped<IGiftStyleService, GiftStyleService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ISubscriptionFeatureService, SubscriptionFeatureService>();
builder.Services.AddScoped<ISubscriptionPackService, SubscriptionPackService>();
builder.Services.AddScoped<ISubscriptionPaymentService, SubscriptionPaymentService>();
#endregion
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
