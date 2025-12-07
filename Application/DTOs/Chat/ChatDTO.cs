namespace Application.DTOs.Chat;

public class ChatDTO
{
    public required string Name { get; set; }
    public required bool IsGroup { get; set; }
    public required int CoverId { get; set; }
    public required int CreatorId { get; set; }
    public required int[] UserIds { get; set; }
    public required int[] AdminIds { get; set; }
}