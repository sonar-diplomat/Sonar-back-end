using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface ISubscriptionPaymentService : IGenericService<SubscriptionPayment>
    {
        /// <summary>
        /// Purchase a subscription for yourself
        /// </summary>
        Task<SubscriptionPayment> PurchaseSubscriptionAsync(PurchaseSubscriptionDTO dto);
    }
}
