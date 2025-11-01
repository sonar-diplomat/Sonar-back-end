using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
    public bool IsProtected { get; set; }

    [Required]
    public int LibraryId { get; set; }

    public int? ParentFolderId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("ParentFolderId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Folder ParentFolder { get; set; }

    [ForeignKey("LibraryId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    [JsonIgnore]
    public virtual Library Library { get; set; }

    public virtual ICollection<Folder> SubFolders { get; set; }
    public virtual ICollection<Collection> Collections { get; set; }
}