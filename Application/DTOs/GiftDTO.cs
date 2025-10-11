namespace Application.DTOs;

/// <summary>
///     DTO for sending a gift subscription
/// </summary>
public class SendGiftDTO
{
    public string Title { get; set; }
    public string TextContent { get; set; }
    public DateTime GiftTime { get; set; }
    public int GiftStyleId { get; set; }
    public int ReceiverId { get; set; }
    public int BuyerId { get; set; } // The sender/buyer
    public int SubscriptionPackId { get; set; }
    public decimal Amount { get; set; }
}
