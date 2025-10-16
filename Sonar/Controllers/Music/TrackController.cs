using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Application.Services.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class TrackController(UserManager<User> userManager, ITrackService trackService) : BaseController(userManager)
{
    [HttpGet("stream")]
    [Authorize]
    public async Task<IActionResult> StreamMusic(int trackId)
    {
        string? rangeHeader = Request.Headers.Range.FirstOrDefault();
        MusicStreamResultDTO? result = await trackService.GetMusicStreamAsync(trackId, rangeHeader);

        if (result == null)
        {
            throw AppExceptionFactory.Create<TrackNotFoundException>();
        }

        result.GetStreamDetails(out Stream stream, out string contentType, out bool enableRangeProcessing);

        return File(stream, contentType, enableRangeProcessing: enableRangeProcessing);
    }
}
