using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.UserExperience;

[Table("Inventory")]
public class Inventory : BaseModel
{
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("UserId")]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public User User { get; set; }

    public ICollection<CosmeticItem> CosmeticItems { get; set; }
}
