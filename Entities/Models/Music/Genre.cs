using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models.Music;

[Table("Genre")]
public class Genre : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [JsonIgnore]
    public virtual ICollection<Track> Tracks { get; set; }

    [JsonIgnore]
    public virtual ICollection<Album> Albums { get; set; }
}

