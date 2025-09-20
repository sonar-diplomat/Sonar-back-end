using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("FileType")]
public class FileType
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(10)]
    public string Name { get; set; }
}