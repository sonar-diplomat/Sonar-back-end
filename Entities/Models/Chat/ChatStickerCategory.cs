using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Chat;

[Table("ChatStickerCategory")]
public class ChatStickerCategory : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [JsonIgnore]
    public virtual ICollection<ChatSticker> Stickers { get; set; }
}

