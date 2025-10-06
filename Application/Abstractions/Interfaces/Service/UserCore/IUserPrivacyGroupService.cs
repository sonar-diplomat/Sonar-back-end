using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IUserPrivacyGroupService : IGenericService<UserPrivacyGroup>
    {
        Task<UserPrivacyGroup> GetDefaultAsync();
    }
}