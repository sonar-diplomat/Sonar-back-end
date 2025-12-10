using Entities.Models.Music;

namespace Infrastructure.Seed;

public static class MoodTagSeedFactory
{
    public static MoodTag[] CreateSeedData()
    {
        return new[]
        {
            new MoodTag { Id = 1, Name = "Happy" },
            new MoodTag { Id = 2, Name = "Sad" },
            new MoodTag { Id = 3, Name = "Energetic" },
            new MoodTag { Id = 4, Name = "Relaxed" },
            new MoodTag { Id = 5, Name = "Melancholic" },
            new MoodTag { Id = 6, Name = "Upbeat" },
            new MoodTag { Id = 7, Name = "Calm" },
            new MoodTag { Id = 8, Name = "Aggressive" },
            new MoodTag { Id = 9, Name = "Romantic" },
            new MoodTag { Id = 10, Name = "Nostalgic" }
        };
    }
}

