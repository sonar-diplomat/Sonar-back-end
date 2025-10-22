using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Distribution;

[Table("License")]
public class License : BaseModel
{
    [Required]
    public DateTime IssuingDate { get; set; }
    
    [Required]
    public DateTime LastUpdatedDate { get; set; }

    [Required]
    public DateTime ExpirationDate { get; set; }
    
    [Required]
    [Length(32, 32)]
    [MaxLength(32)]
    public string LicenseKey { get; set; }

    [Required]
    public int IssuerId { get; set; }

    [ForeignKey("IssuerId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual User Issuer { get; set; }
}
