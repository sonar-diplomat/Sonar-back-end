using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models.ClientSettings;

[Table("Theme")]
public class Theme : BaseModel
{
    [Required, MaxLength(50)] 
    public string Name { get; set; }
    [MaxLength(500)] 
    public string? Description { get; set; }
}