using Entities.Models.Music;

namespace Infrastructure.Seed;

public static class GenreSeedFactory
{
    public static Genre[] CreateSeedData()
    {
        return new[]
        {
            new Genre { Id = 1, Name = "Rock" },
            new Genre { Id = 2, Name = "Pop" },
            new Genre { Id = 3, Name = "Hip-Hop" },
            new Genre { Id = 4, Name = "Electronic" },
            new Genre { Id = 5, Name = "Jazz" },
            new Genre { Id = 6, Name = "Classical" },
            new Genre { Id = 7, Name = "Country" },
            new Genre { Id = 8, Name = "R&B" },
            new Genre { Id = 9, Name = "Metal" },
            new Genre { Id = 10, Name = "Folk" }
        };
    }
}

