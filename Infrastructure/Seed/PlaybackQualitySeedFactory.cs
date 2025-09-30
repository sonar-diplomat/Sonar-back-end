using Entities.Models.ClientSettings;

namespace Infrastructure.Seed;
public static class PlaybackQualitySeedFactory
{
    public static PlaybackQuality[] CreateSeedData() => new[]
    {
        new PlaybackQuality
        {
            Id = 1,
            Name = "Low",
            BitRate = 128,
            Description = "Low quality playback suitable for slow internet connections"
        },
        new PlaybackQuality
        {
            Id = 2,
            Name = "Medium",
            BitRate = 256,
            Description = "Balanced quality and performance"
        },
        new PlaybackQuality
        {
            Id = 3,
            Name = "High",
            BitRate = 320,
            Description = "High quality playback for premium experience"
        }
    };
}