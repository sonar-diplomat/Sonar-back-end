using Application.DTOs;
using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Repository.UserCore;

public interface IUserSessionRepository : IGenericRepository<UserSession>
{
    public Task<UserSession?> GetByRefreshToken(string refreshHash);
    public Task<IEnumerable<UserSession>> GetAllByUserIdAsync(int userId);
    public Task<IEnumerable<ActiveSessionDTO>> GetAllActiveSessionsByUserIdAsync(int userId);
}