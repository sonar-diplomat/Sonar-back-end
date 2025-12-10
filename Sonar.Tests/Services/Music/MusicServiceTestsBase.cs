using Entities.Models.Access;
using Entities.Models.Distribution;
using Entities.Models.File;
using Entities.Models.Music;
using Infrastructure.Data;
using Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Tests.Services.Music;

public abstract class MusicServiceTestsBase : IDisposable
{
    protected readonly SonarContext Context;
    private int _nextId = 1;

    protected MusicServiceTestsBase()
    {
        var options = new DbContextOptionsBuilder<SonarContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new SonarContext(options);
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var genres = GenreSeedFactory.CreateSeedData();
        var moodTags = MoodTagSeedFactory.CreateSeedData();

        Context.Genres.AddRange(genres);
        Context.MoodTags.AddRange(moodTags);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
    }

    protected Genre GetGenre(int id = 1)
    {
        return Context.Genres.First(g => g.Id == id);
    }

    protected MoodTag GetMoodTag(int id = 1)
    {
        return Context.MoodTags.First(mt => mt.Id == id);
    }

    protected List<MoodTag> GetMoodTags(params int[] ids)
    {
        return Context.MoodTags.Where(mt => ids.Contains(mt.Id)).ToList();
    }

    protected VisibilityStatus CreateVisibilityStatus(int? id = null)
    {
        Context.ChangeTracker.Clear();
        var statusId = id ?? GetNextId();
        
        if (Context.VisibilityStatuses.Any(vs => vs.Id == statusId))
        {
            return Context.VisibilityStatuses.First(vs => vs.Id == statusId);
        }

        var status = new VisibilityStatus
        {
            Id = statusId,
            Name = "Public"
        };
        Context.VisibilityStatuses.Add(status);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
        return status;
    }

    protected VisibilityState CreateVisibilityState(int? id = null, int? statusId = null)
    {
        Context.ChangeTracker.Clear();
        var stateId = id ?? GetNextId();
        var visStatusId = statusId ?? 1;

        if (!Context.VisibilityStatuses.Any(vs => vs.Id == visStatusId))
        {
            CreateVisibilityStatus(visStatusId);
        }

        if (Context.VisibilityStates.Any(vs => vs.Id == stateId))
        {
            return Context.VisibilityStates.First(vs => vs.Id == stateId);
        }

        var state = new VisibilityState
        {
            Id = stateId,
            StatusId = visStatusId,
            SetPublicOn = DateTime.UtcNow
        };
        Context.VisibilityStates.Add(state);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
        return state;
    }

    protected ImageFile CreateImageFile(int? id = null, string? itemName = null, string? url = null)
    {
        Context.ChangeTracker.Clear();
        var fileId = id ?? GetNextId();

        if (Context.ImageFiles.Any(f => f.Id == fileId))
        {
            Context.ChangeTracker.Clear();
            return Context.ImageFiles.First(f => f.Id == fileId);
        }

        var file = new ImageFile
        {
            Id = fileId,
            ItemName = itemName ?? $"image_{fileId}.jpg",
            Url = url ?? $"https://example.com/image_{fileId}.jpg"
        };
        Context.ImageFiles.Add(file);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
        return file;
    }

    protected AudioFile CreateAudioFile(int? id = null, string? itemName = null, string? url = null)
    {
        Context.ChangeTracker.Clear();
        var fileId = id ?? GetNextId();

        if (Context.AudioFiles.Any(f => f.Id == fileId))
        {
            Context.ChangeTracker.Clear();
            return Context.AudioFiles.First(f => f.Id == fileId);
        }

        var file = new AudioFile
        {
            Id = fileId,
            ItemName = itemName ?? $"audio_{fileId}.mp3",
            Url = url ?? $"https://example.com/audio_{fileId}.mp3"
        };
        Context.AudioFiles.Add(file);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
        return file;
    }

