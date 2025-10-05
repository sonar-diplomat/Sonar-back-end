using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience;

public class CosmeticItemTypeRepository : GenericRepository<CosmeticItemType>, ICosmeticItemTypeRepository
{
    public CosmeticItemTypeRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}