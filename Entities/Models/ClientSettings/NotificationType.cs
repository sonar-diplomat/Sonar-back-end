using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models.ClientSettings;

[Table("NotificationType")]
public class NotificationType : BaseModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; }
    [Required, MaxLength(500)]
    public string Description { get; set; }
}