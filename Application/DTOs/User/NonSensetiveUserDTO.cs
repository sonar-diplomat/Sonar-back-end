using Entities.Models.Access;
using Entities.Models.Chat;
using Entities.Models.File;

namespace Application.DTOs.User
{

    public record NonSensetiveUserDTO
    {
        public string? Biography { get; set; } // md 
        public string PublicIdentifier { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int AvatarImageId { get; set; }
        public virtual ImageFile AvatarImage { get; set; }
        //public virtual Artist Artist { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<AccessFeature> AccessFeatures { get; set; }
    }
}
