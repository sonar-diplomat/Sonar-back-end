using Entities.Models.Access;
using Entities.Models.ClientSettings;
using Entities.Models.File;
using Entities.Models.Library;
using Entities.Models.UserCore;
using Entities.Models.UserExperience;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace Sonar.Tests.Repositories.UserCore;

public abstract class DatabaseTestBase : IAsyncLifetime
{
    protected PostgreSqlContainer? PostgreSqlContainer;
    protected SonarContext? DbContext;
    protected string? ConnectionString;

    public virtual async Task InitializeAsync()
    {
        PostgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .WithCleanUp(true)
            .Build();

        await PostgreSqlContainer.StartAsync();
        ConnectionString = PostgreSqlContainer.GetConnectionString();

        var options = new DbContextOptionsBuilder<SonarContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        DbContext = new SonarContext(options);
        
        // Use EnsureCreated for tests - it creates the database schema without migrations
        // This is faster and doesn't require migration files at runtime
        await DbContext.Database.EnsureCreatedAsync();
        
        // Create seed data required for User creation
        await CreateSeedDataAsync();
    }
    
    private async Task CreateSeedDataAsync()
    {
        if (DbContext == null) return;
        
        // Create VisibilityStatus (required for VisibilityState)
        if (!await DbContext.VisibilityStatuses.AnyAsync())
        {
            DbContext.VisibilityStatuses.AddRange(
                new VisibilityStatus { Id = 1, Name = "Visible" },
                new VisibilityStatus { Id = 2, Name = "Unlisted" },
                new VisibilityStatus { Id = 3, Name = "Restricted" },
                new VisibilityStatus { Id = 4, Name = "Hidden" }
            );
        }
        
        // Create VisibilityState (required for User)
        if (!await DbContext.VisibilityStates.AnyAsync(vs => vs.Id == 2))
        {
            DbContext.VisibilityStates.Add(new VisibilityState
            {
                Id = 2,
                StatusId = 2, // Unlisted
                SetPublicOn = DateTime.UtcNow
            });
        }
        
        // Create UserStatus (required for UserState)
        if (!await DbContext.UserStatuses.AnyAsync())
        {
            DbContext.UserStatuses.AddRange(
                new UserStatus { Id = 1, Name = "online" },
                new UserStatus { Id = 2, Name = "offline" },
                new UserStatus { Id = 3, Name = "do not disturb" },
                new UserStatus { Id = 4, Name = "idle" }
            );
        }
        
        // Create ImageFile (required for User.AvatarImageId)
        if (!await DbContext.ImageFiles.AnyAsync(i => i.Id == 1))
        {
            DbContext.ImageFiles.Add(new ImageFile
            {
                Id = 1,
                ItemName = "Default avatar",
                Url = ""
            });
        }
        
        // Create Queue (required for UserState)
        // Queue is not in DbSet, so we use Set<Queue>() to access it
        if (!await DbContext.Set<Queue>().AnyAsync(q => q.Id == 1))
        {
            DbContext.Set<Queue>().Add(new Queue
            {
                Id = 1,
                Position = TimeSpan.Zero
            });
        }
        
        // Create UserState (required for User)
        if (!await DbContext.UserStates.AnyAsync(us => us.Id == 1))
        {
            DbContext.UserStates.Add(new UserState
            {
                Id = 1,
                UserStatusId = 1, // online
                QueueId = 1
            });
        }
        
        // Create seed data for Settings
        if (!await DbContext.PlaybackQualities.AnyAsync())
        {
            DbContext.PlaybackQualities.Add(new PlaybackQuality 
            { 
                Id = 1, 
                Name = "Low",
                BitRate = 128,
                Description = "Low quality"
            });
        }
        if (!await DbContext.Languages.AnyAsync())
        {
            DbContext.Languages.Add(new Language 
            { 
                Id = 1, 
                Name = "English",
                Locale = "en",
                NativeName = "English"
            });
        }
        if (!await DbContext.Themes.AnyAsync())
        {
            DbContext.Themes.Add(new Theme 
            { 
                Id = 1, 
                Name = "Light",
                Description = "Light theme"
            });
        }
        if (!await DbContext.UserPrivacyGroups.AnyAsync())
        {
            DbContext.UserPrivacyGroups.AddRange(
                new UserPrivacyGroup { Id = 1, Name = "all" },
                new UserPrivacyGroup { Id = 2, Name = "friends" },
                new UserPrivacyGroup { Id = 3, Name = "nobody" }
            );
        }
        if (!await DbContext.UserPrivacySettings.AnyAsync(ups => ups.Id == 1))
        {
            DbContext.UserPrivacySettings.Add(new UserPrivacySettings
            {
                Id = 1,
                WhichCanMessageId = 1, // all
                WhichCanViewProfileId = 1, // all
                AcceptFriendRequests = true
            });
        }
        
        // Create Settings (required for User)
        if (!await DbContext.Settings.AnyAsync(s => s.Id == 1))
        {
            DbContext.Settings.Add(new Settings
            {
                Id = 1,
                AutoPlay = false,
                Crossfade = false,
                ExplicitContent = true,
                PreferredPlaybackQualityId = 1,
                LanguageId = 1,
                ThemeId = 1,
                UserPrivacySettingsId = 1
            });
        }
        
        // Create Inventory (required for User)
        if (!await DbContext.Inventories.AnyAsync(i => i.Id == 1))
        {
            DbContext.Inventories.Add(new Inventory { Id = 1 });
        }
        
        // Create Library (required for User)
        if (!await DbContext.Libraries.AnyAsync(l => l.Id == 1))
        {
            DbContext.Libraries.Add(new Library { Id = 1 });
        }
        
        await DbContext.SaveChangesAsync();
    }

