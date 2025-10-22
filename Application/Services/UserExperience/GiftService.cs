using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Response;
using Entities.Models.UserCore;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience;

public class GiftService(
    IGiftRepository repository,
    ISubscriptionPaymentService subscriptionPaymentService,
    ISubscriptionPackService subscriptionPackService,
    IGiftStyleService giftStyleService,
    IUserService userService)
    : GenericService<Gift>(repository), IGiftService
{
    public async Task<Gift> SendGiftAsync(SendGiftDTO giftDto)
    {
        SubscriptionPayment payment = new()
        {
            Buyer = await userService.GetByIdValidatedAsync(giftDto.BuyerId),
            SubscriptionPack = await subscriptionPackService.GetByIdValidatedAsync(giftDto.SubscriptionPackId),
            Amount = giftDto.Amount
        };

        Gift gift = new()
        {
            Title = giftDto.Title,
            TextContent = giftDto.TextContent,
            GiftTime = giftDto.GiftTime,
            GiftStyle = await giftStyleService.GetByIdValidatedAsync(giftDto.GiftStyleId),
            Receiver = await userService.GetByIdValidatedAsync(giftDto.ReceiverId),
            SubscriptionPayment = await subscriptionPaymentService.CreateAsync(payment)
        };

        return await repository.AddAsync(gift);
    }

    public async Task<IEnumerable<Gift>> GetReceivedGiftsAsync(int receiverId)
    {
        return await repository.GetAllByReceiverAsync(receiverId);
    }

    public async Task<IEnumerable<Gift>> GetSentGiftsAsync(int senderId)
    {
        IEnumerable<Gift> allGifts = await repository.GetAllAsync();
        IEnumerable<SubscriptionPayment> payments = await subscriptionPaymentService.GetAllAsync();
        IEnumerable<int> senderPaymentIds = payments
            .Where(p => p.BuyerId == senderId)
            .Select(p => p.Id);
        return allGifts.Where(g => senderPaymentIds.Contains(g.SubscriptionPaymentId));
    }

    public async Task<SubscriptionPayment> AcceptGiftAsync(int giftId, int receiverId)
    {
        Gift gift = await GetByIdValidatedAsync(giftId);
        User receiver = await userService.GetByIdValidatedAsync(gift.ReceiverId);
        if (receiver.SubscriptionPackId != null || gift.ReceiverId != receiverId)
            throw ResponseFactory.Create<ForbiddenResponse>(["You are not allowed to accept this gift."]);

        SubscriptionPayment payment =
            await subscriptionPaymentService.GetByIdValidatedAsync(gift.SubscriptionPaymentId);
        receiver.SubscriptionPackId = payment.SubscriptionPackId;
        await userService.UpdateUserAsync(receiver);
        gift.AcceptanceDate = DateTime.UtcNow;
        return payment;
    }

    public async Task CancelGiftAsync(int giftId, int receiverId)
    {
        Gift gift = await GetByIdValidatedAsync(giftId);
        if (gift.ReceiverId != receiverId)
            throw ResponseFactory.Create<ForbiddenResponse>(["You are not allowed to cancel this gift."]);

        SubscriptionPayment payment =
            await subscriptionPaymentService.GetByIdValidatedAsync(gift.SubscriptionPaymentId);
        await repository.RemoveAsync(gift);
        // TODO: Refund payment
        await subscriptionPaymentService.DeleteAsync(payment);
    }
}