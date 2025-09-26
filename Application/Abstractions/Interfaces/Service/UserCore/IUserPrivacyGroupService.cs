using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IUserPrivacyGroupService
    {
        Task<UserPrivacyGroup> GetByIdAsync(int id);
        Task<IEnumerable<UserPrivacyGroup>> GetAllAsync();
        Task<UserPrivacyGroup> CreateAsync(UserPrivacyGroup privacyGroup);
        Task<UserPrivacyGroup> UpdateAsync(UserPrivacyGroup privacyGroup);
        Task<bool> DeleteAsync(int id);
    }
}