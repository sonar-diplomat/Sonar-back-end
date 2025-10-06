using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.UserExperience;

[Table("AchievementProgress")]
public class AchievementProgress : BaseModel
{
    [Required]
    [MaxLength(500)]
    public string Value { get; set; }

    [Required]
    public int AchievementId { get; set; }

    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("AchievementId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Achievement Achievement { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}