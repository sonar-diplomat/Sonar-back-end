using Entities.Models.Distribution;

namespace Entities.Models.Music;

public class AlbumArtist
{
    public required string Pseudonym { get; set; }

    public required int ArtistId { get; set; }
    public required int AlbumId { get; set; }

    /// <summary>
    /// </summary>
    public virtual Artist Artist { get; set; }

    public virtual Album Album { get; set; }
}