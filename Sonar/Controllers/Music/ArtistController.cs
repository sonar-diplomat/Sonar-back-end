using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs;
using Application.DTOs.Music;
using Application.DTOs.User;
using Application.Response;
using Entities.Enums;
using Entities.Models.Chat;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class ArtistController(
    UserManager<User> userManager,
    IShareService shareService,
    IArtistService artistService,
    IPostService postService)
    : ShareController<Artist>(userManager, shareService)
{
    [Authorize]
    private async Task<bool> CheckManageAccess(int artistId)
    {
        User user = await CheckAccessFeatures([]);
        if (user.Id == artistId) return false;
        return user.AccessFeatures.Any(af => af.Name is AccessFeatureStruct.IamAGod or AccessFeatureStruct.ManageUsers)
            ? true
            : throw ResponseFactory.Create<ForbiddenResponse>(["You do not have permission to perform this action"]);
    }

    /// <summary>
    /// Registers a new artist account on the platform.
    /// </summary>
    /// <returns>Success response upon registration.</returns>
    /// <response code="200">Artist registered successfully.</response>
    /// <response code="501">Not yet implemented.</response>
    /// <remarks>
    /// This endpoint is currently under development.
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public async Task<IActionResult> RegisterArtist()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets an artist by their ID including user information.
    /// </summary>
    /// <param name="artistId">The ID of the artist to retrieve.</param>
    /// <returns>Artist details with user information.</returns>
    /// <response code="200">Artist retrieved successfully.</response>
    /// <response code="404">Artist not found.</response>
    [HttpGet("{artistId:int}")]
    [ProducesResponseType(typeof(OkResponse<ArtistDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArtistById(int artistId)
    {
        Artist? artist = await artistService.GetByIdWithUserAsync(artistId);
        if (artist == null)
            throw ResponseFactory.Create<NotFoundResponse>([$"Artist with ID {artistId} not found"]);

        var artistDTO = new ArtistDTO
        {
            Id = artist.Id,
            UserId = artist.UserId,
            ArtistName = artist.ArtistName,
            User = artist.User != null ? new UserResponseDTO
            {
                Id = artist.User.Id,
                UserName = artist.User.UserName,
                FirstName = artist.User.FirstName,
                LastName = artist.User.LastName,
                DateOfBirth = artist.User.DateOfBirth,
                Login = artist.User.Login,
                PublicIdentifier = artist.User.PublicIdentifier,
                Biography = artist.User.Biography,
                RegistrationDate = artist.User.RegistrationDate,
                AvailableCurrency = artist.User.AvailableCurrency,
                AvatarImageId = artist.User.AvatarImageId
            } : null
        };

        throw ResponseFactory.Create<OkResponse<ArtistDTO>>(artistDTO, ["Artist retrieved successfully"]);
    }

    /// <summary>
    /// Gets all posts for a specific artist.
    /// </summary>
    /// <param name="artistId">The ID of the artist whose posts to retrieve.</param>
    /// <returns>Collection of posts for the specified artist.</returns>
    /// <response code="200">Posts retrieved successfully.</response>
    /// <response code="404">Artist not found.</response>
    [HttpGet("{artistId:int}/posts")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<Post>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArtistPosts(int artistId)
    {
        Artist? artist = await artistService.GetByIdAsync(artistId);
        if (artist == null)
            throw ResponseFactory.Create<NotFoundResponse>([$"Artist with ID {artistId} not found"]);

        IEnumerable<Post> posts = await postService.GetByArtistIdAsync(artistId);
        throw ResponseFactory.Create<OkResponse<IEnumerable<Post>>>(posts, ["Artist posts retrieved successfully"]);
    }

    /// <summary>
    /// Updates the name of an artist profile.
    /// </summary>
    /// <param name="artistId">The ID of the artist to update.</param>
    /// <param name="newArtistName">The new artist name.</param>
    /// <returns>Success response upon name update.</returns>
    /// <response code="200">Artist name updated successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">User not authorized to modify this artist.</response>
    /// <response code="404">Artist not found.</response>
    [HttpPut("{artistId:int}/update-name")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateArtistName(int artistId, [FromBody] string newArtistName)
    {
        await CheckManageAccess(artistId);
        await artistService.UpdateNameAsync(artistId, newArtistName);
        throw ResponseFactory.Create<OkResponse>([$"Artist with ID {artistId} successfully updated"]);
    }

    /// <summary>
    /// Deletes the authenticated user's artist account.
    /// </summary>
    /// <returns>Success response upon deletion.</returns>
    /// <response code="200">Artist deleted successfully.</response>
    /// <response code="401">User not authenticated or lacks 'ManageUsers' access feature.</response>
    /// <response code="404">Artist not found.</response>
    /// <remarks>
    /// Requires 'ManageUsers' access feature.
    /// </remarks>
    [HttpDelete]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteArtist()
    {
        int artistId = (await CheckAccessFeatures([AccessFeatureStruct.ManageUsers])).Id;
        await artistService.DeleteArtistAsync(artistId);
        throw ResponseFactory.Create<OkResponse>([$"Artist with ID {artistId} successfully deleted"]);
    }

    /// <summary>
    /// Creates a new post for the authenticated artist.
    /// </summary>
    /// <param name="dto">Post data including content, media, and visibility settings.</param>
    /// <returns>Success response upon post creation.</returns>
    /// <response code="200">Post created successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="400">Invalid post data.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePost(PostDTO dto)
    {
        int artistId = (await CheckAccessFeatures([])).Id;
        await artistService.CreatePostAsync(artistId, dto);
        throw ResponseFactory.Create<OkResponse>([$"Post successfully created for artist with ID {artistId}"]);
    }

    /// <summary>
    /// Deletes a specific artist post.
    /// </summary>
    /// <param name="postId">The ID of the post to delete.</param>
    /// <returns>Success response upon deletion.</returns>
    /// <response code="200">Post deleted successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">User not authorized to delete this post.</response>
    /// <response code="404">Post not found.</response>
    [HttpDelete("post/{postId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbiddenResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost(int postId)
    {
        Post post = await postService.GetByIdValidatedAsync(postId);
        await CheckManageAccess(post.ArtistId);
        await postService.DeleteAsync(postId);
        throw ResponseFactory.Create<OkResponse>([$"Post with ID {postId} successfully deleted"]);
    }

    /// <summary>
    /// Updates an existing artist post.
    /// </summary>
    /// <param name="postId">The ID of the post to update.</param>
    /// <param name="dto">Updated post data.</param>
    /// <returns>Success response upon update.</returns>
    /// <response code="200">Post updated successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Post not found.</response>
    /// <response code="400">Invalid post data.</response>
    [HttpPut("post/{postId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePost(int postId, PostDTO dto)
    {
        int artistId = (await CheckAccessFeatures([])).Id;
        await artistService.UpdatePostAsync(postId, artistId, dto);
        throw ResponseFactory.Create<OkResponse>([$"Post successfully created for artist with ID {artistId}"]);
    }

    /// <summary>
    /// Changes the visibility status of an artist post.
    /// </summary>
    /// <param name="postId">The ID of the post to update.</param>
    /// <param name="visibilityStatusId">The new visibility status ID.</param>
    /// <returns>Success response upon status change.</returns>
    /// <response code="200">Post visibility updated successfully.</response>
    /// <response code="401">User not authorized (requires 'ManageUsers' feature).</response>
    /// <response code="404">Post not found.</response>
    [HttpPut("{postId:int}/visibility")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePostVisibilityStatus(int postId, int visibilityStatusId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageUsers]);
        await postService.UpdateVisibilityStatusAsync(postId, visibilityStatusId);
        throw ResponseFactory.Create<OkResponse>(["Track visibility status was changed successfully"]);
    }
}