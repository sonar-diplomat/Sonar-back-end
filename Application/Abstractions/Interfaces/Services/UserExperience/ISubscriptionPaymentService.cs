using Application.DTOs;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services;

public interface ISubscriptionPaymentService : IGenericService<SubscriptionPayment>
{
    Task<SubscriptionPayment> PurchaseSubscriptionAsync(PurchaseSubscriptionDTO dto);
}
