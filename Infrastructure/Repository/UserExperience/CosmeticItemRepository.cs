using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience;

public class CosmeticItemRepository : GenericRepository<CosmeticItem>, ICosmeticItemRepository
{
    public CosmeticItemRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}
