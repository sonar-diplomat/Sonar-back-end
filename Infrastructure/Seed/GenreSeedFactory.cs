using Entities.Models.Music;

namespace Infrastructure.Seed;

public static class GenreSeedFactory
{
    public static Genre[] CreateSeedData()
    {
        return new[]
        {
            new Genre { Id = 1, Name = "Undefined" },
            new Genre { Id = 2, Name = "Rock" },
            new Genre { Id = 3, Name = "Pop" },
            new Genre { Id = 4, Name = "Hip-Hop" },
            new Genre { Id = 5, Name = "Electronic" },
            new Genre { Id = 6, Name = "Jazz" },
            new Genre { Id = 7, Name = "Classical" },
            new Genre { Id = 8, Name = "Country" },
            new Genre { Id = 9, Name = "R&B" },
            new Genre { Id = 10, Name = "Metal" },
            new Genre { Id = 11, Name = "Folk" }
        };
    }
}

