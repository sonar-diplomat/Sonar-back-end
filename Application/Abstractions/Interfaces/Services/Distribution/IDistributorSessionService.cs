using Application.DTOs;
using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Services;

public interface IDistributorSessionService : IGenericService<DistributorSession>
{
    Task<IEnumerable<DistributorSession>?> GetByDistributorAccountIdAsync(int id);
    Task<DistributorSession?> CreateSessionAsync(SessionDTO dto);
    Task<DistributorSession> UpdateSessionAsync(int id, SessionDTO dto);
    Task<DistributorSession> GetValidatedByRefreshTokenAsync(string refreshHash);
    Task TerminateSessionAsync(int id);
    Task<DistributorSession?> GetByRefreshTokenAsync(string refreshHash);
    Task UpdateLastActiveAsync(DistributorSession session);
    Task RevokeSessionAsync(DistributorSession session);
    Task RevokeAllDistributorSessionsAsync(int accountId);
    Task<IEnumerable<ActiveSessionDTO>> GetAllByUserIdAsync(int accountId);
}
