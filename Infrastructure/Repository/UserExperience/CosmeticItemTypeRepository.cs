using Application.Abstractions.Interfaces.Repository.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class CosmeticItemTypeRepository : GenericRepository<Entities.Models.CosmeticItemType>, ICosmeticItemTypeRepository
    {
        public CosmeticItemTypeRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
