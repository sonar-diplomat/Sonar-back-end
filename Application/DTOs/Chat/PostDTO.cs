using FileModel = Entities.Models.File.File;

namespace Application.DTOs;

public class PostDTO
{
    public required string Title { get; set; }
    public string? TextContent { get; set; }
    public ICollection<int>? AttachmentIds { get; set; }
    public DateTime? SetPublicOn { get; set; }
}