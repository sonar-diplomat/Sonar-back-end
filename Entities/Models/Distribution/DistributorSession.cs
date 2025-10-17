using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Distribution;

[Table("DistributorSession")]
public class DistributorSession : BaseModel
{
    [Required]
    [MinLength(4)]
    [MaxLength(16)]
    private byte[] IpAddressBytes;

    [Required]
    [MaxLength(30)]
    public string UserAgent { get; set; }

    [Required]
    [MaxLength(30)]
    public string DeviceName { get; set; }

    [Required]
    public DateTime LastActive { get; set; }

    [Required]
    public int DistributorAccountId { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("DistributorAccountId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual DistributorAccount DistributorAccount { get; set; }

    [NotMapped]
    public IPAddress IPAddress
    {
        get => new(IpAddressBytes);
        set => IpAddressBytes = value.GetAddressBytes();
    }
}
