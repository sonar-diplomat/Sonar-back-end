using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.File;
using Entities.Models.Music;
using Microsoft.EntityFrameworkCore;

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
    [EmailAddress]
    [MaxLength(200)]
    public string ContactEmail { get; set; }

    [Required]
    public int LicenseId { get; set; }

    [Required]
    public int CoverId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("LicenseId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual License License { get; set; }

    [ForeignKey("CoverId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ImageFile Cover { get; set; }

    public virtual ICollection<DistributorAccount> Accounts { get; set; }
    public virtual ICollection<Album> Albums { get; set; }
}