using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserCore;

namespace Application.Services.UserCore;

public class UserSessionService(IUserSessionRepository repository) : IUserSessionService
{
    public Task<UserSession> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserSession>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserSession> CreateAsync(UserSession entity)
    {
        throw new NotImplementedException();
    }

    public Task<UserSession> UpdateAsync(UserSession entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}