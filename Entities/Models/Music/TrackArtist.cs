using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Entities.Models.Music;

public class TrackArtist : BaseModel
{
    [Required]
    [MaxLength(100)]
    public required string Pseudonym { get; set; }

    public int? ArtistId { get; set; }
    public int TrackId { get; set; }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    public virtual Artist? Artist { get; set; }

    [JsonIgnore]
    public virtual Track Track { get; set; }
}

