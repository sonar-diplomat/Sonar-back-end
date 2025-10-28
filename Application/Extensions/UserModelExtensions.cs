using Application.DTOs.User;
using Entities.Models.Access;
using Entities.Models.Chat;
using Entities.Models.UserCore;

namespace Application.Extensions
{
    public static class UserModelExtensions
    {
        public static NonSensetiveUserDTO ToNonSensetiveUserDTO(this User user)
        {
            return new NonSensetiveUserDTO
            {
                Biography = user.Biography,
                PublicIdentifier = user.PublicIdentifier,
                RegistrationDate = user.RegistrationDate,
                AvatarImageId = user.AvatarImageId,
                AvatarImage = user.AvatarImage,
                Posts = user.Posts?.ToList() ?? new List<Post>(),
                AccessFeatures = user.AccessFeatures?.ToList() ?? new List<AccessFeature>()
            };
        }
    }
}
