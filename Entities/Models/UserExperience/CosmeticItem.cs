using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
    [JsonIgnore]
    [ForeignKey("TypeId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual CosmeticItemType Type { get; set; }

    [JsonIgnore]
    [ForeignKey("FileId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual File.File File { get; set; }

    [JsonIgnore]
    public virtual ICollection<Inventory> Inventories { get; set; }
}
