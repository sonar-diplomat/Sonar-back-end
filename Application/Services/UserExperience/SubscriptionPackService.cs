using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience;

public class SubscriptionPackService(ISubscriptionPackRepository repository)
    : GenericService<SubscriptionPack>(repository), ISubscriptionPackService
{
}
