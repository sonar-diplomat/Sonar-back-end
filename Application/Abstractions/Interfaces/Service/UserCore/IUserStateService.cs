using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IUserStateService
    {
        Task<UserState> GetByIdAsync(int id);
        Task<IEnumerable<UserState>> GetAllAsync();
        Task<UserState> CreateAsync(UserState state);
        Task<UserState> UpdateAsync(UserState state);
        Task<bool> DeleteAsync(int id);
    }
}

