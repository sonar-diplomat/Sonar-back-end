using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.UserExperience;

[Table("GiftStyle")]
public class GiftStyle : BaseModel
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
}
