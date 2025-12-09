using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Music;

[Table("MoodTag")]
public class MoodTag : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [JsonIgnore]
    public virtual ICollection<TrackMoodTag> TrackMoodTags { get; set; }

    [JsonIgnore]
    public virtual ICollection<AlbumMoodTag> AlbumMoodTags { get; set; }
}

