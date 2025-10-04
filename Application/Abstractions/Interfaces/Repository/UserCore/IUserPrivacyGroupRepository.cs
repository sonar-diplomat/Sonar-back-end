using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Repository.UserCore
{
    public interface IUserPrivacyGroupRepository : IGenericRepository<UserPrivacyGroup>
    {
        Task<UserPrivacyGroup> GetDefaultAsync();
    }
}
