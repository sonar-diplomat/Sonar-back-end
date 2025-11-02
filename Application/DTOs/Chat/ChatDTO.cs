using Entities.Models.File;

namespace Application.DTOs.Chat;

public class ChatDTO
{
    public required string Name { get; set; }
    public required ImageFile Cover { get; set; }
    public required int[] UserIds { get; set; }
}