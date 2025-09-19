using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("Folder")]
public class Folder
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; }
    
    [Required]
    public int LibraryId { get; set; }
    public int? ParentFolderId { get; set; }
    
    /// <summary>
    ///
    /// <summary>
    [ForeignKey("LibraryId")]
    public virtual Library Library { get; set; }
    [ForeignKey("ParentFolderId")]
    public virtual Folder ParentFolder { get; set; }
    
    public virtual ICollection<Folder> SubFolders { get; set; }
    public virtual ICollection<Collection> Collections { get; set; }
}