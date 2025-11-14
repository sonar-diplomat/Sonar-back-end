using Application.DTOs.User;

namespace Application.DTOs.UserExperience;

public class SubscriptionPaymentDTO
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public UserResponseDTO Buyer { get; set; }
    
    public int SubscriptionPackId { get; set; }
    public string SubscriptionPackName { get; set; }
}

