using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Models.UserCore;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience;

public class GiftService(
    IGiftRepository repository,
    ISubscriptionPaymentRepository subscriptionPaymentRepository,
    IUserRepository userRepository,
    AppExceptionFactory appExceptionFactory)
    : GenericService<Gift>(repository), IGiftService
{
    public async Task<Gift> SendGiftAsync(SendGiftDTO giftDto)
    {
        // Create the subscription payment (buyer pays for the gift)
        SubscriptionPayment payment = new()
        {
            BuyerId = giftDto.BuyerId,
            SubscriptionPackId = giftDto.SubscriptionPackId,
            Amount = giftDto.Amount
        };

        SubscriptionPayment createdPayment = await subscriptionPaymentRepository.AddAsync(payment);

        // Create the gift with reference to the payment
        Gift gift = new()
        {
            Title = giftDto.Title,
            TextContent = giftDto.TextContent,
            GiftTime = giftDto.GiftTime,
            GiftStyleId = giftDto.GiftStyleId,
            ReceiverId = giftDto.ReceiverId,
            SubscriptionPaymentId = createdPayment.Id
        };

        return await repository.AddAsync(gift);
    }

    public async Task<SubscriptionPayment> AcceptGiftAsync(int giftId, int receiverId)
    {
        // Validate gift exists and belongs to the receiver
        Gift gift = await repository.GetByIdAsync(giftId);
        if (gift == null) throw new NotImplementedException();
        if (gift.ReceiverId != receiverId) throw new NotImplementedException();

        // Get subscription payment details
        SubscriptionPayment payment = await subscriptionPaymentRepository.GetByIdAsync(gift.SubscriptionPaymentId);

        // Activate subscription for receiver
        User? receiver = await userRepository.GetByIdAsync(receiverId);
        if (receiver == null) throw new NotImplementedException();

        receiver.SubscriptionPackId = payment.SubscriptionPackId;
        await userRepository.UpdateAsync(receiver);
        await repository.RemoveAsync(gift);

        return payment;
    }

    public async Task<IEnumerable<Gift>> GetReceivedGiftsAsync(int receiverId)
    {
        IEnumerable<Gift> allGifts = await repository.GetAllAsync();
        return allGifts.Where(g => g.ReceiverId == receiverId);
    }

    public async Task<IEnumerable<Gift>> GetSentGiftsAsync(int senderId)
    {
        // Get all gifts where the SubscriptionPayment.BuyerId matches senderId
        IEnumerable<Gift> allGifts = await repository.GetAllAsync();
        IEnumerable<SubscriptionPayment> payments = await subscriptionPaymentRepository.GetAllAsync();

        IEnumerable<int> senderPaymentIds = payments
            .Where(p => p.BuyerId == senderId)
            .Select(p => p.Id);

        return allGifts.Where(g => senderPaymentIds.Contains(g.SubscriptionPaymentId));
    }

    public async Task<bool> CancelGiftAsync(int giftId, int senderId)
    {
        Gift gift = await repository.GetByIdAsync(giftId);

        if (gift == null) throw new NotImplementedException();

        SubscriptionPayment payment = await subscriptionPaymentRepository.GetByIdAsync(gift.SubscriptionPaymentId);

        if (payment.BuyerId != senderId) throw new NotImplementedException();

        await repository.RemoveAsync(gift);
        return true;
    }
}
