using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface ISubscriptionPaymentService
    {
        Task<SubscriptionPayment> GetByIdAsync(int id);
        Task<IEnumerable<SubscriptionPayment>> GetAllAsync();
        Task<SubscriptionPayment> CreateAsync(SubscriptionPayment payment);
        Task<SubscriptionPayment> UpdateAsync(SubscriptionPayment payment);
        Task<bool> DeleteAsync(int id);
    }
}
