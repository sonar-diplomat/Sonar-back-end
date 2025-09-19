using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("Distributor")]
public class Distributor
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required, MaxLength(500)]
    public string Description { get; set; }
    
    [Required]
    public int LicenseId { get; set; }
    [Required]
    public int CoverId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("LicenseId")]
    public virtual License License { get; set; }
    [ForeignKey("CoverId")]
    public virtual File Cover { get; set; }
    
    public virtual ICollection<DistributorSession> Sessions { get; set; }
    public virtual ICollection<Album> Albums { get; set; }
}