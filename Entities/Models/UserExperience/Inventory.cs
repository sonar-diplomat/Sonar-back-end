using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.UserExperience;

[Table("Inventory")]
public class Inventory : BaseModel
{
    [Required]
    public int UserId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("UserId")]
    public UserCore.User User { get; set; }
    
    public ICollection<CosmeticItem> CosmeticItems { get; set; }
}