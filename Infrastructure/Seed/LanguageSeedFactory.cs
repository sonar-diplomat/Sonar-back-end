using Entities.Models.ClientSettings;

namespace Infrastructure.Seed;

public static class LanguageSeedFactory
{
    public static Language[] CreateSeedData()
    {
        return new[]
        {
            new Language
            {
                Id = 1,
                Locale = "eng",
                Name = "English",
                NativeName = "English"
            },
            new Language
            {
                Id = 2,
                Locale = "ua",
                Name = "Ukrainian",
                NativeName = "Українська"
            },
            new Language
            {
                Id = 3,
                Locale = "ro",
                Name = "Romanian",
                NativeName = "română"
            }
        };
    }
}