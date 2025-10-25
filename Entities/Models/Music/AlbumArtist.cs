using System.ComponentModel.DataAnnotations;

namespace Entities.Models.Music;

public class AlbumArtist : BaseModel
{
    [Required]
    [MaxLength(100)]
    public required string Pseudonym { get; set; }

    public required int? ArtistId { get; set; }
    public required int AlbumId { get; set; }

    /// <summary>
    /// </summary>
    public virtual Artist? Artist { get; set; }

    public virtual Album Album { get; set; }
}