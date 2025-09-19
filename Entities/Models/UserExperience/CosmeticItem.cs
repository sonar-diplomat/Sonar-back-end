using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

[Table("CosmeticItem")]
public class CosmeticItem
{
    [Key]
    public int Id { get; set; }
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
    public virtual File File { get; set; }
    
    public virtual ICollection<Inventory> Inventories { get; set; }
}