using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserCore;

namespace Application.Services.UserCore;

public class UserSessionService(IUserSessionRepository repository)
    : GenericService<UserSession>(repository), IUserSessionService
{
}