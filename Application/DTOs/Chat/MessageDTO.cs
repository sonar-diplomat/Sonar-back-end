namespace Application.DTOs.Chat;

public class MessageDTO
{
    public int? Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? SenderId { get; set; }
    
    public required string TextContent { get; set; }
    public int? ReplyMessageId { get; set; }
    
    public List<MessageReadDTO>? ReadBy { get; set; }
}