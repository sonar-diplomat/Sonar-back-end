using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Extensions;
using Entities.Models.UserExperience;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.UserExperience;

public class SubscriptionPackRepository(SonarContext dbContext) : GenericRepository<SubscriptionPack>(dbContext), ISubscriptionPackRepository
{
    public override async Task<SubscriptionPack?> GetByIdAsync(int? id)
    {
        return await RepositoryIncludeExtensions.SnInclude(context.Set<SubscriptionPack>(), s => s.SubscriptionFeatures).FirstOrDefaultAsync(sp => sp.Id == id);
    }

    public override async Task<IQueryable<SubscriptionPack>> GetAllAsync()
    {
        return await Task.FromResult(RepositoryIncludeExtensions.SnInclude(context.Set<SubscriptionPack>(), s => s.SubscriptionFeatures).AsQueryable());
    }
    public async Task<SubscriptionPack?> FindByExactFeatureSetAsync(List<int> featureIds)
    {
        return await RepositoryIncludeExtensions.SnInclude(context.Set<SubscriptionPack>(), p => p.SubscriptionFeatures)
            .Where(p =>
                p.SubscriptionFeatures.Count == featureIds.Count &&
                p.SubscriptionFeatures.All(f => featureIds.Contains(f.Id)) &&
                featureIds.All(id => p.SubscriptionFeatures.Select(sf => sf.Id).Contains(id))
            )
            .FirstOrDefaultAsync();
    }
}
