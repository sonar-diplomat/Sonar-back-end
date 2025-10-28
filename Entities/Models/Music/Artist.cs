using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    [ForeignKey("UserId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual User User { get; set; }

    public virtual ICollection<Track> Tracks { get; set; }
    public virtual ICollection<AlbumArtist> AlbumArtists { get; set; }
}