using Application.Abstractions.Interfaces.Repository.Access;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Access;

namespace Application.Services.Access;

public class SuspensionService(ISuspensionRepository repository)
    : GenericService<Suspension>(repository), ISuspensionService
{
}