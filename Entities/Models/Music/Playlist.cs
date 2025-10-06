using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Music;

[Table("Playlist")]
public class Playlist : Collection
{
    [Required]
    public int CreatorId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("CreatorId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual User Creator { get; set; }
}