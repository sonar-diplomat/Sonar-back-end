using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.UserCore;

[Table("RefreshTokens")]
public class RefreshToken : BaseModel
{
    [Required]
    [MaxLength(128)]
    public string Token { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(7);

    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = null!;
}
