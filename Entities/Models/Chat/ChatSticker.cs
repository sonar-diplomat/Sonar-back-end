using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.File;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Chat;

[Table("ChatSticker")]
public class ChatSticker : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public int ImageFileId { get; set; }

    public int? CategoryId { get; set; }

    [JsonIgnore]
    [ForeignKey("ImageFileId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual ImageFile ImageFile { get; set; }

    [JsonIgnore]
    [ForeignKey("CategoryId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual ChatStickerCategory? Category { get; set; }
}

