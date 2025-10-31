using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs;
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

    [HttpPost("register")]
    public async Task<IActionResult> RegisterArtist()
    {
        throw new NotImplementedException();
    }

    [HttpPut("{artistId:int}/update-name")]
    [Authorize]
    public async Task<IActionResult> UpdateArtistName(int artistId, [FromBody] string newArtistName)
    {
        await CheckManageAccess(artistId);
        await artistService.UpdateNameAsync(artistId, newArtistName);
        throw ResponseFactory.Create<OkResponse>([$"Artist with ID {artistId} successfully updated"]);
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteArtist()
    {
        int artistId = (await CheckAccessFeatures([AccessFeatureStruct.ManageUsers])).Id;
        await artistService.DeleteArtistAsync(artistId);
        throw ResponseFactory.Create<OkResponse>([$"Artist with ID {artistId} successfully deleted"]);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePost(PostDTO dto)
    {
        int artistId = (await CheckAccessFeatures([])).Id;
        await artistService.CreatePostAsync(artistId, dto);
        throw ResponseFactory.Create<OkResponse>([$"Post successfully created for artist with ID {artistId}"]);
    }

    [HttpDelete("post/{postId:int}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(int postId)
    {
        Post post = await postService.GetByIdValidatedAsync(postId);
        await CheckManageAccess(post.ArtistId);
        await postService.DeleteAsync(postId);
        throw ResponseFactory.Create<OkResponse>([$"Post with ID {postId} successfully deleted"]);
    }

    [HttpPut("post/{postId:int}")]
    [Authorize]
    public async Task<IActionResult> UpdatePost(int postId, PostDTO dto)
    {
        int artistId = (await CheckAccessFeatures([])).Id;
        await artistService.UpdatePostAsync(postId, artistId, dto);
        throw ResponseFactory.Create<OkResponse>([$"Post successfully created for artist with ID {artistId}"]);
    }

    [HttpPut("{postId:int}/visibility")]
    [Authorize]
    public async Task<IActionResult> ChangePostVisibilityStatus(int postId, int visibilityStatusId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageUsers]);
        await postService.UpdateVisibilityStatusAsync(postId, visibilityStatusId);
        throw ResponseFactory.Create<OkResponse>(["Track visibility status was changed successfully"]);
    }
}