using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models.UserCore;

[Table("UserPrivacyGroup")]
public class UserPrivacyGroup : BaseModel
{
    [Required, MaxLength(50)]
    public string Name { get; set; }
}