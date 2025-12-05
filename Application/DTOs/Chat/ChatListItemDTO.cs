namespace Application.DTOs.Chat;

public class ChatListItemDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required bool IsGroup { get; set; }
    public required int CoverId { get; set; }
    public required int CreatorId { get; set; }
    public required int[] UserIds { get; set; }
    public LastMessageDTO? LastMessage { get; set; }
}

public class LastMessageDTO
{
    public required string TextContent { get; set; }
    public DateTime? CreatedAt { get; set; }
}

