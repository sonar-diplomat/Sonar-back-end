using Entities.Models.UserExperience;

namespace Infrastructure.Seed;
public static class GiftStyleSeedFactory
{
    public static GiftStyle[] CreateSeedData() => new[]
    {
        new GiftStyle
        {
            Id = 1,
            Name = "Classic"
        },
        new GiftStyle
        {
            Id = 2,
            Name = "Modern"
        },
        new GiftStyle
        {
            Id = 3,
            Name = "Festive"
        },
        new GiftStyle
        {
            Id = 4,
            Name = "Minimal"
        },
        new GiftStyle
        {
            Id = 5,
            Name = "Luxury"
        }
    };
}