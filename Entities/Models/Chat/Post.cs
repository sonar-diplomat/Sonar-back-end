using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.Access;
using Entities.Models.Music;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Chat;

[Table("Post")]
public class Post : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Title { get; set; }

    [Required]
    [MaxLength(5000)]
    public string TextContent { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public int ArtistId { get; set; }

    [Required]
    public int VisibilityStateId { get; set; }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    [ForeignKey("UserId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Artist Artist { get; set; }

    [JsonIgnore]
    [ForeignKey("VisibilityStateId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual VisibilityState VisibilityState { get; set; }

    [JsonIgnore]
    public virtual ICollection<File.File>? Files { get; set; }
}