using Entities.Models.ClientSettings;

namespace Infrastructure.Seed;
public static class LanguageSeedFactory
{
    public static Language[] CreateSeedData() => new[]
    {
        new Language
        {
            Id = 1,
            Locale = "rus",
            Name = "Russian",
            NativeName = "Русский"
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
            Locale = "eng",
            Name = "English",
            NativeName = "English"
        }
    };
}