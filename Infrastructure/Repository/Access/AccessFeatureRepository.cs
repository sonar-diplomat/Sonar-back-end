using Application.Abstractions.Interfaces.Repository.Access;
using Application.Response;
using Entities.Enums;
using Entities.Models.Access;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Access;

public class AccessFeatureRepository(SonarContext dbContext)
    : GenericRepository<AccessFeature>(dbContext), IAccessFeatureRepository
{
    public async Task<ICollection<AccessFeature>> GetDefaultAsync()
    {
        string[] names =
        [
            AccessFeatureStruct.SendFriendRequest,
            AccessFeatureStruct.SendMessage,
            AccessFeatureStruct.ListenContent,
            AccessFeatureStruct.UserLogin,
            AccessFeatureStruct.ReportContent
        ];
        List<AccessFeature> features = await context.AccessFeatures
            .Where(af => names.Contains(af.Name))
            .ToListAsync();

        return features;
    }

    public async Task<ICollection<AccessFeature>?> GetUserFeaturesByIdAsync(int userId)
    {
        Entities.Models.UserCore.User? user = (await context.Users.Include(u => u.AccessFeatures).FirstOrDefaultAsync(u => u.Id == userId));
        return user?.AccessFeatures;
    }

    public async Task<AccessFeature> GetByNameValidatedAsync(string name)
    {
        AccessFeature? accessFeature = await context.AccessFeatures.FirstOrDefaultAsync(af => af.Name == name);
        return accessFeature ?? throw ResponseFactory.Create<NotFoundResponse>([$"Access feature with name {name} not found."]);
    }
}
