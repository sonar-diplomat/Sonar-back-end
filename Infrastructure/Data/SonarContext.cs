using Entities.Models.Access;
using Entities.Models.Chat;
using Entities.Models.ClientSettings;
using Entities.Models.Distribution;
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

namespace Infrastructure.Data;

public class SonarContext(DbContextOptions<SonarContext> options)
    : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
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
    public DbSet<ChatSticker> ChatStickers { get; set; } = null!;
    public DbSet<ChatStickerCategory> ChatStickerCategories { get; set; } = null!;
    public DbSet<UserFollow> UserFollows { get; set; } = null!;

    // ClientSettings
    public DbSet<Language> Languages { get; set; } = null!;
    public DbSet<NotificationType> NotificationTypes { get; set; } = null!;
    public DbSet<PlaybackQuality> PlaybackQualities { get; set; } = null!;
    public DbSet<Settings> Settings { get; set; } = null!;
    public DbSet<Theme> Themes { get; set; } = null!;
    public DbSet<UserPrivacySettings> UserPrivacySettings { get; set; } = null!;

    // Distribution
    public DbSet<Artist> Artists { get; set; } = null!;
    public DbSet<Distributor> Distributors { get; set; } = null!;
    public DbSet<DistributorAccount> DistributorAccounts { get; set; } = null!;
    public DbSet<DistributorSession> DistributorSessions { get; set; } = null!;
    public DbSet<License> Licenses { get; set; } = null!;

    // File
    public DbSet<File.AudioFile> AudioFiles { get; set; } = null!;
    public DbSet<File.ImageFile> ImageFiles { get; set; } = null!;
    public DbSet<File.VideoFile> VideoFiles { get; set; } = null!;


    // Library
    public DbSet<Folder> Folders { get; set; } = null!;
    public DbSet<Library> Libraries { get; set; } = null!;

    // Music
    public DbSet<Album> Albums { get; set; } = null!;
    public DbSet<Blend> Blends { get; set; } = null!;
    public DbSet<Playlist> Playlists { get; set; } = null!;
    public DbSet<Track> Tracks { get; set; } = null!;
    public DbSet<AlbumArtist> AlbumArtists { get; set; }
    public DbSet<TrackArtist> TrackArtists { get; set; }
    public DbSet<Genre> Genres { get; set; } = null!;
    public DbSet<MoodTag> MoodTags { get; set; } = null!;
    public DbSet<TrackMoodTag> TrackMoodTags { get; set; } = null!;
    public DbSet<AlbumMoodTag> AlbumMoodTags { get; set; } = null!;

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Queue>().HasMany(q => q.Tracks).WithMany(t => t.Queues);

        builder.Entity<Track>().HasMany(t => t.QueuesWherePrimary)
            .WithOne(q => q.CurrentTrack)
            .HasForeignKey(q => q.CurrentTrackId);

        //builder.Entity<AlbumArtist>().HasKey(aa => aa.Id);

        builder.Entity<AlbumArtist>()
            .HasOne(aa => aa.Album)
            .WithMany(a => a.AlbumArtists)
            .HasForeignKey(aa => aa.AlbumId);

        builder.Entity<AlbumArtist>()
            .HasOne(aa => aa.Artist)
            .WithMany(a => a.AlbumArtists) /*.IsRequired(false)*/
            .HasForeignKey(aa => aa.ArtistId) /*.IsRequired(false)*/;

        builder.Entity<TrackArtist>()
            .HasOne(ta => ta.Track)
            .WithMany(t => t.TrackArtists)
            .HasForeignKey(ta => ta.TrackId);

        builder.Entity<TrackArtist>()
            .HasOne(ta => ta.Artist)
            .WithMany(a => a.TrackArtists)/*.IsRequired(false)*/
            .HasForeignKey(ta => ta.ArtistId)/*.IsRequired(false)*/;

        builder.Entity<Track>()
            .HasOne(t => t.Genre)
            .WithMany(g => g.Tracks)
            .HasForeignKey(t => t.GenreId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Album>()
            .HasOne(a => a.Genre)
            .WithMany(g => g.Albums)
            .HasForeignKey(a => a.GenreId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<TrackMoodTag>()
            .HasOne(tmt => tmt.Track)
            .WithMany(t => t.TrackMoodTags)
            .HasForeignKey(tmt => tmt.TrackId);

        builder.Entity<TrackMoodTag>()
            .HasOne(tmt => tmt.MoodTag)
            .WithMany(mt => mt.TrackMoodTags)
            .HasForeignKey(tmt => tmt.MoodTagId);

        builder.Entity<AlbumMoodTag>()
            .HasOne(amt => amt.Album)
            .WithMany(a => a.AlbumMoodTags)
            .HasForeignKey(amt => amt.AlbumId);

        builder.Entity<AlbumMoodTag>()
            .HasOne(amt => amt.MoodTag)
            .WithMany(mt => mt.AlbumMoodTags)
            .HasForeignKey(amt => amt.MoodTagId);

        builder.Entity<User>().HasOne(u => u.Settings).WithOne(s => s.User).HasForeignKey<User>(s => s.SettingsId);
        builder.Entity<Settings>().HasMany(s => s.BlockedUsers).WithMany(s => s.SettingsBlockedUsers);

        builder.Entity<Library>()
            .HasMany(l => l.Folders)
            .WithOne(f => f.Library)
            .HasForeignKey(f => f.LibraryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Folder>()
            .HasOne(f => f.ParentFolder)
            .WithMany(f => f.SubFolders)
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Chat>()
            .HasOne(c => c.Creator)
            .WithMany(u => u.ChatsWhereCreator)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Chat>().HasMany(c => c.Admins).WithMany(u => u.ChatsWhereAdmin)
            .UsingEntity(j => j.ToTable("ChatAdmins"));
        builder.Entity<Chat>().HasMany(c => c.Members).WithMany(u => u.ChatsWhereMember)
            .UsingEntity(j => j.ToTable("ChatMembers"));

        builder.Entity<User>()
            .HasMany(u => u.Friends)
            .WithMany(u => u.FriendOf)
            .UsingEntity(j => j.ToTable("UserFriends"));

        builder.Entity<UserFollow>()
            .HasOne(uf => uf.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(uf => uf.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserFollow>()
            .HasOne(uf => uf.Following)
            .WithMany(u => u.Followers)
            .HasForeignKey(uf => uf.FollowingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<NotificationType>()
            .HasData(NotificationTypeSeedFactory.CreateSeedData());
        builder.Entity<Theme>()
            .HasData(ThemeSeedFactory.CreateSeedData());
        builder.Entity<Language>()
            .HasData(LanguageSeedFactory.CreateSeedData());
        builder.Entity<PlaybackQuality>()
            .HasData(PlaybackQualitySeedFactory.CreateSeedData());
        builder.Entity<AchievementCategory>()
            .HasData(AchievementCategorySeedFactory.CreateSeedData());
        builder.Entity<File.ImageFile>()
            .HasData(ImageFileSeedFactory.CreateSeedData());
        builder.Entity<VisibilityStatus>()
            .HasData(VisibilityStatusSeedFactory.CreateSeedData());
        builder.Entity<GiftStyle>()
            .HasData(GiftStyleSeedFactory.CreateSeedData());
        builder.Entity<Report>()
            .HasOne(r => r.ReportReasonType)
            .WithMany()
            .HasForeignKey(r => r.ReportReasonTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure many-to-many relationship between ReportReasonType and ReportableEntityType
        builder.Entity<ReportReasonType>()
            .HasMany(rrt => rrt.ApplicableEntityTypes)
            .WithMany(ret => ret.ApplicableReportReasonTypes)
            .UsingEntity(j => j.ToTable("ReportReasonTypeReportableEntityType"));

        builder.Entity<ReportableEntityType>()
            .HasData(ReportableEntityTypeSeedFactory.CreateSeedData());
        builder.Entity<ReportReasonType>()
            .HasData(ReportReasonTypeSeedFactory.CreateSeedData());
        
        // Configure mappings between report reason types and entity types
        ReportReasonTypeEntityTypeMappingFactory.ConfigureMappings(builder);
        builder.Entity<AccessFeature>()
            .HasData(AccessFeatureSeedFactory.CreateSeedData());
        builder.Entity<UserPrivacyGroup>()
            .HasData(UserPrivacyGroupSeedFactory.CreateSeedData());
        builder.Entity<UserStatus>()
            .HasData(UserStatusSeedFactory.CreateSeedData());
        builder.Entity<SubscriptionFeature>()
            .HasData(SubscribtionFeatureSeedFactory.CreateSeedData());
        builder.Entity<ChatStickerCategory>()
            .HasData(ChatStickerCategorySeedFactory.CreateSeedData());
        builder.Entity<ChatSticker>()
            .HasData(ChatStickerSeedFactory.CreateSeedData());
        builder.Entity<Genre>()
            .HasData(GenreSeedFactory.CreateSeedData());
        builder.Entity<MoodTag>()
            .HasData(MoodTagSeedFactory.CreateSeedData());
        base.OnModelCreating(builder);
    }
}