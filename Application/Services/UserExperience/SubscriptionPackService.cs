using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Application.Exception;
using Entities.Models.UserExperience;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.UserExperience;

public class SubscriptionPackService(
    ISubscriptionPackRepository repository,
    ISubscriptionFeatureService featureService
)
    : GenericService<SubscriptionPack>(repository), ISubscriptionPackService
{
    public async Task<SubscriptionPack> GetOrCreatePackByFeaturesAsync(List<int> featureIds,
        string packName,
        string description,
        double discountMultiplier = 1.0)
    {
        IQueryable<SubscriptionFeature> featuresQuery = (await featureService.GetAllAsync()).AsQueryable();
        List<SubscriptionFeature> features = await featuresQuery
            .Where(f => featureIds.Contains(f.Id))
            .ToListAsync();

        if (features.Count != featureIds.Count)
            throw AppExceptionFactory.Create<BadRequestException>(["One or more features not found."]);

        SubscriptionPack? existingPack = await repository.FindByExactFeatureSetAsync(featureIds);

        if (existingPack != null)
            return existingPack;


        SubscriptionPack newPack = new()
        {
            Name = packName,
            Description = description,
            DiscountMultiplier = discountMultiplier,
            SubscriptionFeatures = features
        };

        await repository.AddAsync(newPack);
        return newPack;
    }
}
