using Entities.Models.Access;
using Entities.Models.Distribution;
using Entities.Models.UserCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Music;

[Table("Track")]
public class Track : BaseModel
{
    [Required, MaxLength(100)]
    public string Title { get; set; }
    public TimeSpan Duration { get; set; }
    [Required]
    public bool IsExplicit { get; set; }
    [Required]
    public bool DrivingDisturbingNoises { get; set; }

    [Required]
    public int VisibilityStateId { get; set; }
    [Required]
    public int AudioFileId { get; set; }
    [Required]
    public int CoverId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("VisibilityStateId")]
    public virtual VisibilityState VisibilityState { get; set; }
    [ForeignKey("AudioFileId")]
    public virtual File.File AudioFile { get; set; }
    [ForeignKey("CoverId")]
    public virtual File.File Cover { get; set; }

    public virtual ICollection<Artist> Artists { get; set; }
    public virtual ICollection<Collection> Collections { get; set; }
    public virtual ICollection<Queue> Queues { get; set; }

}