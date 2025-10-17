using System.ComponentModel.DataAnnotations;

namespace Entities.Models.Distribution;

public class DistributorAccount : BaseModel
{
    [Required]
    [MinLength(4)]
    [MaxLength(50)]
    public string Username { get; set; }

    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    
    public int DistributorId { get; set; }

    
    public virtual Distributor Distributor { get; set; }
    public virtual ICollection<DistributorSession> Sessions { get; set; }
}
