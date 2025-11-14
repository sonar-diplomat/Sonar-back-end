using Application.DTOs.Access;

namespace Application.DTOs.User;

public record NonSensetiveUserDTO
{
    public string? Biography { get; set; } // md 
    public string PublicIdentifier { get; set; }
    public DateTime RegistrationDate { get; set; }
    public int AvatarImageId { get; set; }

    public string ImageUrl { get; set; }

    //public virtual Artist Artist { get; set; }
    public ICollection<AccessFeatureDTO> AccessFeatures { get; set; }
}