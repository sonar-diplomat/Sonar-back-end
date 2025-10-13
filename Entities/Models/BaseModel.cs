using System.ComponentModel.DataAnnotations;

namespace Entities.Models;

public abstract class BaseModel
{
    [Key]
    public int Id { get; set; }
}
