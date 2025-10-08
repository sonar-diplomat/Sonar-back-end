using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Models.UserCore;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience;

public class SubscriptionPaymentService(
    ISubscriptionPaymentRepository repository,
    IUserRepository userRepository,
    AppExceptionFactory appExceptionFactory)
    : GenericService<SubscriptionPayment>(repository), ISubscriptionPaymentService
{
    public async Task<SubscriptionPayment> PurchaseSubscriptionAsync(PurchaseSubscriptionDTO dto)
    {
        // Create the subscription payment
        SubscriptionPayment payment = new()
        {
            BuyerId = dto.UserId,
            SubscriptionPackId = dto.SubscriptionPackId,
            Amount = dto.Amount
        };

        SubscriptionPayment createdPayment = await repository.AddAsync(payment);

        // Activate the subscription for the user
        User? user = await userRepository.GetByIdAsync(dto.UserId);
        if (user == null) throw new NotImplementedException();

        user.SubscriptionPackId = dto.SubscriptionPackId;
        await userRepository.UpdateAsync(user);

        return createdPayment;
    }
}
