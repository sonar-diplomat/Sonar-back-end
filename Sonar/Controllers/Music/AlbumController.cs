using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Response;
using Entities.Models.Distribution;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class AlbumController(
    UserManager<User> userManager,
    IDistributorAccountService accountService,
    IDistributorService distributorService,
    IAlbumService albumService,
    IImageFileService imageFileService
) : BaseControllerExtended(userManager, accountService, distributorService)
{
    /*
     Upload
     Delete
     Update
     UploadTrack
     */

    [HttpPost]
    public async Task<IActionResult> UploadAlbum()
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{albumId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteAlbum(int albumId)
    {
        await MatchAlbumAndDistributor(albumId);
        await albumService.DeleteAsync(albumId);
        throw ResponseFactory.Create<OkResponse>(["Album was deleted successfully"]);
    }

    [HttpPut("{albumId:int}/name")]
    [Authorize]
    public async Task<IActionResult> UpdateAlbumName(int albumId, string name)
    {
        await MatchAlbumAndDistributor(albumId);
        Album album = await albumService.UpdateNameAsync(albumId, name);
        throw ResponseFactory.Create<OkResponse<Album>>(album, ["Album name was updated successfully"]);
    }

    [HttpPut("{albumId:int}/cover")]
    [Authorize]
    public async Task<IActionResult> UpdateAlbumCover(int albumId, IFormFile file)
    {
        await MatchAlbumAndDistributor(albumId);
        await imageFileService.UploadFileAsync(file);
        throw ResponseFactory.Create<OkResponse>(["Album cover was updated successfully"]);
    }

    [HttpPost("{albumId:int}/add")]
    [Authorize]
    public async Task<IActionResult> UploadTrack()
    {
        throw new NotImplementedException();
    }

    private async Task MatchAlbumAndDistributor(int albumId)
    {
        Distributor distributor = await CheckDistributor();
        Album album = await albumService.GetByIdValidatedAsync(albumId);
        if (album.DistributorId != distributor.Id)
            throw ResponseFactory.Create<UnauthorizedResponse>([""]);
    }
}