using Entities.Models.UserExperience;

namespace Infrastructure.Seed;

public static class AchievementCategorySeedFactory
{
    public static AchievementCategory[] CreateSeedData()
    {
        return new[]
        {
            new AchievementCategory
            {
                Id = 1,
                Name = "Listening"
            },
            new AchievementCategory
            {
                Id = 2,
                Name = "Sharing"
            },
            new AchievementCategory
            {
                Id = 3,
                Name = "Collections"
            },
            new AchievementCategory
            {
                Id = 4,
                Name = "Community"
            }
        };
    }
}
