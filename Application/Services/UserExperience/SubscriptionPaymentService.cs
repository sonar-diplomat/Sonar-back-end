using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Entities.Models.UserCore;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience;

public class SubscriptionPaymentService(
    ISubscriptionPaymentRepository repository,
    IUserService userService,
    ISubscriptionPackService subscriptionPackService)
    : GenericService<SubscriptionPayment>(repository), ISubscriptionPaymentService
{
    public async Task<SubscriptionPayment> PurchaseSubscriptionAsync(int buyerId, PurchaseSubscriptionDTO dto)
    {
        SubscriptionPayment payment = new()
        {
            Buyer = await userService.GetByIdValidatedAsync(buyerId),
            SubscriptionPack = await subscriptionPackService.GetByIdValidatedAsync(dto.SubscriptionPackId),
            Amount = dto.Amount
        };

        SubscriptionPayment createdPayment = await repository.AddAsync(payment);
        User? user = await userService.GetByIdValidatedAsync(buyerId);
        user.SubscriptionPackId = dto.SubscriptionPackId;
        await userService.UpdateUserAsync(user);
        return createdPayment;
    }
}
