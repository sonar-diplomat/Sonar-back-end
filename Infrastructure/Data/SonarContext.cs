using Entities.Models.Access;
using Entities.Models.Chat;
using Entities.Models.ClientSettings;
using Entities.Models.Distribution;
using Entities.Models.File;
using Entities.Models.Library;
using Entities.Models.Music;
using Entities.Models.Report;
using Entities.Models.UserCore;
using Entities.Models.UserExperience;
using Infrastructure.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = Entities.Models.File;

namespace Infrastructure.Data
{
    public class SonarContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public SonarContext(DbContextOptions<SonarContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Settings>()
                .HasOne(r => r.User)
                .WithMany(a => a.SettingsBlockedUsers)
                .HasForeignKey(r => r.UserId);

            builder.Entity<User>()
                .HasOne(r => r.Settings)
                .WithOne(a => a.User)
                .HasForeignKey<Settings>(r => r.UserId);
            // SomeEnum.SentMessage = "SentMessage SentMassegh"
            // User.Coolection DbSet(Name Id)
            builder.Entity<NotificationType>().HasData(NotificationTypeSeedFactory.CreateSeedData());

            builder.Entity<Theme>().HasData(ThemeSeedFactory.CreateSeedData());

            builder.Entity<Language>().HasData(LanguageSeedFactory.CreateSeedData());

            builder.Entity<PlaybackQuality>().HasData(PlaybackQualitySeedFactory.CreateSeedData());

            builder.Entity<AchievementCategory>().HasData(AchievementCategorySeedFactory.CreateSeedData());

            builder.Entity<FileType>().HasData(FileTypeSeedFactory.CreateSeedData());

            builder.Entity<VisibilityStatus>().HasData(VisibilityStatusSeedFactory.CreateSeedData());

            builder.Entity<GiftStyle>().HasData(GiftStyleSeedFactory.CreateSeedData());

            builder.Entity<ReportableEntityType>().HasData(ReportableEntityTypeSeedFactory.CreateSeedData());

            builder.Entity<ReportReasonType>().HasData(ReportReasonTypeSeedFactory.CreateSeedData());



            base.OnModelCreating(builder);
        }

        // Access
        public DbSet<AccessFeature> AccessFeatures { get; set; } = null!;
        public DbSet<Suspension> Suspensions { get; set; } = null!;
        public DbSet<VisibilityState> VisibilityStates { get; set; } = null!;
        public DbSet<VisibilityStatus> VisibilityStatuses { get; set; } = null!;

        // Chat
        public DbSet<Chat> Chats { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<MessageRead> MessageReads { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;

        // ClientSettings
        public DbSet<Language> Languages { get; set; } = null!;
        public DbSet<NotificationType> NotificationTypes { get; set; } = null!;
        public DbSet<PlaybackQuality> PlaybackQualities { get; set; } = null!;
        public DbSet<Settings> Settings { get; set; } = null!;
        public DbSet<Theme> Themes { get; set; } = null!;
        public DbSet<UserPrivacySettings> UserPrivacySettings { get; set; } = null!;

        // ClientSettings
        public DbSet<Artist> Artists { get; set; } = null!;
        public DbSet<Distributor> Distributors { get; set; } = null!;
        public DbSet<DistributorSession> DistributorSessions { get; set; } = null!;
        public DbSet<License> Licenses { get; set; } = null!;

        // File
        public DbSet<File.File> Files { get; set; } = null!;
        public DbSet<File.FileType> FileTypes { get; set; } = null!;

        // Library
        public DbSet<Folder> Folders { get; set; } = null!;
        public DbSet<Library> Libraries { get; set; } = null!;

        // Music
        public DbSet<Album> Albums { get; set; } = null!;
        public DbSet<Blend> Blends { get; set; } = null!;
        public DbSet<Playlist> Playlists { get; set; } = null!;
        public DbSet<Track> Tracks { get; set; } = null!;

        // Report
        public DbSet<Report> Reports { get; set; } = null!;
        public DbSet<ReportableEntityType> ReportableEntityTypes { get; set; } = null!;
        public DbSet<ReportReasonType> ReportReasonTypes { get; set; } = null!;

        // User
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserPrivacyGroup> UserPrivacyGroups { get; set; } = null!;
        public DbSet<UserSession> UserSessions { get; set; } = null!;
        public DbSet<UserState> UserStates { get; set; } = null!;
        public DbSet<UserStatus> UserStatuses { get; set; } = null!;

        // UserExperience
        public DbSet<Achievement> Achievements { get; set; } = null!;
        public DbSet<AchievementCategory> AchievementCategories { get; set; } = null!;
        public DbSet<AchievementProgress> AchievementProgresses { get; set; } = null!;
        public DbSet<CosmeticItem> CosmeticItems { get; set; } = null!;
        public DbSet<CosmeticItemType> CosmeticItemTypes { get; set; } = null!;
        public DbSet<CosmeticSticker> CosmeticStickers { get; set; } = null!;
        public DbSet<Gift> Gifts { get; set; } = null!;
        public DbSet<GiftStyle> GiftStyles { get; set; } = null!;
        public DbSet<Inventory> Inventories { get; set; } = null!;
        public DbSet<SubscriptionFeature> SubscriptionFeatures { get; set; } = null!;
        public DbSet<SubscriptionPack> SubscriptionPacks { get; set; } = null!;
        public DbSet<SubscriptionPayment> SubscriptionPayments { get; set; } = null!;
    }
}
