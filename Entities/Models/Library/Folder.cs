using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.Music;
using Infrastructure;

namespace Entities.Models.Library;

[Table("Folder")]
public class Folder : BaseModel
{
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