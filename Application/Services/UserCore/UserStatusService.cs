using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserCore;

namespace Application.Services.UserCore;

public class UserStatusService(IUserStatusRepository repository) : IUserStatusService
{
    public Task<UserStatus> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserStatus>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserStatus> CreateAsync(UserStatus entity)
    {
        throw new NotImplementedException();
    }

    public Task<UserStatus> UpdateAsync(UserStatus entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}