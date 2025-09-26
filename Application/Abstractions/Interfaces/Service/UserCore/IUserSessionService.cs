using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IUserSessionService
    {
        Task<UserSession> GetByIdAsync(int id);
        Task<IEnumerable<UserSession>> GetAllAsync();
        Task<UserSession> CreateAsync(UserSession session);
        Task<UserSession> UpdateAsync(UserSession session);
        Task<bool> DeleteAsync(int id);
    }
}