using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.Music;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.UserCore;

[Table("Queue")]
public class Queue : BaseModel
{
    [Required]
    public TimeSpan Position { get; set; }

    public int? CollectionId { get; set; }
    public int? CurrentTrackId { get; set; }

    [ForeignKey("CollectionId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Collection? Collection { get; set; }

    [ForeignKey("CurrentTrackId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Track? CurrentTrack { get; set; }

    public virtual UserState UserState { get; set; }
    public virtual ICollection<Track> Tracks { get; set; }
}