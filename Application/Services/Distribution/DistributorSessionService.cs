using System.Net;
using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Models.Distribution;

namespace Application.Services.Distribution;

public class DistributorSessionService(IDistributorSessionRepository repository, IDistributorAccountService distributorAccountService)
    : GenericService<DistributorSession>(repository), IDistributorSessionService
{
    public async Task<IEnumerable<DistributorSession>?> GetByDistributorAccountIdAsync(int id)
    {
        DistributorAccount account = await distributorAccountService.GetByIdValidatedAsync(id);
        return account.Sessions;
    }

    public async Task<DistributorSession> GetValidatedByRefreshTokenAsync(string refreshHash)
    {
        DistributorSession? userSession = await repository.GetByRefreshTokenAsync(refreshHash);
        if (userSession == null)
            AppExceptionFactory.Create<NotFoundException>();
        return userSession!;
    }

    public async Task<DistributorSession?> CreateSessionAsync(SessionDTO dto)
    {
        if (dto.IpAddress == null || dto.UserAgent == null || dto.DeviceName == null)
            throw AppExceptionFactory.Create<BadRequestException>(["Cant create session."]);

        DistributorSession session = new()
        {
            IPAddress = IPAddress.Parse(dto.IpAddress),
            UserAgent = dto.UserAgent,
            DeviceName = dto.DeviceName,
            LastActive = DateTime.UtcNow,
            DistributorAccount = await distributorAccountService.GetByIdValidatedAsync(dto.DistributorId)
        };

        return await repository.AddAsync(session);
    }

    public async Task<DistributorSession> UpdateSessionAsync(int id, SessionDTO dto)
    {
        DistributorSession session = await GetByIdValidatedAsync(id);
        session.IPAddress = dto.IpAddress != null ? IPAddress.Parse(dto.IpAddress) : session.IPAddress;
        session.UserAgent = dto.UserAgent ?? session.UserAgent;
        session.DeviceName = dto.DeviceName ?? session.DeviceName;
        session.LastActive = DateTime.UtcNow;
        return await repository.UpdateAsync(session);
    }

    public async Task TerminateSessionAsync(int id)
    {
        DistributorSession session = await GetByIdValidatedAsync(id);
        await repository.RemoveAsync(session);
    }

    public async Task<DistributorSession?> GetByRefreshTokenAsync(string refreshHash)
    {
        return await repository.GetByRefreshTokenAsync(refreshHash);
    }

    public async Task UpdateLastActiveAsync(DistributorSession session)
    {
        session.LastActive = DateTime.UtcNow;
        await repository.UpdateAsync(session);
    }

    public async Task RevokeSessionAsync(DistributorSession session)
    {
        session.Revoked = true;
        await repository.UpdateAsync(session);
    }

    public async Task RevokeAllDistributorSessionsAsync(int accountId)
    {
        IEnumerable<DistributorSession> sessions = await repository.GetAllByDistributorIdAsync(accountId);

        foreach (DistributorSession session in sessions)
        {
            session.Revoked = true;
            await repository.UpdateAsync(session);
        }
    }

    public async Task<IEnumerable<ActiveSessionDTO>> GetAllByUserIdAsync(int accountId)
    {
        return await repository.GetAllActiveSessionsByDistributorIdAsync(accountId);
    }
}
