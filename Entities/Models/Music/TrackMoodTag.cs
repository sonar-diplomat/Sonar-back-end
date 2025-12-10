using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Music;

[Table("TrackMoodTag")]
public class TrackMoodTag : BaseModel
{
    [Required]
    public int TrackId { get; set; }

    [Required]
    public int MoodTagId { get; set; }

    [JsonIgnore]
    [ForeignKey("TrackId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual Track Track { get; set; }

    [JsonIgnore]
    [ForeignKey("MoodTagId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual MoodTag MoodTag { get; set; }
}

