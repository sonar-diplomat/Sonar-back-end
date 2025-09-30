using Entities.Models.Access;

namespace Infrastructure.Seed
{
    public static class AccessFeatureSeedFactory
    {
        public static AccessFeature[] CreateSeedData() => new[]
        {
            new AccessFeature
            {
                Id = 1,
                Name = "IamAGod"
            },
            // Social
            new AccessFeature
            {
                Id = 2,
                Name = "SendFriendRequest"
            },
            new AccessFeature
            {
                Id = 3,
                Name = "SendMessage"
            },
            new AccessFeature
            {
                Id = 4,
                Name = "ReportContent"
            },
            // Content
            new AccessFeature
            {
                Id = 5,
                Name = "ListenContent"
            },
            // Access
            new AccessFeature
            {
                Id = 6,
                Name = "UserLogin"
            },
            // Moderate
            new AccessFeature
            {
                Id = 7,
                Name = "ManageUsers"
            },
            new AccessFeature
            {
                Id = 8,
                Name = "ManageContent"
            },
            new AccessFeature
            {
                Id = 9,
                Name = "ManageDistributors"
            },
            new AccessFeature
            {
                Id = 10,
                Name = "ManageReports"
            },
            // Artist
            new AccessFeature
            {
                Id = 11,
                Name = "CreatePost"
            }
        };
    }
}
