using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.Chat;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Music;

[Table("Artist")]
public class Artist : BaseModel
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ArtistName { get; set; }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    [ForeignKey("UserId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual User User { get; set; }

    [JsonIgnore]
    public virtual ICollection<Post> Posts { get; set; }
    [JsonIgnore]
    public virtual ICollection<TrackArtist> TrackArtists { get; set; }
    [JsonIgnore]
    public virtual ICollection<AlbumArtist> AlbumArtists { get; set; }
}