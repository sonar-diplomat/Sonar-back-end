using Application.Abstractions.Interfaces.Repository.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class SubscriptionPaymentRepository : GenericRepository<Entities.Models.SubscriptionPayment>, ISubscriptionPaymentRepository
    {
        public SubscriptionPaymentRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
