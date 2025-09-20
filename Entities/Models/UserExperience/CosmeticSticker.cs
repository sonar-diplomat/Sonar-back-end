using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("CosmeticSticker")]
public class CosmeticSticker
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public double X { get; set; }
    [Required]
    public double Y { get; set; }
    
    [Required]
    public int CosmeticItemId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("CosmeticItemId")]
    public virtual CosmeticItem CosmeticItem { get; set; }
}