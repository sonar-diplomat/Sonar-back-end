using Application;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Abstractions.Interfaces.Services.Utilities;
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
    IAlbumService albumService,
    IImageFileService imageFileService,
    ITrackService trackService,
    ICollectionService<Album> collectionService,
    IShareService shareService
)
    : CollectionController<Album>(userManager, collectionService, shareService)
{

    /// <summary>
    /// Gets all albums created by the current distributor.
    /// </summary>
    /// <returns>List of album response DTOs.</returns>
    /// <response code="200">Albums retrieved successfully.</response>
    /// <response code="401">User not authenticated or not a distributor.</response>
    /// <remarks>
    /// Requires distributor authentication. Returns only albums created by the authenticated distributor.
    /// </remarks>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<AlbumResponseDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAlbums()
    {
        int distributorId = (await this.CheckDistributorAsync()).Id;
        IEnumerable<Album> albums = await albumService.GetAlbumsByDistributorIdAsync(distributorId);
        
        IEnumerable<AlbumResponseDTO> albumDTOs = albums.Select(a => new AlbumResponseDTO
        {
            Id = a.Id,
            Name = a.Name,
            CoverId = a.CoverId,
            DistributorName = a.Distributor?.Name ?? string.Empty,
            TrackCount = a.Tracks?.Count ?? 0,
            Authors = a.AlbumArtists?.Select(aa => new AuthorDTO
            {
                Pseudonym = aa.Pseudonym,
                ArtistId = aa.ArtistId
            }).ToList() ?? new List<AuthorDTO>(),
            Genre = a.Genre != null ? new GenreDTO
            {
                Id = a.Genre.Id,
                Name = a.Genre.Name
            } : null,
            MoodTags = a.AlbumMoodTags?.Select(amt => new MoodTagDTO
            {
                Id = amt.MoodTag.Id,
                Name = amt.MoodTag.Name
            }).ToList() ?? new List<MoodTagDTO>()
        });

        throw ResponseFactory.Create<OkResponse<IEnumerable<AlbumResponseDTO>>>(albumDTOs);
    }

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
        Album albumWithArtists = await albumService.GetValidatedIncludeAlbumArtistsAsync(album.Id);
        AlbumResponseDTO responseDto = new()
        {
            Id = album.Id,
            Name = album.Name,
            CoverId = album.CoverId,
            DistributorName = album.Distributor?.Name ?? string.Empty,
            TrackCount = album.Tracks?.Count ?? 0,
            Authors = albumWithArtists.AlbumArtists?.Select(aa => new AuthorDTO
            {
                Pseudonym = aa.Pseudonym,
                ArtistId = aa.ArtistId
            }).ToList() ?? new List<AuthorDTO>()
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
        Album albumWithArtists = await albumService.GetValidatedIncludeAlbumArtistsAsync(albumId);
        AlbumResponseDTO responseDto = new()
        {
            Id = album.Id,
            Name = album.Name,
            CoverId = album.CoverId,
            DistributorName = album.Distributor?.Name ?? string.Empty,
            TrackCount = album.Tracks?.Count ?? 0,
            Authors = albumWithArtists.AlbumArtists?.Select(aa => new AuthorDTO
            {
                Pseudonym = aa.Pseudonym,
                ArtistId = aa.ArtistId
            }).ToList() ?? new List<AuthorDTO>()
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
    [RequestFormLimits(MultipartBodyLengthLimit = 104857600, ValueLengthLimit = 104857600)]
    [ProducesResponseType(typeof(OkResponse<Track>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadTrack(int albumId, [FromForm] UploadTrackDTO dto)
    {
        await MatchAlbumAndDistributor(albumId);
        // TODO: create DTO
        Track track = await albumService.CreateTrackAsync(albumId, dto);
        throw ResponseFactory.Create<OkResponse<Track>>(track, ["Track was added successfully"]);
    }

    /// <summary>
    /// Retrieves a specific album by its ID with basic information.
    /// </summary>
    /// <param name="albumId">The ID of the album to retrieve.</param>
    /// <returns>Album response DTO with album details.</returns>
    /// <response code="200">Album retrieved successfully.</response>
    /// <response code="404">Album not found.</response>
    [HttpGet("{albumId:int}")]
    [ProducesResponseType(typeof(OkResponse<AlbumResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAlbumById(int albumId)
    {
        int? userId = null;
        try
        {
            User? user = await GetUserByJwt();
            userId = user?.Id;
        }
        catch
        {
            
        }

        Album album = await albumService.GetValidatedIncludeVisibilityStateAsync(albumId, userId ?? 0);

        IEnumerable<int>? authorIds = album.AlbumArtists?
            .Where(aa => aa.Artist?.UserId != null)
            .Select(aa => aa.Artist!.UserId!)
            .ToList();

        VisibilityStateValidator.IsAccessible(album.VisibilityState, userId, authorIds, "Album", albumId);

        Album albumWithDistributor = await albumService.GetValidatedIncludeDistributorAsync(albumId);
        // album already has Genre and MoodTags loaded from GetValidatedIncludeVisibilityStateAsync

        AlbumResponseDTO responseDto = new()
        {
            Id = album.Id,
            Name = album.Name,
            CoverId = album.CoverId,
            DistributorName = albumWithDistributor.Distributor?.Name ?? string.Empty,
            TrackCount = album.Tracks?.Count ?? 0,
            Authors = album.AlbumArtists?.Select(aa => new AuthorDTO
            {
                Pseudonym = aa.Pseudonym,
                ArtistId = aa.ArtistId
            }).ToList() ?? new List<AuthorDTO>(),
            Genre = album.Genre != null ? new GenreDTO
            {
                Id = album.Genre.Id,
                Name = album.Genre.Name
            } : null,
            MoodTags = album.AlbumMoodTags?.Select(amt => new MoodTagDTO
            {
                Id = amt.MoodTag.Id,
                Name = amt.MoodTag.Name
            }).ToList() ?? new List<MoodTagDTO>()
        };
        throw ResponseFactory.Create<OkResponse<AlbumResponseDTO>>(responseDto, ["Album retrieved successfully"]);
    }

    /// <summary>
    /// Gets all tracks in an album (public endpoint).
    /// </summary>
    /// <param name="albumId">The ID of the album.</param>
    /// <returns>List of track DTOs.</returns>
    /// <response code="200">Album tracks retrieved successfully.</response>
    /// <response code="404">Album not found.</response>
    /// <remarks>
    /// Public endpoint. If user is authenticated, response may contain personalized information.
    /// </remarks>
    [HttpGet("{albumId:int}/tracks")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<TrackDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAlbumTracks(int albumId)
    {
        // Try to get userId if user is authenticated, but don't require authentication
        int? userId = null;
        try
        {
            User? user = await GetUserByJwt();
            userId = user?.Id;
        }
        catch
        {
            // User is not authenticated, userId remains null
        }

        // Validate album visibility
        Album album = await albumService.GetValidatedIncludeVisibilityStateAsync(albumId, userId ?? 0);
        IEnumerable<int>? authorIds = album.AlbumArtists?
            .Where(aa => aa.Artist?.UserId != null)
            .Select(aa => aa.Artist!.UserId!)
            .ToList();
        VisibilityStateValidator.IsAccessible(album.VisibilityState, userId, authorIds, "Album", albumId);
        
        IEnumerable<TrackDTO> tracks = await albumService.GetAlbumTracksAsync(albumId, userId);
        throw ResponseFactory.Create<OkResponse<IEnumerable<TrackDTO>>>(tracks, ["Album tracks retrieved successfully"]);
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
        await albumService.UpdateCoverAsync(albumId, image.Id);
        throw ResponseFactory.Create<OkResponse>(["Album cover was updated successfully"]);
    }

    /// <summary>
    /// Retrieves detailed information about a specific album for distributors, ignoring visibility state.
    /// </summary>
    /// <param name="albumId">The ID of the album to retrieve.</param>
    /// <returns>Album response DTO with full details.</returns>
    /// <response code="200">Album retrieved successfully.</response>
    /// <response code="404">Album not found.</response>
    /// <response code="401">User not authenticated or album does not belong to distributor.</response>
    /// <remarks>
    /// Requires distributor authentication. Only albums belonging to the authenticated distributor can be retrieved.
    /// Visibility state is ignored for distributors.
    /// </remarks>
    [HttpGet("distributor/{albumId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<AlbumResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAlbumByIdForDistributor(int albumId)
    {
        Distributor distributor = await this.CheckDistributorAsync();
        AlbumResponseDTO albumDto = await albumService.GetAlbumDtoForDistributorAsync(albumId, distributor.Id);
        throw ResponseFactory.Create<OkResponse<AlbumResponseDTO>>(albumDto, ["Album retrieved successfully"]);
    }

    /// <summary>
    /// Gets all tracks in an album for distributors, ignoring visibility state.
    /// </summary>
    /// <param name="albumId">The ID of the album.</param>
    /// <returns>List of track DTOs.</returns>
    /// <response code="200">Album tracks retrieved successfully.</response>
    /// <response code="404">Album not found.</response>
    /// <response code="401">User not authenticated or album does not belong to distributor.</response>
    /// <remarks>
    /// Requires distributor authentication. Only albums belonging to the authenticated distributor can be accessed.
    /// Visibility state is ignored for distributors.
    /// </remarks>
    [HttpGet("distributor/{albumId:int}/tracks")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<TrackDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAlbumTracksForDistributor(int albumId)
    {
        Distributor distributor = await this.CheckDistributorAsync();
        IEnumerable<TrackDTO> tracks = await albumService.GetAlbumTracksForDistributorAsync(albumId, distributor.Id);
        throw ResponseFactory.Create<OkResponse<IEnumerable<TrackDTO>>>(tracks, ["Album tracks retrieved successfully"]);
    }

    private async Task MatchAlbumAndDistributor(int albumId)
    {
        Distributor distributor = await this.CheckDistributorAsync();
        Album album = await albumService.GetByIdValidatedAsync(albumId);
        if (album.DistributorId != distributor.Id)
            throw ResponseFactory.Create<UnauthorizedResponse>([""]);
    }
}