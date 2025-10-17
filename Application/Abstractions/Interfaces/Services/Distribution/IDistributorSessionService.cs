using Application.DTOs;
using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Services;

public interface IDistributorSessionService : IGenericService<DistributorSession>
{
    Task<IEnumerable<DistributorSession>?> GetByDistributorAccountIdAsync(int id);
    Task<DistributorSession?> CreateSessionAsync(SessionDTO dto);
    Task<DistributorSession> UpdateSessionAsync(int id, SessionDTO dto);
    Task TerminateSessionAsync(int id);
}
