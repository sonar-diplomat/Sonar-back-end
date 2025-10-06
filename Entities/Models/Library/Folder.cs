using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.Music;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Library;

[Table("Folder")]
public class Folder : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    public int LibraryId { get; set; }

    public int? ParentFolderId { get; set; }

    /// <summary>
    ///     <summary>
    [ForeignKey("LibraryId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Library Library { get; set; }

    [ForeignKey("ParentFolderId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Folder ParentFolder { get; set; }

    public virtual ICollection<Folder> SubFolders { get; set; }
    public virtual ICollection<Collection> Collections { get; set; }
}