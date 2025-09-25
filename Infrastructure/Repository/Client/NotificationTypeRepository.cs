using Application.Abstractions.Interfaces.Repository.Client;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Client
{
    public class NotificationTypeRepository : GenericRepository<Entities.Models.NotificationType>, INotificationTypeRepository
    {
        public NotificationTypeRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
