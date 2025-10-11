using Entities.Models.File;

namespace Infrastructure.Seed;

public static class FileTypeSeedFactory
{
    public static FileType[] CreateSeedData()
    {
        return new[]
        {
            new FileType
            {
                Id = 1,
                Name = "image"
            },
            new FileType
            {
                Id = 2,
                Name = "audio"
            },
            new FileType
            {
                Id = 3,
                Name = "gif"
            }
        };
    }
}
