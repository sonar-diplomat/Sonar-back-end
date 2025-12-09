using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.Distribution;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Music;

[Table("Album")]
public class Album : Collection
{
    [Required]
    public int DistributorId { get; set; }

    public int? GenreId { get; set; }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    [ForeignKey("DistributorId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Distributor Distributor { get; set; }

    [JsonIgnore]
    [ForeignKey("GenreId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Genre? Genre { get; set; }

    [JsonIgnore]
    public virtual ICollection<AlbumArtist> AlbumArtists { get; set; }

    [JsonIgnore]
    public virtual ICollection<AlbumMoodTag> AlbumMoodTags { get; set; }
}