using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.Access;
using Entities.Models.File;
using Entities.Models.Library;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Music;

public abstract class Collection : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public int VisibilityStateId { get; set; }

    [Required]
    public int CoverId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("VisibilityStateId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual VisibilityState VisibilityState { get; set; }

    [ForeignKey("CoverId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ImageFile Cover { get; set; }

    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<Track> Tracks { get; set; }
    public virtual ICollection<Folder> Folders { get; set; }
}