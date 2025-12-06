namespace Application.DTOs.Chat;

public class ChatStickerDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ImageFileId { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
}

