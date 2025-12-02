using Entities.Models.Access;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Sonar.Tests.Controllers.Chat;

/// <summary>
/// Базовый класс для настройки тестов с реальным DbContext
/// </summary>
public abstract class ChatControllerTestsBase
{
    protected SonarContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<SonarContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new SonarContext(options);
    }

    protected UserManager<User> CreateUserManager(SonarContext context)
    {
        var userStore = new UserStore<User, IdentityRole<int>, SonarContext, int>(context);
        var userManager = new UserManager<User>(
            userStore,
            null!, // IOptions<IdentityOptions>
            new PasswordHasher<User>(),
            Array.Empty<IUserValidator<User>>(),
            Array.Empty<IPasswordValidator<User>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            new ServiceCollection().BuildServiceProvider(),
            null! // ILogger<UserManager<User>>
        );
        
        return userManager;
    }

    protected User CreateTestUser(int id = 1)
    {
        return new User
        {
            Id = id,
            UserName = $"testuser{id}",
            Email = $"test{id}@example.com",
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = new DateOnly(2000, 1, 1),
            Login = $"testuser{id}",
            PublicIdentifier = $"testuser{id}",
            AvailableCurrency = 0,
            RegistrationDate = DateTime.UtcNow,
            Enabled2FA = false,
            AvatarImageId = 1,
            VisibilityStateId = 1,
            UserStateId = 1,
            SettingsId = 1,
            InventoryId = 1,
            LibraryId = 1,
            AccessFeatures = new List<AccessFeature>
            {
                new() { Id = 1, Name = Entities.Enums.AccessFeatureStruct.UserLogin }
            }
        };
    }

    protected ClaimsPrincipal CreateClaimsPrincipal(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }
}

