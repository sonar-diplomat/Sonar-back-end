using System.Text;
using Application.Abstractions.Interfaces.Repository;
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
using Application.Abstractions.Interfaces.Services.File;
using Application.Abstractions.Interfaces.Services.Utilities;
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
using Application.Services.Utilities;
using Entities.Models.Music;
using Entities.Models.UserCore;
using FileSignatures;
using FileSignatures.Formats;
using Infrastructure.Data;
using Infrastructure.Repository.Access;
using Infrastructure.Repository.Chat;
using Infrastructure.Repository.ClientSettings;
using Infrastructure.Repository.Distribution;
using Infrastructure.Repository.File;
using Infrastructure.Repository.Library;
using Infrastructure.Repository.Music;
using Infrastructure.Repository.Report;
using Infrastructure.Repository.User;
using Infrastructure.Repository.UserExperience;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using QRCoder;
using Sonar.Controllers;
using Sonar.Hubs;
using Sonar.Infrastructure.Repository;
using Sonar.Infrastructure.Repository.Access;
using Sonar.Infrastructure.Repository.Chat;
using Sonar.Infrastructure.Repository.Client;
using Sonar.Infrastructure.Repository.Distribution;
using Sonar.Infrastructure.Repository.Music;
using Sonar.Infrastructure.Repository.Report;
using Sonar.Infrastructure.Repository.UserCore;
using Sonar.Infrastructure.Repository.UserExperience;
using Sonar.Middleware;
using Flac = Application.Services.File.Flac;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SonarContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SonarContext") ??
                      throw new InvalidOperationException("Connection string 'SonarContext' not found.")));

// Add services to the container.
builder.Services.AddControllers();
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
//        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
//    });

// CORS policy configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        // .AllowCredentials();
    });
    options.AddPolicy("ViteDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") // Vite dev origin
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); 
    });
});

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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                StringValues accessToken = ctx.Request.Query["access_token"];
                PathString path = ctx.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                    ctx.Token = accessToken;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddOpenApi();

builder.Services.AddSignalR();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});


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
builder.Services.AddScoped<IDistributorAccountRepository, DistributorAccountRepository>();
builder.Services.AddScoped<IArtistRegistrationRequestRepository, ArtistRegistrationRequestRepository>();

// File Repositories
builder.Services.AddScoped<IAudioFileRepository, AudioFileRepository>();
builder.Services.AddScoped<IImageFileRepository, ImageFileRepository>();
builder.Services.AddScoped<IVideoFileRepository, VideoFileRepository>();

// Library Repositories
builder.Services.AddScoped<IFolderRepository, FolderRepository>();
builder.Services.AddScoped<ILibraryRepository, LibraryRepository>();

// Music Repositories
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IBlendRepository, BlendRepository>();
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<ITrackRepository, TrackRepository>();
builder.Services.AddScoped<IAlbumArtistRepository, AlbumArtistRepository>();

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
builder.Services.AddScoped<IQueueRepository, QueueRepository>();

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

// Access Services
builder.Services.AddScoped<IAccessFeatureService, AccessFeatureService>();
builder.Services.AddScoped<ISuspensionService, SuspensionService>();
builder.Services.AddScoped<IVisibilityStateService, VisibilityStateService>();
builder.Services.AddScoped<IVisibilityStatusService, VisibilityStatusService>();

// Chat Services
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageReadService, MessageReadService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IPostService, PostService>();

// Client Settings Services
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<INotificationTypeService, NotificationTypeService>();
builder.Services.AddScoped<IPlaybackQualityService, PlaybackQualityService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<IUserPrivacySettingsService, UserPrivacySettingsService>();

// Distribution Services
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IDistributorService, DistributorService>();
builder.Services.AddScoped<IDistributorSessionService, DistributorSessionService>();
builder.Services.AddScoped<ILicenseService, LicenseService>();
builder.Services.AddScoped<IApiKeyGeneratorService, ApiKeyGeneratorService>();
builder.Services.AddScoped<IDistributorAccountService, DistributorAccountService>();

// File Services
builder.Services.AddScoped<IAudioFileService, AudioFileService>();
builder.Services.AddScoped<IImageFileService, ImageFileService>();
builder.Services.AddScoped<IVideoFileService, VideoFileService>();

// Library Services
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();

// Music Services
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IBlendService, BlendService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IAlbumArtistService, AlbumArtistService>();
builder.Services.AddScoped<ICollectionService<Album>, CollectionService<Album>>();
builder.Services.AddScoped<ICollectionService<Blend>, CollectionService<Blend>>();
builder.Services.AddScoped<ICollectionService<Playlist>, CollectionService<Playlist>>();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Report Services
builder.Services.AddScoped<IReportableEntityTypeService, ReportableEntityTypeService>();
builder.Services.AddScoped<IReportReasonTypeService, ReportReasonTypeService>();
builder.Services.AddScoped<IReportService, ReportService>();

// User Services
builder.Services.AddScoped<IUserPrivacyGroupService, UserPrivacyGroupService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<IUserStateService, UserStateService>();
builder.Services.AddScoped<IUserStatusService, UserStatusService>();
builder.Services.AddScoped<IQueueService, QueueService>();

// UserExperience Services
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

builder.Services.AddScoped<MailgunSettings>(_ =>
    new MailgunSettings
    {
        ApiKey = builder.Configuration["Mailgun:ApiKey"] ??
                 throw new InvalidOperationException("Mailgun ApiKey not found."),
        Domain = builder.Configuration["Mailgun:Domain"] ??
                 throw new InvalidOperationException("Mailgun Domain not found."),
        From = builder.Configuration["Mailgun:From"] ?? throw new InvalidOperationException("Mailgun From not found.")
    }
);

builder.Services.AddSingleton<IChatNotifier, ChatNotifier>();


// Utility Services
builder.Services.AddScoped<IEmailSenderService, MailgunEmailService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<QRCodeGenerator>();
builder.Services.AddSingleton<IShareService, ShareService>();
builder.Services.AddSingleton<IFileFormatInspector>(new FileFormatInspector(
    [new Png(), new Jpeg(), new Mp3(), new Flac(), new Gif()]));
builder.Services.AddSingleton<IFileStorageService, FileStorageService>();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

#endregion

WebApplication app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("ViteDev");

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<ChatHub>("/hubs/chat");
app.MapControllers();


app.Run();