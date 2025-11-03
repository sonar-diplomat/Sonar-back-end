using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Library;

[Table("Library")]
public class Library : BaseModel
{
    public virtual User User { get; set; }
    public int? RootFolderId { get; set; }

    [ForeignKey("RootFolderId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Folder? RootFolder { get; set; }

    public virtual ICollection<Folder> Folders { get; set; }
}