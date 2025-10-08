namespace Application.DTOs;

/// <summary>
///     DTO for purchasing a subscription for yourself
/// </summary>
public class PurchaseSubscriptionDTO
{
    public int UserId { get; set; }
    public int SubscriptionPackId { get; set; }
    public decimal Amount { get; set; }
}
