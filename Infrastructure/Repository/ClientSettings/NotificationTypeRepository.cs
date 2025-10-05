using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models.ClientSettings;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Client;

public class NotificationTypeRepository : GenericRepository<NotificationType>, INotificationTypeRepository
{
    public NotificationTypeRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}