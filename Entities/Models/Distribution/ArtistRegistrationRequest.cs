using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

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
    [JsonIgnore]
    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    [JsonIgnore]
    [ForeignKey("DistributorId")]
    public virtual Distributor Distributor { get; set; }
}