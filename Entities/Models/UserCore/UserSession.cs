using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

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

    /// <summary>
    /// </summary>
    [ForeignKey("UserId")]
    public User User { get; set; }

    [NotMapped]
    public IPAddress IPAddress
    {
        get => new(IpAddressBytes);
        set => IpAddressBytes = value.GetAddressBytes();
    }
}