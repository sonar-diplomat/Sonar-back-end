using Entities.Models.Distribution;

namespace Application.Abstractions.Interfaces.Repository.Distribution;

public interface IDistributorAccountRepository : IGenericRepository<DistributorAccount>
{
    public Task<DistributorAccount?> GetByEmailAsync(string email);

    public Task<bool> ExistsByEmailAsync(string email);
    Task<IEnumerable<DistributorAccount>> GetAllByDistributorId(int id);
}
