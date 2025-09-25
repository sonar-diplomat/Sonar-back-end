using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models.UserExperience;

[Table("CosmeticItemType")]
public class CosmeticItemType : BaseModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; }
}