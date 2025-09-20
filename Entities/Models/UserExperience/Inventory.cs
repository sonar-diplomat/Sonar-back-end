using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models;

namespace Entities.Models;

[Table("Inventory")]
public class Inventory
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    public ICollection<CosmeticItem> CosmeticItems { get; set; }
}