using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class GiftStyleRepository : GenericRepository<GiftStyle>, IGiftStyleRepository
    {
        public GiftStyleRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
