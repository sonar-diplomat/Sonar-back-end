using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("PlaybackQuality")]
    public class PlaybackQuality
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public int BitRate { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
    }
}