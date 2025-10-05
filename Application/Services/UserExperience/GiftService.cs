using Application.Abstractions.Interfaces.Exception;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class GiftService : IGiftService
    {
        private readonly IGiftRepository giftRepository;
        private readonly ISubscriptionPaymentRepository subscriptionPaymentRepository;
        private readonly IAppExceptionFactory<AppException> appExceptionFactory;

        public GiftService(
            IGiftRepository giftRepository,
            ISubscriptionPaymentRepository subscriptionPaymentRepository,
            IAppExceptionFactory<AppException> appExceptionFactory)
        {
            this.giftRepository = giftRepository;
            this.subscriptionPaymentRepository = subscriptionPaymentRepository;
            this.appExceptionFactory = appExceptionFactory;
        }

        public async Task<Gift> SendGiftAsync(SendGiftDTO giftDto)
        {
            // Create the subscription payment (buyer pays for the gift)
            SubscriptionPayment payment = new SubscriptionPayment
            {
                BuyerId = giftDto.BuyerId,
                SubscriptionPackId = giftDto.SubscriptionPackId,
                Amount = giftDto.Amount
            };

            SubscriptionPayment createdPayment = await subscriptionPaymentRepository.AddAsync(payment);

            // Create the gift with reference to the payment
            Gift gift = new Gift
            {
                Title = giftDto.Title,
                TextContent = giftDto.TextContent,
                GiftTime = giftDto.GiftTime,
                GiftStyleId = giftDto.GiftStyleId,
                ReceiverId = giftDto.ReceiverId,
                SubscriptionPaymentId = createdPayment.Id
            };

            return await giftRepository.AddAsync(gift);
        }

        public async Task<SubscriptionPayment> AcceptGiftAsync(int giftId, int receiverId)
        {
            Gift gift = await giftRepository.GetByIdAsync(giftId);

            if (gift == null)
            {
                throw appExceptionFactory.CreateNotFound();
            }

            if (gift.ReceiverId != receiverId)
            {
                throw appExceptionFactory.CreateForbidden();
            }

            // TODO: Check if gift is already accepted
            // TODO: Activate subscription for the receiver
            // For now, just return the subscription payment
            return await subscriptionPaymentRepository.GetByIdAsync(gift.SubscriptionPaymentId);
        }

        public async Task<IEnumerable<Gift>> GetReceivedGiftsAsync(int receiverId)
        {
            IEnumerable<Gift> allGifts = await giftRepository.GetAllAsync();
            return allGifts.Where(g => g.ReceiverId == receiverId);
        }

        public async Task<IEnumerable<Gift>> GetSentGiftsAsync(int senderId)
        {
            // Get all gifts where the SubscriptionPayment.BuyerId matches senderId
            IEnumerable<Gift> allGifts = await giftRepository.GetAllAsync();
            IEnumerable<SubscriptionPayment> payments = await subscriptionPaymentRepository.GetAllAsync();

            var senderPaymentIds = payments
                .Where(p => p.BuyerId == senderId)
                .Select(p => p.Id);

            return allGifts.Where(g => senderPaymentIds.Contains(g.SubscriptionPaymentId));
        }

        public async Task<Gift> GetGiftByIdAsync(int giftId)
        {
            return await giftRepository.GetByIdAsync(giftId);
        }

        public async Task<bool> CancelGiftAsync(int giftId, int senderId)
        {
            Gift gift = await giftRepository.GetByIdAsync(giftId);

            if (gift == null)
            {
                throw appExceptionFactory.CreateNotFound();
            }

            SubscriptionPayment payment = await subscriptionPaymentRepository.GetByIdAsync(gift.SubscriptionPaymentId);

            if (payment.BuyerId != senderId)
            {
                throw appExceptionFactory.CreateForbidden();
            }

            // TODO: Check if gift is already accepted - cannot cancel accepted gifts

            await giftRepository.RemoveAsync(gift);
            await giftRepository.SaveChangesAsync();
            return true;
        }
    }
}

