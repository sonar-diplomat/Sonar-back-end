using Entities.Models.Chat;

namespace Infrastructure.Seed;

public static class ChatStickerCategorySeedFactory
{
    public static ChatStickerCategory[] CreateSeedData()
    {
        return new[]
        {
            new ChatStickerCategory
            {
                Id = 1,
                Name = "Emotions"
            },
            new ChatStickerCategory
            {
                Id = 2,
                Name = "Reactions"
            },
            new ChatStickerCategory
            {
                Id = 3,
                Name = "Fun"
            }
        };
    }
}

