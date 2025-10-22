using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.UserCore;

[Table("UserSession")]
public class UserSession : BaseModel
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
    public int UserId { get; set; }

    //
    [Required]
    [MaxLength(64)]
    public string RefreshTokenHash { get; set; }


    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(30);

    public bool Revoked { get; set; }
    //


    /// <summary>
    /// </summary>
    [ForeignKey("UserId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public User User { get; set; }

    [NotMapped]
    public IPAddress IPAddress
    {
        get => new(IpAddressBytes);
        set => IpAddressBytes = value.GetAddressBytes();
    }
}
