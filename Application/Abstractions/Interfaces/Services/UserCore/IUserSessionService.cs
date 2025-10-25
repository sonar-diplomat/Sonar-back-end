using Application.DTOs;
using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services;

public interface IUserSessionService : IGenericService<UserSession>
{
    public Task<UserSession?> GetByRefreshTokenAsync(string refreshHash);

    public Task<UserSession> GetValidatedByRefreshTokenAsync(string refreshHash);

    public Task UpdateLastActiveAsync(UserSession session);

    public Task RevokeSessionAsync(UserSession session);

    public Task RevokeAllUserSessionsAsync(int userId);

    public Task<IEnumerable<ActiveSessionDTO>> GetAllByUserIdAsync(int userId);
}