using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class CosmeticStickerRepository : GenericRepository<CosmeticSticker>, ICosmeticStickerRepository
    {
        public CosmeticStickerRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
