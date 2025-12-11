using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.Music;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.UserCore;

[Table("QueueTrack")]
public class QueueTrack : BaseModel
{
    [Required]
    public int QueueId { get; set; }
    
    [Required]
    public int TrackId { get; set; }
    
    [Required]
    public int Order { get; set; }
    
    public bool IsManuallyAdded { get; set; }

    [JsonIgnore]
    [ForeignKey("QueueId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual Queue Queue { get; set; }

    [JsonIgnore]
    [ForeignKey("TrackId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual Track Track { get; set; }
}

