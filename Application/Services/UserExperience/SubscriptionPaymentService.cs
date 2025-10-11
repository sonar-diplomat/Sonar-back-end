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
    public async Task<SubscriptionPayment> PurchaseSubscriptionAsync(PurchaseSubscriptionDTO dto)
    {
        SubscriptionPayment payment = new()
        {
            Buyer = await userService.GetByIdValidatedAsync(dto.UserId),
            SubscriptionPack = await subscriptionPackService.GetByIdValidatedAsync(dto.SubscriptionPackId),
            Amount = dto.Amount
        };

        SubscriptionPayment createdPayment = await repository.AddAsync(payment);
        User? user = await userService.GetByIdValidatedAsync(dto.UserId);
        user.SubscriptionPackId = dto.SubscriptionPackId;
        await userService.UpdateUserAsync(user);
        return createdPayment;
    }
}
