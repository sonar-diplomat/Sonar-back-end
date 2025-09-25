using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.Distribution;

namespace Entities.Models.Music;

[Table("Album")]
public class Album : Collection
{
    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Artist> Artists { get; set; }
}