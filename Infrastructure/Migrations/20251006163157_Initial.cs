using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessFeature",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessFeature", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AchievementCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CosmeticItemType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CosmeticItemType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GiftStyle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftStyle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Locale = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    NativeName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlaybackQuality",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BitRate = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaybackQuality", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportableEntityType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportableEntityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportReasonType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RecommendedSuspensionDuration = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportReasonType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionFeature",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionFeature", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPack",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DiscountMultiplier = table.Column<double>(type: "double precision", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPack", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Theme",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Theme", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPrivacyGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrivacyGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisibilityStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisibilityStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionFeatureSubscriptionPack",
                columns: table => new
                {
                    SubscriptionFeaturesId = table.Column<int>(type: "integer", nullable: false),
                    SubscriptionPacksId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionFeatureSubscriptionPack", x => new { x.SubscriptionFeaturesId, x.SubscriptionPacksId });
                    table.ForeignKey(
                        name: "FK_SubscriptionFeatureSubscriptionPack_SubscriptionFeature_Sub~",
                        column: x => x.SubscriptionFeaturesId,
                        principalTable: "SubscriptionFeature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionFeatureSubscriptionPack_SubscriptionPack_Subscr~",
                        column: x => x.SubscriptionPacksId,
                        principalTable: "SubscriptionPack",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPrivacySettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WhichCanViewProfileId = table.Column<int>(type: "integer", nullable: false),
                    WhichCanMessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrivacySettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPrivacySettings_UserPrivacyGroup_WhichCanMessageId",
                        column: x => x.WhichCanMessageId,
                        principalTable: "UserPrivacyGroup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserPrivacySettings_UserPrivacyGroup_WhichCanViewProfileId",
                        column: x => x.WhichCanViewProfileId,
                        principalTable: "UserPrivacyGroup",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VisibilityState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SetPublicOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisibilityState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisibilityState_VisibilityStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "VisibilityStatus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AutoPlay = table.Column<bool>(type: "boolean", nullable: false),
                    Crossfade = table.Column<bool>(type: "boolean", nullable: false),
                    ExplicitContent = table.Column<bool>(type: "boolean", nullable: false),
                    PreferredPlaybackQualityId = table.Column<int>(type: "integer", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false),
                    ThemeId = table.Column<int>(type: "integer", nullable: false),
                    UserPrivacySettingsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Settings_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Settings_PlaybackQuality_PreferredPlaybackQualityId",
                        column: x => x.PreferredPlaybackQualityId,
                        principalTable: "PlaybackQuality",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Settings_Theme_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "Theme",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Settings_UserPrivacySettings_UserPrivacySettingsId",
                        column: x => x.UserPrivacySettingsId,
                        principalTable: "UserPrivacySettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTypeSettings",
                columns: table => new
                {
                    NotificationTypesId = table.Column<int>(type: "integer", nullable: false),
                    SettingsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTypeSettings", x => new { x.NotificationTypesId, x.SettingsId });
                    table.ForeignKey(
                        name: "FK_NotificationTypeSettings_NotificationType_NotificationTypes~",
                        column: x => x.NotificationTypesId,
                        principalTable: "NotificationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationTypeSettings_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessFeatureUser",
                columns: table => new
                {
                    AccessFeaturesId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessFeatureUser", x => new { x.AccessFeaturesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_AccessFeatureUser_AccessFeature_AccessFeaturesId",
                        column: x => x.AccessFeaturesId,
                        principalTable: "AccessFeature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Achievement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Condition = table.Column<string>(type: "text", nullable: false),
                    Target = table.Column<string>(type: "text", nullable: false),
                    Reward = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Achievement_AchievementCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "AchievementCategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AchievementProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AchievementId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AchievementProgress_Achievement_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievement",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Album",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    DistributorId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Album", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlbumArtist",
                columns: table => new
                {
                    AlbumsId = table.Column<int>(type: "integer", nullable: false),
                    ArtistsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumArtist", x => new { x.AlbumsId, x.ArtistsId });
                    table.ForeignKey(
                        name: "FK_AlbumArtist_Album_AlbumsId",
                        column: x => x.AlbumsId,
                        principalTable: "Album",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Artist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArtistTrack",
                columns: table => new
                {
                    ArtistsId = table.Column<int>(type: "integer", nullable: false),
                    TracksId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistTrack", x => new { x.ArtistsId, x.TracksId });
                    table.ForeignKey(
                        name: "FK_ArtistTrack_Artist_ArtistsId",
                        column: x => x.ArtistsId,
                        principalTable: "Artist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Username = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Login = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Biography = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    PublicIdentifier = table.Column<string>(type: "text", nullable: false),
                    AvailableCurrency = table.Column<int>(type: "integer", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Enabled2FA = table.Column<bool>(type: "boolean", nullable: false),
                    GoogleAuthorizationKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AvatarImageId = table.Column<int>(type: "integer", nullable: false),
                    VisibilityStateId = table.Column<int>(type: "integer", nullable: false),
                    SubscriptionPackId = table.Column<int>(type: "integer", nullable: true),
                    UserStateId = table.Column<int>(type: "integer", nullable: false),
                    SettingsId = table.Column<int>(type: "integer", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_SubscriptionPack_SubscriptionPackId",
                        column: x => x.SubscriptionPackId,
                        principalTable: "SubscriptionPack",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetUsers_VisibilityState_VisibilityStateId",
                        column: x => x.VisibilityStateId,
                        principalTable: "VisibilityState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Library",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Library", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Library_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "License",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssuingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IssuerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_License", x => x.Id);
                    table.ForeignKey(
                        name: "FK_License_AspNetUsers_IssuerId",
                        column: x => x.IssuerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TextContent = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    VisibilityStateId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Post_VisibilityState_VisibilityStateId",
                        column: x => x.VisibilityStateId,
                        principalTable: "VisibilityState",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokenUser",
                columns: table => new
                {
                    RefreshTokensId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokenUser", x => new { x.RefreshTokensId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_RefreshTokenUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefreshTokenUser_RefreshTokens_RefreshTokensId",
                        column: x => x.RefreshTokensId,
                        principalTable: "RefreshTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    EntityIdentifier = table.Column<int>(type: "integer", nullable: false),
                    ReportableEntityTypeId = table.Column<int>(type: "integer", nullable: false),
                    ReporterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Report_AspNetUsers_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_ReportableEntityType_ReportableEntityTypeId",
                        column: x => x.ReportableEntityTypeId,
                        principalTable: "ReportableEntityType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SettingsUser",
                columns: table => new
                {
                    BlockedUsersId = table.Column<int>(type: "integer", nullable: false),
                    SettingsBlockedUsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingsUser", x => new { x.BlockedUsersId, x.SettingsBlockedUsersId });
                    table.ForeignKey(
                        name: "FK_SettingsUser_AspNetUsers_BlockedUsersId",
                        column: x => x.BlockedUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SettingsUser_Settings_SettingsBlockedUsersId",
                        column: x => x.SettingsBlockedUsersId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    BuyerId = table.Column<int>(type: "integer", nullable: false),
                    SubscriptionPackId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionPayment_AspNetUsers_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionPayment_SubscriptionPack_SubscriptionPackId",
                        column: x => x.SubscriptionPackId,
                        principalTable: "SubscriptionPack",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserAgent = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DeviceName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    LastActive = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSession_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Folder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LibraryId = table.Column<int>(type: "integer", nullable: false),
                    ParentFolderId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folder_Folder_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "Folder",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Folder_Library_LibraryId",
                        column: x => x.LibraryId,
                        principalTable: "Library",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    TypeId = table.Column<int>(type: "integer", nullable: false),
                    PostId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                    table.ForeignKey(
                        name: "FK_File_FileType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "FileType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_File_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReportReportReasonType",
                columns: table => new
                {
                    ReportReasonTypeId = table.Column<int>(type: "integer", nullable: false),
                    ReportsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportReportReasonType", x => new { x.ReportReasonTypeId, x.ReportsId });
                    table.ForeignKey(
                        name: "FK_ReportReportReasonType_ReportReasonType_ReportReasonTypeId",
                        column: x => x.ReportReasonTypeId,
                        principalTable: "ReportReasonType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportReportReasonType_Report_ReportsId",
                        column: x => x.ReportsId,
                        principalTable: "Report",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Suspension",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PunisherId = table.Column<int>(type: "integer", nullable: false),
                    AssociatedReportId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suspension", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suspension_AspNetUsers_PunisherId",
                        column: x => x.PunisherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Suspension_Report_AssociatedReportId",
                        column: x => x.AssociatedReportId,
                        principalTable: "Report",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gift",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TextContent = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    GiftTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GiftStyleId = table.Column<int>(type: "integer", nullable: false),
                    ReceiverId = table.Column<int>(type: "integer", nullable: false),
                    SubscriptionPaymentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gift_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Gift_GiftStyle_GiftStyleId",
                        column: x => x.GiftStyleId,
                        principalTable: "GiftStyle",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Gift_SubscriptionPayment_SubscriptionPaymentId",
                        column: x => x.SubscriptionPaymentId,
                        principalTable: "SubscriptionPayment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsGroup = table.Column<bool>(type: "boolean", nullable: false),
                    CoverId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chat_File_CoverId",
                        column: x => x.CoverId,
                        principalTable: "File",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Collection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    VisibilityStateId = table.Column<int>(type: "integer", nullable: false),
                    CoverId = table.Column<int>(type: "integer", nullable: false),
                    FolderId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collection_File_CoverId",
                        column: x => x.CoverId,
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Collection_Folder_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folder",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Collection_VisibilityState_VisibilityStateId",
                        column: x => x.VisibilityStateId,
                        principalTable: "VisibilityState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CosmeticItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    TypeId = table.Column<int>(type: "integer", nullable: false),
                    FileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CosmeticItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CosmeticItem_CosmeticItemType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "CosmeticItemType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CosmeticItem_File_FileId",
                        column: x => x.FileId,
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Distributor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LicenseId = table.Column<int>(type: "integer", nullable: false),
                    CoverId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distributor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Distributor_File_CoverId",
                        column: x => x.CoverId,
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Distributor_License_LicenseId",
                        column: x => x.LicenseId,
                        principalTable: "License",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Track",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsExplicit = table.Column<bool>(type: "boolean", nullable: false),
                    DrivingDisturbingNoises = table.Column<bool>(type: "boolean", nullable: false),
                    VisibilityStateId = table.Column<int>(type: "integer", nullable: false),
                    AudioFileId = table.Column<int>(type: "integer", nullable: false),
                    CoverId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Track", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Track_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Track_File_AudioFileId",
                        column: x => x.AudioFileId,
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Track_File_CoverId",
                        column: x => x.CoverId,
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Track_VisibilityState_VisibilityStateId",
                        column: x => x.VisibilityStateId,
                        principalTable: "VisibilityState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatUser",
                columns: table => new
                {
                    ChatsId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUser", x => new { x.ChatsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ChatUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatUser_Chat_ChatsId",
                        column: x => x.ChatsId,
                        principalTable: "Chat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextContent = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ReplyMessageId = table.Column<int>(type: "integer", nullable: true),
                    ChatId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_Chat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chat",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Message_Message_ReplyMessageId",
                        column: x => x.ReplyMessageId,
                        principalTable: "Message",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Blend",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blend", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blend_Collection_Id",
                        column: x => x.Id,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionUser",
                columns: table => new
                {
                    CollectionsId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionUser", x => new { x.CollectionsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_CollectionUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionUser_Collection_CollectionsId",
                        column: x => x.CollectionsId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Playlist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playlist_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Playlist_Collection_Id",
                        column: x => x.Id,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Queue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Position = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CollectionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Queue_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CosmeticItemInventory",
                columns: table => new
                {
                    CosmeticItemsId = table.Column<int>(type: "integer", nullable: false),
                    InventoriesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CosmeticItemInventory", x => new { x.CosmeticItemsId, x.InventoriesId });
                    table.ForeignKey(
                        name: "FK_CosmeticItemInventory_CosmeticItem_CosmeticItemsId",
                        column: x => x.CosmeticItemsId,
                        principalTable: "CosmeticItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CosmeticItemInventory_Inventory_InventoriesId",
                        column: x => x.InventoriesId,
                        principalTable: "Inventory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CosmeticSticker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    X = table.Column<double>(type: "double precision", nullable: false),
                    Y = table.Column<double>(type: "double precision", nullable: false),
                    CosmeticItemId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CosmeticSticker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CosmeticSticker_CosmeticItem_CosmeticItemId",
                        column: x => x.CosmeticItemId,
                        principalTable: "CosmeticItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DistributorSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserAgent = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DeviceName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    LastActive = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DistributorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributorSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistributorSession_Distributor_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributor",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CollectionTrack",
                columns: table => new
                {
                    CollectionsId = table.Column<int>(type: "integer", nullable: false),
                    TracksId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionTrack", x => new { x.CollectionsId, x.TracksId });
                    table.ForeignKey(
                        name: "FK_CollectionTrack_Collection_CollectionsId",
                        column: x => x.CollectionsId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionTrack_Track_TracksId",
                        column: x => x.TracksId,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageRead",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageRead", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageRead_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MessageRead_Message_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Message",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageUser",
                columns: table => new
                {
                    MessagesId = table.Column<int>(type: "integer", nullable: false),
                    usersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageUser", x => new { x.MessagesId, x.usersId });
                    table.ForeignKey(
                        name: "FK_MessageUser_AspNetUsers_usersId",
                        column: x => x.usersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageUser_Message_MessagesId",
                        column: x => x.MessagesId,
                        principalTable: "Message",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueueTrack",
                columns: table => new
                {
                    QueuesId = table.Column<int>(type: "integer", nullable: false),
                    TracksId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueTrack", x => new { x.QueuesId, x.TracksId });
                    table.ForeignKey(
                        name: "FK_QueueTrack_Queue_QueuesId",
                        column: x => x.QueuesId,
                        principalTable: "Queue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QueueTrack_Track_TracksId",
                        column: x => x.TracksId,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimarySessionId = table.Column<int>(type: "integer", nullable: true),
                    UserStatusId = table.Column<int>(type: "integer", nullable: false),
                    QueueId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserState_Queue_QueueId",
                        column: x => x.QueueId,
                        principalTable: "Queue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserState_UserSession_PrimarySessionId",
                        column: x => x.PrimarySessionId,
                        principalTable: "UserSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserState_UserStatus_UserStatusId",
                        column: x => x.UserStatusId,
                        principalTable: "UserStatus",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AchievementCategory",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Listening" },
                    { 2, "Sharing" },
                    { 3, "Collections" },
                    { 4, "Community" }
                });

            migrationBuilder.InsertData(
                table: "FileType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "image" },
                    { 2, "audio" },
                    { 3, "gif" }
                });

            migrationBuilder.InsertData(
                table: "GiftStyle",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Classic" },
                    { 2, "Modern" },
                    { 3, "Festive" },
                    { 4, "Minimal" },
                    { 5, "Luxury" }
                });

            migrationBuilder.InsertData(
                table: "Language",
                columns: new[] { "Id", "Locale", "Name", "NativeName" },
                values: new object[,]
                {
                    { 1, "eng", "English", "English" },
                    { 2, "ua", "Ukrainian", "Українська" },
                    { 3, "ro", "Romanian", "română" }
                });

            migrationBuilder.InsertData(
                table: "NotificationType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Notification about a new message from another user", "Message" },
                    { 2, "Notification about a new friend request", "FriendRequest" },
                    { 3, "Notification about system updates or alerts", "SystemAlert" },
                    { 4, "Notification about promotions or special offers", "Promotion" }
                });

            migrationBuilder.InsertData(
                table: "PlaybackQuality",
                columns: new[] { "Id", "BitRate", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 128, "Low quality playback suitable for slow internet connections", "Low" },
                    { 2, 320, "Balanced quality and performance", "Medium" },
                    { 3, 700, "High quality playback for premium experience", "High" }
                });

            migrationBuilder.InsertData(
                table: "ReportReasonType",
                columns: new[] { "Id", "Name", "RecommendedSuspensionDuration" },
                values: new object[,]
                {
                    { 1, "Spam", new TimeSpan(1, 0, 0, 0, 0) },
                    { 2, "Harassment", new TimeSpan(7, 0, 0, 0, 0) },
                    { 3, "Copyright Violation", new TimeSpan(30, 0, 0, 0, 0) },
                    { 4, "Inappropriate Content", new TimeSpan(14, 0, 0, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "ReportableEntityType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "User" },
                    { 2, "Track" },
                    { 3, "Album" },
                    { 4, "Comment" }
                });

            migrationBuilder.InsertData(
                table: "Theme",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, null, "Dark" },
                    { 2, null, "Light" }
                });

            migrationBuilder.InsertData(
                table: "VisibilityStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Visible" },
                    { 2, "Unlisted" },
                    { 3, "Restricted" },
                    { 4, "Hidden" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessFeatureUser_UsersId",
                table: "AccessFeatureUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_CategoryId",
                table: "Achievement",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_UserId",
                table: "Achievement",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementProgress_AchievementId",
                table: "AchievementProgress",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementProgress_UserId",
                table: "AchievementProgress",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Album_DistributorId",
                table: "Album",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumArtist_ArtistsId",
                table: "AlbumArtist",
                column: "ArtistsId");

            migrationBuilder.CreateIndex(
                name: "IX_Artist_UserId",
                table: "Artist",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArtistTrack_TracksId",
                table: "ArtistTrack",
                column: "TracksId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AvatarImageId",
                table: "AspNetUsers",
                column: "AvatarImageId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SettingsId",
                table: "AspNetUsers",
                column: "SettingsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SubscriptionPackId",
                table: "AspNetUsers",
                column: "SubscriptionPackId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserStateId",
                table: "AspNetUsers",
                column: "UserStateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_VisibilityStateId",
                table: "AspNetUsers",
                column: "VisibilityStateId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chat_CoverId",
                table: "Chat",
                column: "CoverId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUser_UsersId",
                table: "ChatUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Collection_CoverId",
                table: "Collection",
                column: "CoverId");

            migrationBuilder.CreateIndex(
                name: "IX_Collection_FolderId",
                table: "Collection",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Collection_VisibilityStateId",
                table: "Collection",
                column: "VisibilityStateId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionTrack_TracksId",
                table: "CollectionTrack",
                column: "TracksId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionUser_UsersId",
                table: "CollectionUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_CosmeticItem_FileId",
                table: "CosmeticItem",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_CosmeticItem_TypeId",
                table: "CosmeticItem",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CosmeticItemInventory_InventoriesId",
                table: "CosmeticItemInventory",
                column: "InventoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_CosmeticSticker_CosmeticItemId",
                table: "CosmeticSticker",
                column: "CosmeticItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributor_CoverId",
                table: "Distributor",
                column: "CoverId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributor_LicenseId",
                table: "Distributor",
                column: "LicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorSession_DistributorId",
                table: "DistributorSession",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_File_PostId",
                table: "File",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_File_TypeId",
                table: "File",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Folder_LibraryId",
                table: "Folder",
                column: "LibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_Folder_ParentFolderId",
                table: "Folder",
                column: "ParentFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Gift_GiftStyleId",
                table: "Gift",
                column: "GiftStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_Gift_ReceiverId",
                table: "Gift",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Gift_SubscriptionPaymentId",
                table: "Gift",
                column: "SubscriptionPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_UserId",
                table: "Inventory",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Library_UserId",
                table: "Library",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_License_IssuerId",
                table: "License",
                column: "IssuerId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ChatId",
                table: "Message",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ReplyMessageId",
                table: "Message",
                column: "ReplyMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRead_MessageId",
                table: "MessageRead",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRead_UserId",
                table: "MessageRead",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageUser_usersId",
                table: "MessageUser",
                column: "usersId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTypeSettings_SettingsId",
                table: "NotificationTypeSettings",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlist_CreatorId",
                table: "Playlist",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserId",
                table: "Post",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_VisibilityStateId",
                table: "Post",
                column: "VisibilityStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Queue_CollectionId",
                table: "Queue",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueTrack_TracksId",
                table: "QueueTrack",
                column: "TracksId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokenUser_UsersId",
                table: "RefreshTokenUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportableEntityTypeId",
                table: "Report",
                column: "ReportableEntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReporterId",
                table: "Report",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportReportReasonType_ReportsId",
                table: "ReportReportReasonType",
                column: "ReportsId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_LanguageId",
                table: "Settings",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_PreferredPlaybackQualityId",
                table: "Settings",
                column: "PreferredPlaybackQualityId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_ThemeId",
                table: "Settings",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_UserPrivacySettingsId",
                table: "Settings",
                column: "UserPrivacySettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingsUser_SettingsBlockedUsersId",
                table: "SettingsUser",
                column: "SettingsBlockedUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionFeatureSubscriptionPack_SubscriptionPacksId",
                table: "SubscriptionFeatureSubscriptionPack",
                column: "SubscriptionPacksId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPayment_BuyerId",
                table: "SubscriptionPayment",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPayment_SubscriptionPackId",
                table: "SubscriptionPayment",
                column: "SubscriptionPackId");

            migrationBuilder.CreateIndex(
                name: "IX_Suspension_AssociatedReportId",
                table: "Suspension",
                column: "AssociatedReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Suspension_PunisherId",
                table: "Suspension",
                column: "PunisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Track_AudioFileId",
                table: "Track",
                column: "AudioFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Track_CoverId",
                table: "Track",
                column: "CoverId");

            migrationBuilder.CreateIndex(
                name: "IX_Track_UserId",
                table: "Track",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Track_VisibilityStateId",
                table: "Track",
                column: "VisibilityStateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivacySettings_WhichCanMessageId",
                table: "UserPrivacySettings",
                column: "WhichCanMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivacySettings_WhichCanViewProfileId",
                table: "UserPrivacySettings",
                column: "WhichCanViewProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_UserId",
                table: "UserSession",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserState_PrimarySessionId",
                table: "UserState",
                column: "PrimarySessionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserState_QueueId",
                table: "UserState",
                column: "QueueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserState_UserStatusId",
                table: "UserState",
                column: "UserStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_VisibilityState_StatusId",
                table: "VisibilityState",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessFeatureUser_AspNetUsers_UsersId",
                table: "AccessFeatureUser",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Achievement_AspNetUsers_UserId",
                table: "Achievement",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AchievementProgress_AspNetUsers_UserId",
                table: "AchievementProgress",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Album_Collection_Id",
                table: "Album",
                column: "Id",
                principalTable: "Collection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Album_Distributor_DistributorId",
                table: "Album",
                column: "DistributorId",
                principalTable: "Distributor",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumArtist_Artist_ArtistsId",
                table: "AlbumArtist",
                column: "ArtistsId",
                principalTable: "Artist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Artist_AspNetUsers_UserId",
                table: "Artist",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistTrack_Track_TracksId",
                table: "ArtistTrack",
                column: "TracksId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_File_AvatarImageId",
                table: "AspNetUsers",
                column: "AvatarImageId",
                principalTable: "File",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserState_UserStateId",
                table: "AspNetUsers",
                column: "UserStateId",
                principalTable: "UserState",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Library_AspNetUsers_UserId",
                table: "Library");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_AspNetUsers_UserId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSession_AspNetUsers_UserId",
                table: "UserSession");

            migrationBuilder.DropTable(
                name: "AccessFeatureUser");

            migrationBuilder.DropTable(
                name: "AchievementProgress");

            migrationBuilder.DropTable(
                name: "AlbumArtist");

            migrationBuilder.DropTable(
                name: "ArtistTrack");
            

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Blend");

            migrationBuilder.DropTable(
                name: "ChatUser");

            migrationBuilder.DropTable(
                name: "CollectionTrack");

            migrationBuilder.DropTable(
                name: "CollectionUser");

            migrationBuilder.DropTable(
                name: "CosmeticItemInventory");

            migrationBuilder.DropTable(
                name: "CosmeticSticker");

            migrationBuilder.DropTable(
                name: "DistributorSession");

            migrationBuilder.DropTable(
                name: "Gift");

            migrationBuilder.DropTable(
                name: "MessageRead");

            migrationBuilder.DropTable(
                name: "MessageUser");

            migrationBuilder.DropTable(
                name: "NotificationTypeSettings");

            migrationBuilder.DropTable(
                name: "Playlist");

            migrationBuilder.DropTable(
                name: "QueueTrack");

            migrationBuilder.DropTable(
                name: "RefreshTokenUser");

            migrationBuilder.DropTable(
                name: "ReportReportReasonType");

            migrationBuilder.DropTable(
                name: "SettingsUser");

            migrationBuilder.DropTable(
                name: "SubscriptionFeatureSubscriptionPack");

            migrationBuilder.DropTable(
                name: "Suspension");

            migrationBuilder.DropTable(
                name: "AccessFeature");

            migrationBuilder.DropTable(
                name: "Achievement");

            migrationBuilder.DropTable(
                name: "Album");

            migrationBuilder.DropTable(
                name: "Artist");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "CosmeticItem");

            migrationBuilder.DropTable(
                name: "GiftStyle");

            migrationBuilder.DropTable(
                name: "SubscriptionPayment");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "NotificationType");

            migrationBuilder.DropTable(
                name: "Track");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "ReportReasonType");

            migrationBuilder.DropTable(
                name: "SubscriptionFeature");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "AchievementCategory");

            migrationBuilder.DropTable(
                name: "Distributor");

            migrationBuilder.DropTable(
                name: "CosmeticItemType");

            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.DropTable(
                name: "ReportableEntityType");

            migrationBuilder.DropTable(
                name: "License");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "SubscriptionPack");

            migrationBuilder.DropTable(
                name: "UserState");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropTable(
                name: "PlaybackQuality");

            migrationBuilder.DropTable(
                name: "Theme");

            migrationBuilder.DropTable(
                name: "UserPrivacySettings");

            migrationBuilder.DropTable(
                name: "Queue");

            migrationBuilder.DropTable(
                name: "UserSession");

            migrationBuilder.DropTable(
                name: "UserStatus");

            migrationBuilder.DropTable(
                name: "UserPrivacyGroup");

            migrationBuilder.DropTable(
                name: "Collection");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropTable(
                name: "Folder");

            migrationBuilder.DropTable(
                name: "FileType");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Library");

            migrationBuilder.DropTable(
                name: "VisibilityState");

            migrationBuilder.DropTable(
                name: "VisibilityStatus");
        }
    }
}
