using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Music;

[Table("Playlist")]
public class Playlist : Collection
{
    [Required] public int CreatorId { get; set; }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    [ForeignKey("CreatorId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual User Creator { get; set; }

    [JsonIgnore]
    public virtual ICollection<User> Contributors { get; set; }
}