    protected Entities.Models.UserCore.User CreateUser(int? id = null)
    {
        Context.ChangeTracker.Clear();
        var userId = id ?? GetNextId();

        if (Context.Users.Any(u => u.Id == userId))
        {
            return Context.Users.First(u => u.Id == userId);
        }

        var visibilityState = CreateVisibilityState();
        var avatarImage = CreateImageFile();

        var userStatus = new Entities.Models.UserCore.UserStatus
        {
            Id = 1,
            Name = "Online"
        };
        if (!Context.UserStatuses.Any(us => us.Id == 1))
        {
            Context.UserStatuses.Add(userStatus);
        }

        var queue = new Entities.Models.UserCore.Queue
        {
            Id = userId,
            Position = TimeSpan.Zero
        };
        if (!Context.Set<Entities.Models.UserCore.Queue>().Any(q => q.Id == queue.Id))
        {
            Context.Set<Entities.Models.UserCore.Queue>().Add(queue);
        }

        var userState = new Entities.Models.UserCore.UserState
        {
            Id = userId,
            UserStatusId = 1,
            QueueId = queue.Id
        };
        if (!Context.UserStates.Any(us => us.Id == userState.Id))
        {
            Context.UserStates.Add(userState);
        }

        Context.SaveChanges();
        Context.ChangeTracker.Clear();

        var playbackQuality = new Entities.Models.ClientSettings.PlaybackQuality
        {
            Id = 1,
            Name = "High",
            BitRate = 320,
            Description = "High quality audio"
        };
        if (!Context.PlaybackQualities.Any(pq => pq.Id == 1))
        {
            Context.PlaybackQualities.Add(playbackQuality);
        }

        var language = new Entities.Models.ClientSettings.Language
        {
            Id = 1,
            Locale = "en",
            Name = "English",
            NativeName = "English"
        };
        if (!Context.Languages.Any(l => l.Id == 1))
        {
            Context.Languages.Add(language);
        }

        var theme = new Entities.Models.ClientSettings.Theme
        {
            Id = 1,
            Name = "Dark"
        };
        if (!Context.Themes.Any(t => t.Id == 1))
        {
            Context.Themes.Add(theme);
        }

        var privacyGroup = new Entities.Models.UserCore.UserPrivacyGroup
        {
            Id = 1,
            Name = "Everyone"
        };
        if (!Context.UserPrivacyGroups.Any(upg => upg.Id == 1))
        {
            Context.UserPrivacyGroups.Add(privacyGroup);
        }

        var userPrivacy = new Entities.Models.ClientSettings.UserPrivacySettings
        {
            Id = userId,
            WhichCanViewProfileId = 1,
            WhichCanMessageId = 1,
            AcceptFriendRequests = true
        };
        if (!Context.UserPrivacySettings.Any(ups => ups.Id == userPrivacy.Id))
        {
            Context.UserPrivacySettings.Add(userPrivacy);
        }

        var settings = new Entities.Models.ClientSettings.Settings
        {
            Id = userId,
            AutoPlay = false,
            Crossfade = false,
            ExplicitContent = true,
            PreferredPlaybackQualityId = 1,
            LanguageId = 1,
            ThemeId = 1,
            UserPrivacySettingsId = userPrivacy.Id
        };
        if (!Context.Settings.Any(s => s.Id == settings.Id))
        {
            Context.Settings.Add(settings);
        }

        var inventory = new Entities.Models.UserExperience.Inventory
        {
            Id = userId
        };
        if (!Context.Inventories.Any(i => i.Id == inventory.Id))
        {
            Context.Inventories.Add(inventory);
        }

        var library = new Entities.Models.Library.Library
        {
            Id = userId
        };
        if (!Context.Libraries.Any(l => l.Id == library.Id))
        {
            Context.Libraries.Add(library);
        }

        Context.SaveChanges();
        Context.ChangeTracker.Clear();

        var user = new Entities.Models.UserCore.User
        {
            Id = userId,
            UserName = $"user_{userId}",
            Email = $"user_{userId}@example.com",
            FirstName = "Test",
            LastName = "User",
            Login = $"user_{userId}",
            PublicIdentifier = $"user_{userId}",
            DateOfBirth = new DateOnly(2000, 1, 1),
            RegistrationDate = DateTime.UtcNow,
            AvailableCurrency = 0,
            Enabled2FA = false,
            AvatarImageId = avatarImage.Id,
            VisibilityStateId = visibilityState.Id,
            UserStateId = userState.Id,
            SettingsId = settings.Id,
            InventoryId = inventory.Id,
            LibraryId = library.Id
        };
        Context.Users.Add(user);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
        return user;
    }

    protected License CreateLicense(int? id = null, int? issuerId = null)
    {
        Context.ChangeTracker.Clear();
        var licenseId = id ?? GetNextId();
        var userId = issuerId ?? GetNextId();

        if (!Context.Users.Any(u => u.Id == userId))
        {
            CreateUser(userId);
        }

        if (Context.Licenses.Any(l => l.Id == licenseId))
        {
            return Context.Licenses.First(l => l.Id == licenseId);
        }

        var licenseKey = Guid.NewGuid().ToString("N")[..32];
        var license = new License
        {
            Id = licenseId,
            IssuingDate = DateTime.UtcNow,
            LastUpdatedDate = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            LicenseKey = licenseKey,
            IssuerId = userId
        };
        Context.Licenses.Add(license);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
        return license;
    }

    protected Distributor CreateDistributor(int? id = null, int? licenseId = null, int? coverId = null)
    {
        Context.ChangeTracker.Clear();
        var distributorId = id ?? GetNextId();
        var licId = licenseId ?? GetNextId();
        var covId = coverId ?? GetNextId();

        if (!Context.Licenses.Any(l => l.Id == licId))
        {
            CreateLicense(licId);
        }

        if (!Context.ImageFiles.Any(f => f.Id == covId))
        {
            CreateImageFile(covId);
        }

        if (Context.Distributors.Any(d => d.Id == distributorId))
        {
            return Context.Distributors.First(d => d.Id == distributorId);
        }

        var distributor = new Distributor
        {
            Id = distributorId,
            Name = "Test Distributor",
            CreatedAt = DateTime.UtcNow,
            Description = "Test Description",
            ContactEmail = "test@example.com",
            LicenseId = licId,
            CoverId = covId
        };
        Context.Distributors.Add(distributor);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
        return distributor;
    }

    protected int GetNextId()
    {
        return _nextId++;
    }

    public void Dispose()
    {
        Context?.Dispose();
    }
}

