using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface ISubscriptionFeatureService
    {
        Task<SubscriptionFeature> GetByIdAsync(int id);
        Task<IEnumerable<SubscriptionFeature>> GetAllAsync();
        Task<SubscriptionFeature> CreateAsync(SubscriptionFeature feature);
        Task<SubscriptionFeature> UpdateAsync(SubscriptionFeature feature);
        Task<bool> DeleteAsync(int id);
    }
}