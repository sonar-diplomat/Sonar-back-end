namespace Application.DTOs.Chat;

public class CreateChatDTO
{
    public required string Name { get; set; }
    public required bool IsGroup { get; set; }
    public required int CoverId { get; set; }
    public int? UserId { get; set; }
}