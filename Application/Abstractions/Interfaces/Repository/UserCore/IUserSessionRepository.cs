using Application.DTOs.Auth;
using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Repository.UserCore;

public interface IUserSessionRepository : IGenericRepository<UserSession>
{
    public Task<UserSession?> GetByRefreshToken(string refreshHash);
    public Task<IEnumerable<UserSession>> GetAllByUserIdAsync(int userId);
    public Task<IEnumerable<ActiveSessionDTO>> GetAllActiveSessionsByUserIdAsync(int userId);
    Task<UserSession?> GetByUserIdAndDeviceIdAsync(int userId, string deviceId);
}