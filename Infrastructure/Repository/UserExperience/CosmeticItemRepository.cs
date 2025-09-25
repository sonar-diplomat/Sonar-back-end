using Application.Abstractions.Interfaces.Repository.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class CosmeticItemRepository : GenericRepository<Entities.Models.CosmeticItem>, ICosmeticItemRepository
    {
        public CosmeticItemRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