    public virtual async Task DisposeAsync()
    {
        if (DbContext != null)
        {
            await DbContext.DisposeAsync();
        }

        if (PostgreSqlContainer != null)
        {
            await PostgreSqlContainer.DisposeAsync();
        }
    }

    protected async Task CleanDatabaseAsync()
    {
        if (DbContext == null) return;

        // Delete in correct order to respect foreign key constraints
        DbContext.UserFollows.RemoveRange(DbContext.UserFollows);
        
        // Remove all test users (seed data will be recreated)
        // We need to be careful not to delete users that are part of seed data
        // But since we recreate seed data, we can safely remove all users
        var allUsers = await DbContext.Users.ToListAsync();
        DbContext.Users.RemoveRange(allUsers);
        
        await DbContext.SaveChangesAsync();
        
        // Recreate seed data (required for User creation)
        await CreateSeedDataAsync();
    }

    protected async Task<User> CreateTestUserAsync(int id, string userName = "testuser", string email = "test@test.com")
    {
        if (DbContext == null) throw new InvalidOperationException("DbContext is null");

        // Create unique Settings for this user (one-to-one relationship)
        var settings = new Settings
        {
            Id = id,
            AutoPlay = false,
            Crossfade = false,
            ExplicitContent = true,
            PreferredPlaybackQualityId = 1,
            LanguageId = 1,
            ThemeId = 1,
            UserPrivacySettingsId = 1
        };

        // Create unique UserState for this user
        var userState = new UserState
        {
            Id = id,
            UserStatusId = 1, // online
            QueueId = id // Each user needs their own Queue
        };

        // Create unique Queue for this user
        var queue = new Queue
        {
            Id = id,
            Position = TimeSpan.Zero
        };

        // Create unique Library for this user (one-to-one relationship)
        var library = new Library
        {
            Id = id
        };

        // Create unique Inventory for this user
        var inventory = new Inventory
        {
            Id = id
        };

        // Create unique VisibilityState for this user
        var visibilityState = new VisibilityState
        {
            Id = id + 1000, // Use high IDs to avoid conflicts
            StatusId = 2, // Unlisted
            SetPublicOn = DateTime.UtcNow
        };

        // Save all dependencies first
        DbContext.Set<Queue>().Add(queue);
        DbContext.UserStates.Add(userState);
        DbContext.Settings.Add(settings);
        DbContext.Libraries.Add(library);
        DbContext.Inventories.Add(inventory);
        DbContext.VisibilityStates.Add(visibilityState);
        await DbContext.SaveChangesAsync();

        // Now create the user
        return new User
        {
            Id = id,
            UserName = userName,
            NormalizedUserName = userName.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = false,
            FirstName = userName, // In real data, FirstName = LastName = UserName
            LastName = userName,
            DateOfBirth = new DateOnly(2000, 1, 1),
            Login = $"some_{userName}", // Login differs from UserName in real data
            PublicIdentifier = (id * 1000000).ToString(), // PublicIdentifier is numeric in real data
            AvailableCurrency = 0,
            RegistrationDate = DateTime.UtcNow,
            Enabled2FA = false,
            GoogleAuthorizationKey = null,
            AvatarImageId = 1,
            VisibilityStateId = id + 1000,
            SubscriptionPackId = null,
            UserStateId = id,
            SettingsId = id,
            InventoryId = id,
            LibraryId = id,
            PasswordHash = "AQAAAAIAAYagAAAAEEZ1zDl4bYWDKHAniymWRF78fcVbsE4oRKiiGbdKyEChzX+R1mCGqtfRRZGkUSSOTQ==", // Dummy hash for tests
            SecurityStamp = Guid.NewGuid().ToString("N").ToUpperInvariant().Substring(0, 32), // 32 chars like in real data
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = true,
            AccessFailedCount = 0
        };
    }

