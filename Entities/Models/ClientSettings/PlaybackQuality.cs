using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.ClientSettings;

[Table("PlaybackQuality")]
public class PlaybackQuality : BaseModel
{
    [Required, MaxLength(50)]
    public string Name { get; set; }
    [Required]
    public int BitRate { get; set; }
    [MaxLength(500)]
    public string Description { get; set; }
}