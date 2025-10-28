using Entities.Models.File;

namespace Infrastructure.Seed;

public class ImageFileSeedFactory
{
    public static ImageFile[] CreateSeedData()
    {
        return
        [
            new ImageFile
            {
                Id = 1,
                ItemName = "Default avatar",
                // TODO: Add url to default user image
                Url = ""
            }
        ];
    }
}