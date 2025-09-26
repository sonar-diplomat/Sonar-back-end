using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models.Distribution;

[Table("License")]
public class License : BaseModel
{
    [Required]
    public DateTime IssuingDate { get; set; }
    [Required]
    public DateTime ExpirationDate { get; set; }
    
    [Required]
    public int IssuerId { get; set; }
    
    [ForeignKey("IssuerId")]
    public virtual UserCore.User Issuer { get; set; }
}