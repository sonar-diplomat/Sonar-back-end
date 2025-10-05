using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.File;

[Table("FileType")]
public class FileType : BaseModel
{
    [Required]
    [MaxLength(10)]
    public string Name { get; set; }
}