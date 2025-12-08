using Entities.Models.Chat;

namespace Infrastructure.Seed;

public static class ChatStickerSeedFactory
{
    public static ChatSticker[] CreateSeedData()
    {
        return new[]
        {
            // Emotions category
            new ChatSticker
            {
                Id = 1,
                Name = "Happy",
                ImageFileId = 100, // TODO: Replace with actual ImageFile ID after creating sticker images
                CategoryId = 1
            },
            new ChatSticker
            {
                Id = 2,
                Name = "Sad",
                ImageFileId = 101,
                CategoryId = 1
            },
            new ChatSticker
            {
                Id = 3,
                Name = "Angry",
                ImageFileId = 102,
                CategoryId = 1
            },
            new ChatSticker
            {
                Id = 4,
                Name = "Love",
                ImageFileId = 103,
                CategoryId = 1
            },
            // Reactions category
            new ChatSticker
            {
                Id = 5,
                Name = "Like",
                ImageFileId = 104,
                CategoryId = 2
            },
            new ChatSticker
            {
                Id = 6,
                Name = "Dislike",
                ImageFileId = 105,
                CategoryId = 2
            },
            new ChatSticker
            {
                Id = 7,
                Name = "Thumbs Up",
                ImageFileId = 106,
                CategoryId = 2
            },
            // Fun category
            new ChatSticker
            {
                Id = 8,
                Name = "Party",
                ImageFileId = 107,
                CategoryId = 3
            },
            new ChatSticker
            {
                Id = 9,
                Name = "Celebrate",
                ImageFileId = 108,
                CategoryId = 3
            }
        };
    }
}

