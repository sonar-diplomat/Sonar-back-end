namespace Application.DTOs.Chat;

public class MessageDTO
{
    // Optional fields for response (when returning messages)
    public int? Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? SenderId { get; set; }
    
    // Required fields for creation
    public required string TextContent { get; set; }
    public int? ReplyMessageId { get; set; }
}