using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Repository.UserExperience;

public interface IGiftRepository : IGenericRepository<Gift>
{
    Task<IQueryable<Gift>> GetAllByReceiverAsync(int receiverId);
}
