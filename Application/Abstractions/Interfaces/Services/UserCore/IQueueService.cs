﻿using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services;

public interface IQueueService : IGenericService<Queue>
{
    Task AddTracksToQueueAsync(int queueId, IEnumerable<int> trackIds);
    Task RemoveTracksFromQueueAsync(int queueId, IEnumerable<int> trackIds);
    Task<Queue> GetQueueWithTracksAsync(int queueId);
    Task SaveQueueAsync(int queueId, IEnumerable<int> trackIds);
}