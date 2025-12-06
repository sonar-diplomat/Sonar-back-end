using Application.Abstractions.Interfaces.Services.Chat;
using Application.DTOs.Chat;
using Application.Response;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Chat;

[Route("api/[controller]")]
[ApiController]
public class ChatStickerController(IChatStickerService chatStickerService) : ControllerBase
{
    /// <summary>
    /// Retrieves all available chat stickers.
    /// </summary>
    /// <returns>List of chat stickers with their image file IDs.</returns>
    /// <response code="200">Stickers retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<ChatStickerDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChatStickers()
    {
        IEnumerable<ChatStickerDTO> stickers = await chatStickerService.GetAllAsync();
        throw ResponseFactory.Create<OkResponse<IEnumerable<ChatStickerDTO>>>(stickers, ["Stickers retrieved successfully"]);
    }
}

