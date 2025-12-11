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
using Application.Abstractions.Interfaces.Services.UserCore;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.Services.Access;
using Application.Services.Chat;
using Application.Services.ClientSettings;
using Application.Services.Distribution;
using Application.Services.File;
using Application.Services.Library;
using Application.Services.Music;
using Application.Services.Report;
using Application.Services.Search;
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
using Infrastructure.Repository.UserCore;
using Infrastructure.Repository.UserExperience;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Logging;
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
using System.Text;
using System.Text.Json;
using Application.Abstractions.Interfaces.Services.Chat;
using Sonar.HealthChecks;
using Flac = Application.Services.File.Flac;
using Microsoft.Extensions.DependencyInjection;
using Application.Response;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Initialize custom logger
Logger.Initialize(builder.Configuration);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}

// Register custom EF Core logger factory as singleton
builder.Services.AddSingleton<ILoggerFactory>(_ => 
    LoggerFactory.Create(builder => builder.AddProvider(new EfCoreLoggerProvider())));

builder.Services.AddDbContext<SonarContext>((serviceProvider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("SonarContext") ??
                      throw new InvalidOperationException("Connection string 'SonarContext' not found."));
    
    // Enable EF Core logging using custom logger factory
    ILoggerFactory efCoreLoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
    options.UseLoggerFactory(efCoreLoggerFactory);
    
    // Enable sensitive data logging and detailed errors in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Add Health Checks
builder.Services.AddScoped<SonarContextHealthCheck>();
builder.Services.AddHealthChecks()
    .AddCheck<SonarContextHealthCheck>(
        name: "database",
        tags: new[] { "db", "sql", "postgresql" });
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
    options.ValueLengthLimit = 104857600; // 100 MB - allows individual form values up to 100 MB
    options.KeyLengthLimit = 1024;
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100L * 1024 * 1024; // 100 MB
});


// Add services to the container.
builder.Services.AddControllers(options =>
    {
        // Configure form model binding limits
        options.MaxModelBindingCollectionSize = int.MaxValue;
    })
    .ConfigureApiBehaviorOptions(_ => { })
    .AddJsonOptions(options =>
    {
        // Increase MaxDepth to allow OpenAPI schema generation to complete
        // The NavigationPropertyIgnoreTransformer will prevent actual circular references
        options.JsonSerializerOptions.MaxDepth = 64;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

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
            },
            OnChallenge = context =>
            {
                // Проверяем, требует ли endpoint аутентификации
                // Если endpoint имеет [AllowAnonymous], не обрабатываем challenge
                var endpoint = context.HttpContext.GetEndpoint();
                if (endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null)
                {
                    // Позволяем стандартному поведению работать для анонимных endpoints
                    return Task.CompletedTask;
                }
                
                // Предотвращаем стандартный ответ 401 от JWT Bearer
                // HandleResponse() останавливает дальнейшую обработку запроса
                context.HandleResponse();
                
                // Проверяем, что ответ еще не начат
                if (context.Response.HasStarted)
                {
                    return Task.CompletedTask;
                }
                
                // Устанавливаем правильный формат ответа
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                
                var unauthorizedResponse = new UnauthorizedResponse();
                var serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new ResponseJsonConverter() }
                };
                
                string json = JsonSerializer.Serialize(unauthorizedResponse, serializerOptions);
                return context.Response.WriteAsync(json);
            }
        };
    });

builder.Services.AddOpenApi();

builder.Services.AddSignalR();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

// Add Swagger UI for OpenAPI visualization
builder.Services.AddSwaggerGen();

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
builder.Services.AddScoped<IChatStickerRepository, ChatStickerRepository>();

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
builder.Services.AddScoped<IFileRepository, FileRepository>();
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
builder.Services.AddScoped<ITrackArtistRepository, TrackArtistRepository>();
builder.Services.AddScoped<ITrackMoodTagRepository, TrackMoodTagRepository>();
builder.Services.AddScoped<IAlbumMoodTagRepository, AlbumMoodTagRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IMoodTagRepository, MoodTagRepository>();

