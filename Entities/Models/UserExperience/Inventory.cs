using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.UserExperience;

[Table("Inventory")]
public class Inventory : BaseModel
{
    public ICollection<CosmeticItem> CosmeticItems { get; set; }
}
