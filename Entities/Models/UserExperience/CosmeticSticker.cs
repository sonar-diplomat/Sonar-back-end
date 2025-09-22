using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models;

[Table("CosmeticSticker")]
public class CosmeticSticker : BaseModel
{
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