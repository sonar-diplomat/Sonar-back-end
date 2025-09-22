using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure;

namespace Entities.Models
{
    [Table("Language")]
    public class Language : BaseModel
    {
        [Required, MaxLength(10)]
        public string Locale { get; set; }
        [Required, MaxLength(150)]
        public string Name { get; set; }
        [Required, MaxLength(150)]
        public string NativeName { get; set; }
    }
}