using Entities.Models.Access;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Sonar.Tests.Controllers.Chat;

/// <summary>
/// Вспомогательный класс для настройки тестовых данных
/// </summary>
public static class TestHelper
{
    /// <summary>
    /// Создает тестового пользователя с необходимыми AccessFeatures
    /// </summary>
    public static User CreateTestUser(int id = 1)
    {
        return new User
        {
            Id = id,
            UserName = $"testuser{id}",
            Email = $"test{id}@example.com",
            AccessFeatures = new List<AccessFeature>
            {
                new() { Id = 1, Name = Entities.Enums.AccessFeatureStruct.UserLogin }
            }
        };
    }

    /// <summary>
    /// Создает ClaimsPrincipal для тестового пользователя
    /// </summary>
    public static ClaimsPrincipal CreateClaimsPrincipal(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }

    /// <summary>
    /// Настраивает ControllerContext с ClaimsPrincipal
    /// </summary>
    public static void SetupControllerContext(ControllerBase controller, ClaimsPrincipal user)
    {
        var httpContext = new DefaultHttpContext
        {
            User = user
        };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    /// <summary>
    /// Настраивает UserManager мок для работы с тестовым пользователем
    /// </summary>
    public static void SetupUserManagerMock(Mock<UserManager<User>> userManagerMock, User testUser)
    {
        // Настройка GetUserAsync
        userManagerMock
            .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(testUser);
        
        // Настройка Users - возвращаем пользователя с AccessFeatures
        // Внимание: Include() не будет работать с простым IQueryable,
        // но для unit-тестов контроллера это обычно не критично,
        // так как мы проверяем вызовы методов сервиса, а не работу BaseController
        var usersList = new List<User> { testUser };
        var usersQueryable = usersList.AsQueryable();
        
        userManagerMock
            .Setup(x => x.Users)
            .Returns(usersQueryable);
    }
}

