namespace Application.DTOs.Library;

public class FolderDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsProtected { get; set; }
    
    // Related data
    public int? ParentFolderId { get; set; }
    public string? ParentFolderName { get; set; }
    
    // Collections
    public List<SubFolderDTO> SubFolders { get; set; }
    public List<CollectionSummaryDTO> Collections { get; set; }
}

public class SubFolderDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsProtected { get; set; }
    public int SubFolderCount { get; set; }
    public int CollectionCount { get; set; }
}

public class CollectionSummaryDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; } // "Album", "Playlist", "Blend"
    public string CoverUrl { get; set; }
}

