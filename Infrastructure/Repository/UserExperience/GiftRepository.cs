using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience;

public class GiftRepository(SonarContext dbContext) : GenericRepository<Gift>(dbContext), IGiftRepository
{
    public async Task<IQueryable<Gift>> GetAllByReceiverAsync(int receiverId)
    {
        return await Task.FromResult(dbContext.Gifts.Where(g => g.ReceiverId == receiverId));
    }
}
