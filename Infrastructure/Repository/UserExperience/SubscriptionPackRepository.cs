using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models.UserExperience;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.UserExperience;

public class SubscriptionPackRepository : GenericRepository<SubscriptionPack>, ISubscriptionPackRepository
{
    public SubscriptionPackRepository(SonarContext dbContext) : base(dbContext)
    {
        
    }
    public async Task<SubscriptionPack?> FindByExactFeatureSetAsync(List<int> featureIds)
    {
        return await context.Set<SubscriptionPack>()
            .Include(p => p.SubscriptionFeatures)
            .Where(p =>
                p.SubscriptionFeatures.Count == featureIds.Count &&
                p.SubscriptionFeatures.All(f => featureIds.Contains(f.Id)) &&
                featureIds.All(id => p.SubscriptionFeatures.Select(sf => sf.Id).Contains(id))
            )
            .FirstOrDefaultAsync();
    }
}
