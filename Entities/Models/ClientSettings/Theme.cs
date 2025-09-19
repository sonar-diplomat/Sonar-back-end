using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Theme")]
    public class Theme
    {
        [Key] 
        public int Id { get; set; }
        [Required, MaxLength(50)] 
        public string Name { get; set; }
        [MaxLength(500)] 
        public string? Description { get; set; }
    }
}