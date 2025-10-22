using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.Distribution;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Music;

[Table("Album")]
public class Album : Collection
{
    [Required]
    public int DistributorId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("DistributorId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Distributor Distributor { get; set; }

    public virtual ICollection<Artist> Artists { get; set; }
}