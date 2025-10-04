using Entities.Models.UserCore;

namespace Infrastructure.Seed
{
    public static class UserPrivacyGroupSeedFactory
    {
        public static UserPrivacyGroup[] CreateSeedData() => new[]
        {
            new UserPrivacyGroup { Id = 1, Name = "all" },
            new UserPrivacyGroup { Id = 2, Name = "friends" },
            new UserPrivacyGroup { Id = 3, Name = "nobody" },
        };
    }
}