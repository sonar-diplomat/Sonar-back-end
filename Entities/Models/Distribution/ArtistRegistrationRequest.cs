using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;

namespace Entities.Models.Distribution;

public class ArtistRegistrationRequest : BaseModel
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ArtistName { get; set; }

    [Required]
    public int DistributorId { get; set; }
    
    public DateTime? ResolvedAt { get; set; }

    [Required]
    public DateTime RequestedAt { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    [ForeignKey("DistributorId")]
    public virtual Distributor Distributor { get; set; }
}