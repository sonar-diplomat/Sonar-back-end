using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.UserExperience;

[Table("Achievement")]
public class Achievement : BaseModel
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; }

    [Required]
    public string Condition { get; set; }

    [Required]
    public string Target { get; set; }

    [Required]
    public string Reward { get; set; }

    [Required]
    public int CategoryId { get; set; }

    /// <summary>
    /// </summary>
    [JsonIgnore]
    [ForeignKey("CategoryId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual AchievementCategory AchievementCategory { get; set; }
}
