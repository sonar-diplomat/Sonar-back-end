using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IGiftService : IGenericService<Gift> 
    {
        /// <summary>
        /// Send a gift subscription to another user
        /// </summary>
        Task<Gift> SendGiftAsync(SendGiftDTO giftDto);

        /// <summary>
        /// Accept a gift and activate the subscription
        /// </summary>
        Task<SubscriptionPayment> AcceptGiftAsync(int giftId, int receiverId);

        /// <summary>
        /// Get all gifts received by a user
        /// </summary>
        Task<IEnumerable<Gift>> GetReceivedGiftsAsync(int receiverId);

        /// <summary>
        /// Get all gifts sent by a user (including planned)
        /// </summary>
        Task<IEnumerable<Gift>> GetSentGiftsAsync(int senderId);

        /// <summary>
        /// Cancel a planned gift (only before it's accepted)
        /// </summary>
        Task<bool> CancelGiftAsync(int giftId, int senderId);
    }
}

