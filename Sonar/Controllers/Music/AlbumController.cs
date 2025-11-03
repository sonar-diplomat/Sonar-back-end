using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs.Music;
using Application.Response;
using Entities.Enums;
using Entities.Models.Distribution;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sonar.Extensions;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class AlbumController(
    UserManager<User> userManager,
    IDistributorAccountService accountService,
    IDistributorService distributorService,
    IAlbumService albumService,
    IImageFileService imageFileService,
    ITrackService trackService,
    ICollectionService<Album> collectionService,
    IShareService shareService
)
    : CollectionController<Album>(userManager, collectionService)
{

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UploadAlbum(UploadAlbumDTO dto)
    {
        int distributorId = (await this.CheckDistributorAsync()).Id;
        Album album = await albumService.UploadAsync(dto, distributorId);
        AlbumResponseDTO responseDto = new AlbumResponseDTO
        {
            Id = album.Id,
            Name = album.Name,
            CoverUrl = album.Cover?.Url ?? string.Empty,
            DistributorName = album.Distributor?.Name ?? string.Empty,
            TrackCount = album.Tracks?.Count ?? 0
        };
        throw ResponseFactory.Create<OkResponse<AlbumResponseDTO>>(responseDto, ["Album was created successfully"]);
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
        AlbumResponseDTO responseDto = new AlbumResponseDTO
        {
            Id = album.Id,
            Name = album.Name,
            CoverUrl = album.Cover?.Url ?? string.Empty,
            DistributorName = album.Distributor?.Name ?? string.Empty,
            TrackCount = album.Tracks?.Count ?? 0
        };
        throw ResponseFactory.Create<OkResponse<AlbumResponseDTO>>(responseDto, ["Album name was updated successfully"]);
    }

    [HttpPost("{albumId:int}/add")]
    [Authorize]
    public async Task<IActionResult> UploadTrack(int albumId, UploadTrackDTO dto)
    {
        await MatchAlbumAndDistributor(albumId);
        // TODO: create DTO
        Track track = await trackService.CreateTrackAsync(albumId, dto);
        throw ResponseFactory.Create<OkResponse<Track>>(track, ["Track was added successfully"]);
    }

    [HttpPut("{albumId:int}/visibility")]
    [Authorize]
    public async Task<IActionResult> UpdateVisibilityStatus(int albumId, int visibilityStateId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageContent]);
        await albumService.UpdateVisibilityStateAsync(albumId, visibilityStateId);

        throw ResponseFactory.Create<OkResponse>(["Album visibility state was changed successfully"]);
    }

    [HttpPut("{albumId:int}/cover")]
    [Authorize]
    public async Task<IActionResult> UpdateAlbumCover(int albumId, IFormFile file)
    {
        await MatchAlbumAndDistributor(albumId);
        await imageFileService.UploadFileAsync(file);
        throw ResponseFactory.Create<OkResponse>(["Album cover was updated successfully"]);
    }

    private async Task MatchAlbumAndDistributor(int albumId)
    {
        Distributor distributor = await this.CheckDistributorAsync();
        Album album = await albumService.GetByIdValidatedAsync(albumId);
        if (album.DistributorId != distributor.Id)
            throw ResponseFactory.Create<UnauthorizedResponse>([""]);
    }
}