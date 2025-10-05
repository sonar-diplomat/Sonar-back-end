using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.Music;

namespace Entities.Models.Distribution;

[Table("Distributor")]
public class Distributor : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; }

    [Required]
    public int LicenseId { get; set; }

    [Required]
    public int CoverId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("LicenseId")]
    public virtual License License { get; set; }

    [ForeignKey("CoverId")]
    public virtual File.File Cover { get; set; }

    public virtual ICollection<DistributorSession> Sessions { get; set; }
    public virtual ICollection<Album> Albums { get; set; }
}