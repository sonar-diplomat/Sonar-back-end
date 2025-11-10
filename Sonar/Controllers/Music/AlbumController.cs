using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs.Music;
using Application.Response;
using Entities.Models.Distribution;
using Entities.Models.File;
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
    ICollectionService<Album> collectionService
)
    : CollectionController<Album>(userManager, collectionService)
{

    /// <summary>
    /// Uploads a new album to the platform.
    /// </summary>
    /// <param name="dto">Album upload data including name, release date, genre, and description.</param>
    /// <returns>Album response DTO with the created album details.</returns>
    /// <response code="200">Album created successfully.</response>
    /// <response code="401">User not authenticated or not a distributor.</response>
    /// <response code="400">Invalid album data.</response>
    /// <remarks>
    /// Requires distributor authentication. The album will be associated with the authenticated distributor.
    /// </remarks>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<AlbumResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadAlbum(UploadAlbumDTO dto)
    {
        int distributorId = (await this.CheckDistributorAsync()).Id;
        Album album = await albumService.UploadAsync(dto, distributorId);
        AlbumResponseDTO responseDto = new()
        {
            Id = album.Id,
            Name = album.Name,
            CoverUrl = album.Cover?.Url ?? string.Empty,
            DistributorName = album.Distributor?.Name ?? string.Empty,
            TrackCount = album.Tracks?.Count ?? 0
        };
        throw ResponseFactory.Create<OkResponse<AlbumResponseDTO>>(responseDto, ["Album was created successfully"]);
    }

    /// <summary>
    /// Deletes an album from the platform.
    /// </summary>
    /// <param name="albumId">The ID of the album to delete.</param>
    /// <returns>Success response upon deletion.</returns>
    /// <response code="200">Album deleted successfully.</response>
    /// <response code="401">User not authorized to delete this album.</response>
    /// <response code="404">Album not found.</response>
    [HttpDelete("{albumId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAlbum(int albumId)
    {
        await MatchAlbumAndDistributor(albumId);
        await albumService.DeleteAsync(albumId);
        throw ResponseFactory.Create<OkResponse>(["Album was deleted successfully"]);
    }

    /// <summary>
    /// Updates the name of an existing album.
    /// </summary>
    /// <param name="albumId">The ID of the album to update.</param>
    /// <param name="name">The new album name.</param>
    /// <returns>Updated album response DTO.</returns>
    /// <response code="200">Album name updated successfully.</response>
    /// <response code="401">User not authorized to update this album.</response>
    /// <response code="404">Album not found.</response>
    [HttpPut("{albumId:int}/name")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<AlbumResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAlbumName(int albumId, [FromQuery] string name)
    {
        await MatchAlbumAndDistributor(albumId);
        Album album = await albumService.UpdateNameAsync(albumId, name);
        AlbumResponseDTO responseDto = new()
        {
            Id = album.Id,
            Name = album.Name,
            CoverUrl = album.Cover?.Url ?? string.Empty,
            DistributorName = album.Distributor?.Name ?? string.Empty,
            TrackCount = album.Tracks?.Count ?? 0
        };
        throw ResponseFactory.Create<OkResponse<AlbumResponseDTO>>(responseDto, ["Album name was updated successfully"]);
    }

    /// <summary>
    /// Adds a new track to an existing album.
    /// </summary>
    /// <param name="albumId">The ID of the album to add the track to.</param>
    /// <param name="dto">Track upload data including title, duration, track number, and content flags.</param>
    /// <returns>Created track entity.</returns>
    /// <response code="200">Track added successfully.</response>
    /// <response code="401">User not authorized to modify this album.</response>
    /// <response code="404">Album not found.</response>
    [HttpPost("{albumId:int}/add")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<Track>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadTrack(int albumId, [FromForm] UploadTrackDTO dto)
    {
        await MatchAlbumAndDistributor(albumId);
        // TODO: create DTO
        Track track = await trackService.CreateTrackAsync(albumId, dto);
        throw ResponseFactory.Create<OkResponse<Track>>(track, ["Track was added successfully"]);
    }

    /// <summary>
    /// Updates the cover image of an album.
    /// </summary>
    /// <param name="albumId">The ID of the album to update.</param>
    /// <param name="file">The image file to use as the album cover.</param>
    /// <returns>Success response upon cover update.</returns>
    /// <response code="200">Album cover updated successfully.</response>
    /// <response code="401">User not authorized to modify this album.</response>
    /// <response code="400">Invalid image file.</response>
    /// <response code="404">Album not found.</response>
    [HttpPut("{albumId:int}/cover")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAlbumCover(int albumId, IFormFile file)
    {
        await MatchAlbumAndDistributor(albumId);
        ImageFile image = await imageFileService.UploadFileAsync(file);
        Album album = await albumService.GetByIdValidatedAsync(albumId);
        album.CoverId = image.Id;
        await albumService.UpdateAsync(album);
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