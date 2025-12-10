using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Music;

[Table("AlbumMoodTag")]
public class AlbumMoodTag : BaseModel
{
    [Required]
    public int AlbumId { get; set; }

    [Required]
    public int MoodTagId { get; set; }

    [JsonIgnore]
    [ForeignKey("AlbumId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual Album Album { get; set; }

    [JsonIgnore]
    [ForeignKey("MoodTagId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual MoodTag MoodTag { get; set; }
}

