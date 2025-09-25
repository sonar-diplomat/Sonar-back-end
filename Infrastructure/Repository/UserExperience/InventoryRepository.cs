using Application.Abstractions.Interfaces.Repository.UserExperience;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserExperience
{
    public class InventoryRepository : GenericRepository<Entities.Models.Inventory>, IInventoryRepository
    {
        public InventoryRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
