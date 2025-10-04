
using Entities.Models.Music;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.UserCore
{
    [Table("Queue")]
    public class Queue : BaseModel
    {
        [Required]
        public TimeSpan Position { get; set; }
        [Required]
        public int CollectionId { get; set; }
        [ForeignKey("CollectionId")]
        public virtual Collection Collection { get; set; }

        public virtual UserState UserState { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }
    }
}
