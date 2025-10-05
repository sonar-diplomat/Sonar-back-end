using Entities.Models.ClientSettings;

namespace Infrastructure.Seed;

public static class ThemeSeedFactory
{
    public static Theme[] CreateSeedData()
    {
        return new[]
        {
            new Theme { Id = 1, Name = "Dark" },
            new Theme { Id = 2, Name = "Light" }
        };
    }
}