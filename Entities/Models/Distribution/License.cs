using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("License")]
public class License
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime IssuingDate { get; set; }
    [Required]
    public DateTime ExpirationDate { get; set; }
    
    [Required]
    public int IssuerId { get; set; }
    
    [ForeignKey("IssuerId")]
    public virtual User Issuer { get; set; }
}