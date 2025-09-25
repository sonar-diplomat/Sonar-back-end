using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models.UserExperience;

[Table("CosmeticItem")]
public class CosmeticItem : BaseModel
{
    [Required]
    public int Price { get; set; }
    
    [Required]
    public int TypeId { get; set; }
    [Required]
    public int FileId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("TypeId")]
    public virtual CosmeticItemType Type { get; set; }
    [ForeignKey("FileId")]
    public virtual File.File File { get; set; }
    
    public virtual ICollection<Inventory> Inventories { get; set; }
}