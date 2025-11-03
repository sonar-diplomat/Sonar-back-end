namespace Application.DTOs.Chat;

public class MessageDTO
{
    public required string TextContent { get; set; }

    public int? ReplyMessageId { get; set; }
}