using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Auth;
using Application.Exception;
using Entities.Models.UserCore;

namespace Application.Services.UserCore;

public class UserSessionService(IUserSessionRepository repository)
    : GenericService<UserSession>(repository), IUserSessionService
{
    public async Task<UserSession?> GetByRefreshTokenAsync(string refreshHash)
    {
        return await repository.GetByRefreshToken(refreshHash);
    }

    public async Task<UserSession> GetValidatedByRefreshTokenAsync(string refreshHash)
    {
        UserSession? userSession = await repository.GetByRefreshToken(refreshHash);
        if (userSession == null)
            AppExceptionFactory.Create<NotFoundException>();
        return userSession!;
    }

    public async Task UpdateLastActiveAsync(UserSession session)
    {
        session.LastActive = DateTime.UtcNow;
        await repository.UpdateAsync(session);
    }

    public async Task RevokeSessionAsync(UserSession session)
    {//TODO ASK ZAHAR WHY WE DONT DELETE SESSION
        session.Revoked = true;
        await repository.UpdateAsync(session);
    }

    public async Task RevokeAllUserSessionsAsync(int userId)
    {
        IEnumerable<UserSession> sessions = await repository.GetAllByUserIdAsync(userId);

        foreach (UserSession session in sessions)
        {
            session.Revoked = true;
            await repository.UpdateAsync(session);
        }
    }
    
    public async Task<IEnumerable<ActiveUserSessionDTO>> GetAllByUserIdAsync(int userId)
    {
        return await repository.GetAllActiveSessionsByUserIdAsync(userId);
    }
}
