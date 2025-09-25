using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.User;

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