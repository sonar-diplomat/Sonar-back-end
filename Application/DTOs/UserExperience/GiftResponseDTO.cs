namespace Application.DTOs.UserExperience;

public class GiftResponseDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string TextContent { get; set; }
    public DateTime GiftTime { get; set; }
    public DateTime AcceptanceDate { get; set; }
    public string ReceiverName { get; set; }
    public string GiftStyleName { get; set; }
    public decimal SubscriptionAmount { get; set; }
    public string SubscriptionPackName { get; set; }
}

