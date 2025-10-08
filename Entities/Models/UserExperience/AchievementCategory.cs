using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.UserExperience;

[Table("AchievementCategory")]
public class AchievementCategory : BaseModel
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
}
