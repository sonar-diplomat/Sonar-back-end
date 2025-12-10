using Entities.Models.Access;
using Entities.Models.File;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Music;

[Table("Track")]
public class Track : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    public TimeSpan? Duration { get; set; }

    [Required]
    public bool IsExplicit { get; set; }

    [Required]
    public bool DrivingDisturbingNoises { get; set; }

    [Required]
    public int VisibilityStateId { get; set; }

    [Required]
    public int CoverId { get; set; }

    [Required]
    public int GenreId { get; set; }

    [Required]
    public virtual int LowQualityAudioFileId { get; set; }

    public virtual int? MediumQualityAudioFileId { get; set; }

    public virtual int? HighQualityAudioFileId { get; set; }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    [ForeignKey("LowQualityAudioFileId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual AudioFile LowQualityAudioFile { get; set; }

    [JsonIgnore]
    [ForeignKey("MediumQualityAudioFileId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual AudioFile? MediumQualityAudioFile { get; set; }

    [JsonIgnore]
    [ForeignKey("HighQualityAudioFileId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual AudioFile? HighQualityAudioFile { get; set; }

    [JsonIgnore]
    [ForeignKey("VisibilityStateId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual VisibilityState VisibilityState { get; set; }

    [JsonIgnore]
    [ForeignKey("CoverId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ImageFile Cover { get; set; }

    [JsonIgnore]
    [ForeignKey("GenreId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Genre Genre { get; set; }

    [JsonIgnore]
    public virtual ICollection<Queue> QueuesWherePrimary { get; set; }
    [JsonIgnore]
    public virtual ICollection<TrackArtist> TrackArtists { get; set; }
    [JsonIgnore]
    public virtual ICollection<Collection> Collections { get; set; }
    [JsonIgnore]
    public virtual ICollection<Queue> Queues { get; set; }
    [JsonIgnore]
    public virtual ICollection<TrackMoodTag> TrackMoodTags { get; set; }
}