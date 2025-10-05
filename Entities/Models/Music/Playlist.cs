using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;

namespace Entities.Models.Music;

[Table("Playlist")]
public class Playlist : Collection
{
    [Required]
    public int CreatorId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("CreatorId")]
    public virtual User Creator { get; set; }
}