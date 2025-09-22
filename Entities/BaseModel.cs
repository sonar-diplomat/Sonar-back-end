using System.ComponentModel.DataAnnotations;

namespace Infrastructure;

public abstract class BaseModel
{
    [Key]
    public int Id { get; set; }
}