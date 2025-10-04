using Application.Abstractions.Interfaces.Repository.Access;
using Entities.Enums;
using Entities.Models.Access;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Access
{
    public class AccessFeatureRepository : GenericRepository<AccessFeature>, IAccessFeatureRepository
    {

        public AccessFeatureRepository(SonarContext dbContext) : base(dbContext)
        {
        }

        public async Task<ICollection<AccessFeature>> GetDefaultAsync()
        {
            List<AccessFeature> features = new();
            string[] names = new string[]
            {
                AccessFeatureStruct.SendFriendRequest,
                AccessFeatureStruct.SendMessage,
                AccessFeatureStruct.ListenContent,
                AccessFeatureStruct.UserLogin,
                AccessFeatureStruct.ReportContent
            };
            features = await context.AccessFeatures
                .Where(af => names.Contains(af.Name))
                .ToListAsync();

            return features;
        }
    }
}
