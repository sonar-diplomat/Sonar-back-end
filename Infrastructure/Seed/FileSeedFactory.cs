using File = Entities.Models.File.File;

namespace Infrastructure.Seed;

public class FileSeedFactory
{
    public static File[] CreateSeedData()
    {
        return new[]
        {
            new File
            {
                Id = 1,
                ItemName = "Default avatar",
                // TODO: Add url to default user image
                Url = "",
                TypeId = 1
            }
        };
    }
}