// Report Repositories
builder.Services.AddScoped<IReportableEntityTypeRepository, ReportableEntityTypeRepository>();
builder.Services.AddScoped<IReportReasonTypeRepository, ReportReasonTypeRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// User Repositories
builder.Services.AddScoped<IUserPrivacyGroupRepository, UserPrivacyGroupRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserFollowRepository, UserFollowRepository>();
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
builder.Services.AddScoped<IChatStickerService, ChatStickerService>();

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
builder.Services.AddScoped<IFolderCollectionService, FolderCollectionService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();

// Music Services
builder.Services.AddScoped<ITrackAlbumService, TrackAlbumService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IBlendService, BlendService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IAlbumArtistService, AlbumArtistService>();
builder.Services.AddScoped<ITrackArtistService, TrackArtistService>();
builder.Services.AddScoped<ICollectionService<Album>, CollectionService<Album>>();
builder.Services.AddScoped<ICollectionService<Blend>, CollectionService<Blend>>();
builder.Services.AddScoped<ICollectionService<Playlist>, CollectionService<Playlist>>();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Report Services
builder.Services.AddScoped<IReportableEntityTypeService, ReportableEntityTypeService>();
builder.Services.AddScoped<IReportReasonTypeService, ReportReasonTypeService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Search Services
builder.Services.AddScoped<ISearchService, SearchService>();

// User Services
builder.Services.AddScoped<IUserPrivacyGroupService, UserPrivacyGroupService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserFollowService, UserFollowService>();
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

builder.Services.AddScoped<SmtpSettings>(_ =>
    new SmtpSettings
    {
        // Gmail SMTP: smtp.gmail.com, Port 587 (TLS) or 465 (SSL)
        Host = builder.Configuration["Smtp:Host"] ?? string.Empty,
        Port = builder.Configuration.GetValue<int>("Smtp:Port", 587),
        Username = builder.Configuration["Smtp:Username"] ?? string.Empty,
        Password = builder.Configuration["Smtp:Password"] ?? string.Empty,
        From = builder.Configuration["Smtp:From"] ?? string.Empty,
        FromName = builder.Configuration["Smtp:FromName"],
        EnableSsl = builder.Configuration.GetValue<bool>("Smtp:EnableSsl", true), // Required for Gmail
        UseDefaultCredentials = builder.Configuration.GetValue<bool>("Smtp:UseDefaultCredentials", false)
    }
);

builder.Services.AddSingleton<IChatNotifier, ChatNotifier>();


// Utility Services
// Switch between MailgunEmailService and SmtpEmailService based on configuration
// Default to MailgunEmailService if Smtp:Host is not configured
bool useSmtp = !string.IsNullOrEmpty(builder.Configuration["Smtp:Host"]);
if (useSmtp)
{
    builder.Services.AddScoped<IEmailSenderService>(sp => 
        new SmtpEmailService(sp.GetRequiredService<SmtpSettings>(), builder.Configuration));
}
else
{
    builder.Services.AddScoped<IEmailSenderService>(sp => 
        new MailgunEmailService(
            sp.GetRequiredService<MailgunSettings>(), 
            sp.GetRequiredService<HttpClient>(), 
            builder.Configuration));
}
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<QRCodeGenerator>();
builder.Services.AddSingleton<IShareService, ShareService>();
builder.Services.AddSingleton<IFileFormatInspector>(new FileFormatInspector(
    [new Png(), new Jpeg(), new Mp3(), new Flac(), new Gif()]));
builder.Services.AddSingleton<IFileStorageService, FileStorageService>();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Add gRPC client for Analytics Service
builder.Services.AddGrpcClient<Analytics.API.Analytics.AnalyticsClient>(o =>
{
    o.Address = new Uri(builder.Configuration["Analytics:GrpcUrl"] 
        ?? throw new InvalidOperationException("Analytics:GrpcUrl not configured"));
});

builder.Services.AddGrpcClient<Analytics.API.Recommendations.RecommendationsClient>(o =>
{
    o.Address = new Uri(builder.Configuration["Analytics:GrpcUrl"]
        ?? throw new InvalidOperationException("Analytics:GrpcUrl not configured"));
});

#endregion

WebApplication app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Exposes OpenAPI JSON at /openapi/v1.json

    // Add Swagger UI
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Sonar API v1");
        options.RoutePrefix = "swagger"; // Access UI at /swagger
    });
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("ViteDev");

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<ChatHub>("/hubs/chat");
app.MapControllers();


app.Run();