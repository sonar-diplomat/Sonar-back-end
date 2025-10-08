using Entities.Models.UserCore;

namespace Infrastructure.Seed;

public static class UserStatusSeedFactory
{
    public static UserStatus[] CreateSeedData()
    {
        return new[]
        {
            new UserStatus { Id = 1, Name = "online" },
            new UserStatus { Id = 2, Name = "offline" },
            new UserStatus { Id = 3, Name = "do not disturb" },
            new UserStatus { Id = 4, Name = "idle" }
        };
    }
}
