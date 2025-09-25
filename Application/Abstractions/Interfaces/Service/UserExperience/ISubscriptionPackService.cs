using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface ISubscriptionPackService
    {
        Task<SubscriptionPack> GetByIdAsync(int id);
        Task<IEnumerable<SubscriptionPack>> GetAllAsync();
        Task<SubscriptionPack> CreateAsync(SubscriptionPack pack);
        Task<SubscriptionPack> UpdateAsync(SubscriptionPack pack);
        Task<bool> DeleteAsync(int id);
    }
}

