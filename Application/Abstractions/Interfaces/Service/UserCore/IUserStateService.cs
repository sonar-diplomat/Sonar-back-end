using Application.Abstractions.Interfaces.Service;
using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services;

public interface IUserStateService : IGenericService<UserState>
{
    Task<UserState> CreateDefaultAsync();
}