using Entities.Models.UserExperience;

namespace Infrastructure.Seed
{
    public static class SubscribtionFeatureSeedFactory
    {
        public static SubscriptionFeature[] CreateSeedData()
        {
            return new[] {
                new SubscriptionFeature {
                    Id = 1,
                    Name = "High Quality Audio",
                    Description = "Enjoy crystal-clear, high-bitrate sound for the best listening experience.",
                    Price = 100,
                    SubscriptionPacks = new List<SubscriptionPack>()
                },
                new SubscriptionFeature {
                    Id = 2,
                    Name = "Music Download",
                    Description = "Download your favorite tracks and listen offline anytime, anywhere.",
                    Price = 100,
                    SubscriptionPacks = new List<SubscriptionPack>()
                },
                new SubscriptionFeature {
                    Id = 3,
                    Name = "Seasonal Skins",
                    Description = "Unlock exclusive visual themes and profile skins available only during special seasons or events.",
                    Price = 100,
                    SubscriptionPacks = new List<SubscriptionPack>()
                },
                new SubscriptionFeature {
                    Id = 4,
                    Name = "Animated GIF Avatars",
                    Description = "Set animated GIFs as your profile avatar to stand out from the crowd (feature under review).",
                    Price = 100,
                    SubscriptionPacks = new List<SubscriptionPack>()
                },
            };
        }
    }
}
