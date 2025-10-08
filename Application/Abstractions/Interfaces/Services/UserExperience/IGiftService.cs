using Application.DTOs;
using Entities.Models.UserExperience;

namespace Application.Abstractions.Interfaces.Services;

public interface IGiftService : IGenericService<Gift>
{
    Task<Gift> SendGiftAsync(SendGiftDTO giftDto);
    Task<SubscriptionPayment> AcceptGiftAsync(int giftId, int receiverId);
    Task<IEnumerable<Gift>> GetReceivedGiftsAsync(int receiverId);
    Task<IEnumerable<Gift>> GetSentGiftsAsync(int senderId);
    Task<bool> CancelGiftAsync(int giftId, int senderId);
}
