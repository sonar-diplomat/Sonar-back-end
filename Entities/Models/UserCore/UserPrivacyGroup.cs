using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.UserCore;

[Table("UserPrivacyGroup")]
public class UserPrivacyGroup : BaseModel
{
    [Required, MaxLength(50)]
    public string Name { get; set; }
}