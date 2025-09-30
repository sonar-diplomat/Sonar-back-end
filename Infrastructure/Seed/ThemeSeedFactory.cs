using Entities.Models.ClientSettings;

namespace Infrastructure.Seed;

public static class ThemeSeedFactory
{
    public static Theme[] CreateSeedData() => new[]
    {
        new Theme { Id = 1, Name = "Light" },
        new Theme { Id = 2, Name = "Dark" }
    };
}


