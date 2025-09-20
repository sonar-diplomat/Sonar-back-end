using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("Album")]
public class Album : Collection
{
    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Artist> Artists { get; set; }
}