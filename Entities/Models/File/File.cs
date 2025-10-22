using System.ComponentModel.DataAnnotations;

namespace Entities.Models.File;

public abstract class File : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; }

    [Required]
    [MaxLength(1000)]
    [Url]
    [ConcurrencyCheck]
    public string Url { get; set; }
}