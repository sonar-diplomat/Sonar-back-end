using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.Access;
using Entities.Models.File;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

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
    public virtual int LowQualityAudioFileId { get; set; }

    public virtual int? MediumQualityAudioFileId { get; set; }

    public virtual int? HighQualityAudioFileId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("LowQualityAudioFileId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual AudioFile LowQualityAudioFile { get; set; }

    [ForeignKey("MediumQualityAudioFileId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual AudioFile? MediumQualityAudioFile { get; set; }

    [ForeignKey("HighQualityAudioFileId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual AudioFile? HighQualityAudioFile { get; set; }

    [ForeignKey("VisibilityStateId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual VisibilityState VisibilityState { get; set; }

    [ForeignKey("CoverId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ImageFile Cover { get; set; }

    public virtual ICollection<Artist> Artists { get; set; }
    public virtual ICollection<Collection> Collections { get; set; }
    public virtual ICollection<Queue> Queues { get; set; }
}