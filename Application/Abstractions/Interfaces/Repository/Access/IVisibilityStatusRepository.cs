using Entities.Models.Access;

namespace Application.Abstractions.Interfaces.Repository.Access;

public interface IVisibilityStatusRepository : IGenericRepository<VisibilityStatus>
{
    Task<VisibilityStatus> GetDefaultAsync();
}