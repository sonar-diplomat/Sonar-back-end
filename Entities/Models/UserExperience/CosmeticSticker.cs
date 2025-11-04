using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
    [JsonIgnore]
    [ForeignKey("CosmeticItemId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual CosmeticItem CosmeticItem { get; set; }
}
