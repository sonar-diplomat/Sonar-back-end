using Entities.Models.ClientSettings;

namespace Infrastructure.Seed;

public static class NotificationTypeSeedFactory
{
    public static NotificationType[] CreateSeedData()
    {
        return new[]
        {
            new NotificationType
            {
                Id = 1,
                Name = "Message",
                Description = "Notification about a new message from another user"
            },
            new NotificationType
            {
                Id = 2,
                Name = "FriendRequest",
                Description = "Notification about a new friend request"
            },
            new NotificationType
            {
                Id = 3,
                Name = "SystemAlert",
                Description = "Notification about system updates or alerts"
            },
            new NotificationType
            {
                Id = 4,
                Name = "Promotion",
                Description = "Notification about promotions or special offers"
            }
        };
    }
}
