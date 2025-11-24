using Application.DTOs.Access;

namespace Application.DTOs.User;

public record NonSensetiveUserDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Login { get; set; }
    public string? Biography { get; set; } // md 
    public string PublicIdentifier { get; set; }
    public DateTime RegistrationDate { get; set; }
    public int AvailableCurrency { get; set; }
    public int AvatarImageId { get; set; }

    //public virtual Artist Artist { get; set; }
    public ICollection<AccessFeatureDTO> AccessFeatures { get; set; }
}