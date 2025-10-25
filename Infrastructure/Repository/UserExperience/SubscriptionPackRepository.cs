using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models.UserExperience;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.UserExperience;

public class SubscriptionPackRepository(SonarContext dbContext) : GenericRepository<SubscriptionPack>(dbContext), ISubscriptionPackRepository
{
    public override async Task<SubscriptionPack?> GetByIdAsync(int? id)
    {
        return await context.Set<SubscriptionPack>().Include(s => s.SubscriptionFeatures).FirstOrDefaultAsync(sp => sp.Id == id);
    }

    public override async Task<IQueryable<SubscriptionPack>> GetAllAsync()
    {
        return await Task.FromResult(context.Set<SubscriptionPack>().Include(s => s.SubscriptionFeatures).AsQueryable());
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
