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
            new ImageFile
            {
                Id = 6,
                ItemName = "Favorites folder",
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
            new ImageFile { Id = 108, ItemName = "Celebrate Sticker", Url = "image/stickers/celebrate.png" },

            new ImageFile { Id = 109, ItemName = "Boom Sticker", Url = "image/stickers/9.png" },
            new ImageFile { Id = 110, ItemName = "Bomb Sticker", Url = "image/stickers/Boom-1.png" },
            new ImageFile { Id = 111, ItemName = "CDC Sticker", Url = "image/stickers/CDC.png" },
            new ImageFile { Id = 112, ItemName = "Chengdu Sticker", Url = "image/stickers/Chengdu.png" },
            new ImageFile { Id = 113, ItemName = "Chilis Sticker", Url = "image/stickers/Chilis.png" },
            new ImageFile { Id = 114, ItemName = "Chill Sticker", Url = "image/stickers/Chill.png" },
            new ImageFile { Id = 115, ItemName = "Comfortable Sticker", Url = "image/stickers/Comfortable.png" },
            new ImageFile { Id = 116, ItemName = "Eyes Sticker", Url = "image/stickers/Eyes.png" },

            new ImageFile { Id = 117, ItemName = "Fa Cai Sticker", Url = "image/stickers/Fa Cai.png" },
            new ImageFile { Id = 118, ItemName = "Figma Sticker", Url = "image/stickers/Figma.png" },
            new ImageFile { Id = 119, ItemName = "Great Sticker", Url = "image/stickers/Great.png" },
            new ImageFile { Id = 120, ItemName = "Hand Sticker", Url = "image/stickers/Hand.png" },
            new ImageFile { Id = 121, ItemName = "Hotpot Sticker", Url = "image/stickers/Hotpot.png" },
            new ImageFile { Id = 122, ItemName = "OKOK Sticker", Url = "image/stickers/OKOK.png" },
            new ImageFile { Id = 123, ItemName = "Oops Sticker", Url = "image/stickers/Oops.png" },
            new ImageFile { Id = 124, ItemName = "Rainbow Sticker", Url = "image/stickers/Rainbow.png" },

            new ImageFile { Id = 125, ItemName = "Smile Sticker", Url = "image/stickers/Smile.png" },
            new ImageFile { Id = 126, ItemName = "Spark Sticker", Url = "image/stickers/Spark.png" },
            new ImageFile { Id = 127, ItemName = "Spark Alt Sticker", Url = "image/stickers/Spark-1.png" },
            new ImageFile { Id = 128, ItemName = "Taen Sticker", Url = "image/stickers/Taen.png" },
            new ImageFile { Id = 129, ItemName = "Tea Sticker", Url = "image/stickers/Tea.png" },
            new ImageFile { Id = 130, ItemName = "Wave Sticker", Url = "image/stickers/Wave.png" },
            new ImageFile { Id = 131, ItemName = "Windmill Sticker", Url = "image/stickers/Windmill.png" },
            new ImageFile { Id = 132, ItemName = "Yaoji Sticker", Url = "image/stickers/Yaoji.png" },

            new ImageFile { Id = 133, ItemName = "Panda Sticker", Url = "image/stickers/Zhong.png" },
            new ImageFile { Id = 134, ItemName = "Zhong Sticker", Url = "image/stickers/Zhong-1.png" }

        ];
    }
}