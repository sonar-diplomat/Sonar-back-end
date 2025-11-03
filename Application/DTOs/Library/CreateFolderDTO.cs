namespace Application.DTOs;

public class CreateFolderDTO
{
    public required string Name { get; set; }
    public int? ParentFolderId { get; set; }
}