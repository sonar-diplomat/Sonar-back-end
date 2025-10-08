using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.UserExperience;

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
    /// </summary>
    [ForeignKey("CosmeticItemId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual CosmeticItem CosmeticItem { get; set; }
}
