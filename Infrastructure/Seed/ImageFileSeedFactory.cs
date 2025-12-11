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
            },
            new ImageFile
            {
                Id = 2,
                ItemName = "Default playlist",
                // TODO: Add url to default user image
                Url = ""
            },
            new ImageFile
            {
                Id = 3,
                ItemName = "Default track",
                // TODO: Add url to default user image
                Url = ""
            },
            new ImageFile
            {
                Id = 4,
                ItemName = "Default playlist negative",
                // TODO: Add url to default user image
                Url = ""
            },
            new ImageFile
            {
                Id = 5,
                ItemName = "Default track negative",
                // TODO: Add url to default user image
                Url = ""
            },
            // Sticker Images
            new ImageFile { Id = 100, ItemName = "Happy Face Sticker", Url = "image/stickers/happy.png" },
            new ImageFile { Id = 101, ItemName = "Sad Face Sticker", Url = "image/stickers/sad.png" },
            new ImageFile { Id = 102, ItemName = "Angry Face Sticker", Url = "image/stickers/angry.png" },
            new ImageFile { Id = 103, ItemName = "Love Sticker", Url = "image/stickers/love.png" },
            new ImageFile { Id = 104, ItemName = "Like Sticker", Url = "image/stickers/like.png" },
            new ImageFile { Id = 105, ItemName = "Dislike Sticker", Url = "image/stickers/dislike.png" },
            new ImageFile { Id = 106, ItemName = "Thumbs Up Sticker", Url = "image/stickers/thumbs_up.png" },
            new ImageFile { Id = 107, ItemName = "Party Sticker", Url = "image/stickers/party.png" },
            new ImageFile { Id = 108, ItemName = "Celebrate Sticker", Url = "image/stickers/celebrate.png" }
        ];
    }
}