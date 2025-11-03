using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text.Json.Serialization;

namespace Entities.Models.Distribution;

[Table("DistributorSession")]
public class DistributorSession : BaseModel
{
    [Required]
    [MinLength(4)]
    [MaxLength(16)]
    private byte[]? IpAddressBytes;

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

    [Required]
    [MaxLength(64)]
    public string RefreshTokenHash { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(30);

    public bool Revoked { get; set; }

    /// <summary>
    /// </summary>
    [ForeignKey("DistributorAccountId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual DistributorAccount DistributorAccount { get; set; }

    [NotMapped]
    public IPAddress? IPAddress
    {
        get => IpAddressBytes == null ? null : new(IpAddressBytes);
        set => IpAddressBytes = value == null ? null : value.GetAddressBytes();
    }
}
