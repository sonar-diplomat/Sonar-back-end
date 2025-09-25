using Application.Abstractions.Interfaces.Repository.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class GiftStyleRepository : GenericRepository<Entities.Models.GiftStyle>, IGiftStyleRepository
    {
        public GiftStyleRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
