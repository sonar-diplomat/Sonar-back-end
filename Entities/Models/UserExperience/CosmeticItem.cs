using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
    /// </summary>
    [ForeignKey("TypeId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual CosmeticItemType Type { get; set; }

    [ForeignKey("FileId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual File.File File { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; }
}
