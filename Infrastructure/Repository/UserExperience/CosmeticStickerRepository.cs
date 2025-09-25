using Application.Abstractions.Interfaces.Repository.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class CosmeticStickerRepository : GenericRepository<Entities.Models.CosmeticSticker>, ICosmeticStickerRepository
    {
        public CosmeticStickerRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
