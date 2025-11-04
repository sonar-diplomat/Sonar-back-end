using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Distribution;

public class DistributorAccount : BaseModel
{
    [Required]
    [MinLength(4)]
    [MaxLength(50)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; }

    [Required] public bool IsMaster { get; set; }

    [Required] public byte[] PasswordHash { get; set; }
    [Required] public byte[] PasswordSalt { get; set; }

    [Required] public int DistributorId { get; set; }

    [JsonIgnore]
    [ForeignKey("DistributorId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual Distributor Distributor { get; set; }

    [JsonIgnore]
    public virtual ICollection<DistributorSession> Sessions { get; set; }
}