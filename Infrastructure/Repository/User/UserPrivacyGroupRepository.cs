using Application.Abstractions.Interfaces.Repository.UserCore;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.UserCore
{
    public class UserPrivacyGroupRepository(SonarContext dbContext) : GenericRepository<UserPrivacyGroup>(dbContext), IUserPrivacyGroupRepository
    {
        public async Task<UserPrivacyGroup> GetDefaultAsync()
        {
            return (await context.UserPrivacyGroups.FirstOrDefaultAsync(u => u.Id == 1))!;
        }
    }
}
