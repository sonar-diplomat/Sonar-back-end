using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.UserCore;

[Table("UserStatus")]
public class UserStatus : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } //online   offline  dontDisturb  inactive/idle
}
