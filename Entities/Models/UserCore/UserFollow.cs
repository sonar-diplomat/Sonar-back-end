using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.UserCore;

[Table("UserFollows")]
[Index(nameof(FollowerId), nameof(FollowingId), IsUnique = true)]
public class UserFollow : BaseModel
{
    [Required]
    public int FollowerId { get; set; }

    [Required]
    public int FollowingId { get; set; }

    [Required]
    public DateTime FollowedAt { get; set; }

    [JsonIgnore]
    [ForeignKey("FollowerId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual User Follower { get; set; }

    [JsonIgnore]
    [ForeignKey("FollowingId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual User Following { get; set; }
}

