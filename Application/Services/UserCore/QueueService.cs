using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserCore;

namespace Application.Services.UserCore;

public class QueueService(
    IQueueRepository repository
)
    : GenericService<Queue>(repository), IQueueService
{
}