using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.File;

[Table("File")]
public class File : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; }

    [Required]
    [MaxLength(1000)]
    [Url]
    [ConcurrencyCheck]
    public string Url { get; set; }

    [Required]
    public int TypeId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("TypeId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public FileType Type { get; set; }
}