    protected User CreateTestUser(int id, string userName = "testuser", string email = "test@test.com")
    {
        // Legacy method - use CreateTestUserAsync instead
        // This creates a user object but dependencies must be created separately
        return new User
        {
            Id = id,
            UserName = userName,
            NormalizedUserName = userName.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = false,
            FirstName = userName,
            LastName = userName,
            DateOfBirth = new DateOnly(2000, 1, 1),
            Login = $"some_{userName}",
            PublicIdentifier = (id * 1000000).ToString(),
            AvailableCurrency = 0,
            RegistrationDate = DateTime.UtcNow,
            Enabled2FA = false,
            GoogleAuthorizationKey = null,
            AvatarImageId = 1,
            VisibilityStateId = id + 1000,
            SubscriptionPackId = null,
            UserStateId = id,
            SettingsId = id,
            InventoryId = id,
            LibraryId = id,
            PasswordHash = "AQAAAAIAAYagAAAAEEZ1zDl4bYWDKHAniymWRF78fcVbsE4oRKiiGbdKyEChzX+R1mCGqtfRRZGkUSSOTQ==",
            SecurityStamp = Guid.NewGuid().ToString("N").ToUpperInvariant().Substring(0, 32),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = true,
            AccessFailedCount = 0
        };
    }

    protected async Task CreateUserDependenciesAsync(int userId)
    {
        if (DbContext == null) return;

        // Create unique Queue for this user
        if (!await DbContext.Set<Queue>().AnyAsync(q => q.Id == userId))
        {
            DbContext.Set<Queue>().Add(new Queue
            {
                Id = userId,
                Position = TimeSpan.Zero
            });
        }

        // Create unique UserState for this user
        if (!await DbContext.UserStates.AnyAsync(us => us.Id == userId))
        {
            DbContext.UserStates.Add(new UserState
            {
                Id = userId,
                UserStatusId = 1, // online
                QueueId = userId
            });
        }

        // Create unique VisibilityState for this user
        if (!await DbContext.VisibilityStates.AnyAsync(vs => vs.Id == userId + 1000))
        {
            DbContext.VisibilityStates.Add(new VisibilityState
            {
                Id = userId + 1000,
                StatusId = 2, // Unlisted
                SetPublicOn = DateTime.UtcNow
            });
        }

        // Create unique Settings for this user (one-to-one relationship)
        if (!await DbContext.Settings.AnyAsync(s => s.Id == userId))
        {
            DbContext.Settings.Add(new Settings
            {
                Id = userId,
                AutoPlay = false,
                Crossfade = false,
                ExplicitContent = true,
                PreferredPlaybackQualityId = 1,
                LanguageId = 1,
                ThemeId = 1,
                UserPrivacySettingsId = 1
            });
        }

        // Create unique Library for this user (one-to-one relationship)
        if (!await DbContext.Libraries.AnyAsync(l => l.Id == userId))
        {
            DbContext.Libraries.Add(new Library
            {
                Id = userId
            });
        }

        // Create unique Inventory for this user
        if (!await DbContext.Inventories.AnyAsync(i => i.Id == userId))
        {
            DbContext.Inventories.Add(new Inventory
            {
                Id = userId
            });
        }

        await DbContext.SaveChangesAsync();
    }
}

