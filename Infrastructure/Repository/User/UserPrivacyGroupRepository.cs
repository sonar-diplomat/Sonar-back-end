using Application.Abstractions.Interfaces.Repository.UserCore;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.User
{
    public class UserPrivacyGroupRepository(SonarContext dbContext) : GenericRepository<UserPrivacyGroup>(dbContext), IUserPrivacyGroupRepository
    {
        public async Task<UserPrivacyGroup> GetDefaultAsync()
        {
            return (await context.UserPrivacyGroups.FirstOrDefaultAsync(u => u.Id == 1))!;
        }
    }
}
