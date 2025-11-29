using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.User;
using Application.DTOs.UserExperience;
using Application.Response;
using Entities.Models.UserCore;
using Entities.Models.UserExperience;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.UserExperience;

[Route("api/[controller]")]
[ApiController]
public class GiftController(
    IGiftService giftService,
    IGiftStyleService giftStyleService,
    UserManager<User> userManager
)
    : BaseController(userManager)
{
    /// <summary>
    /// Sends a gift with a subscription pack to another user.
    /// </summary>
    /// <param name="giftDto">Gift data including buyer ID, receiver ID, subscription pack, and message.</param>
    /// <returns>Created gift entity.</returns>
    /// <response code="201">Gift sent successfully.</response>
    /// <response code="400">Invalid buyer ID or gift data.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpPost("send")]
    [Authorize]
    [ProducesResponseType(typeof(Gift), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Gift>> SendGift([FromBody] SendGiftDTO giftDto)
    {
        User user = await CheckAccessFeatures([]);
        if (user.Id != giftDto.BuyerId)
            ResponseFactory.Create<BadRequestResponse>();
        Gift gift = await giftService.SendGiftAsync(giftDto);
        return CreatedAtAction(nameof(GetGift), new { id = gift.Id }, gift);
    }

    /// <summary>
    /// Accepts a gift and activates the associated subscription.
    /// </summary>
    /// <param name="id">The ID of the gift to accept.</param>
    /// <returns>Subscription payment DTO with activated subscription details.</returns>
    /// <response code="200">Gift accepted and subscription activated.</response>
    /// <response code="401">User not authenticated or not the gift recipient.</response>
    /// <response code="404">Gift not found.</response>
    [HttpPost("{id}/accept")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<SubscriptionPaymentDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionPaymentDTO>> AcceptGift(int id)
    {
        User user = await CheckAccessFeatures([]);
        SubscriptionPayment payment = await giftService.AcceptGiftAsync(id, user.Id);
        SubscriptionPaymentDTO dto = new()
        {
            Id = payment.Id,
            Amount = payment.Amount,
            CreatedAt = payment.CreatedAt,
            Buyer = new UserResponseDTO
            {
                Id = payment.Buyer.Id,
                UserName = payment.Buyer.UserName,
                FirstName = payment.Buyer.FirstName,
                LastName = payment.Buyer.LastName,
                DateOfBirth = payment.Buyer.DateOfBirth,
                Login = payment.Buyer.Login,
                PublicIdentifier = payment.Buyer.PublicIdentifier,
                Biography = payment.Buyer.Biography,
                RegistrationDate = payment.Buyer.RegistrationDate,
                AvailableCurrency = payment.Buyer.AvailableCurrency,
                AvatarImageId = payment.Buyer.AvatarImageId
            },
            SubscriptionPackId = payment.SubscriptionPackId,
            SubscriptionPackName = payment.SubscriptionPack?.Name ?? string.Empty
        };
        throw ResponseFactory.Create<OkResponse<SubscriptionPaymentDTO>>(dto,
            ["Gift accepted and subscription activated."]);
    }

    /// <summary>
    /// Retrieves all gifts received by the authenticated user.
    /// </summary>
    /// <returns>List of gift response DTOs.</returns>
    /// <response code="200">Received gifts retrieved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet("received")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<GiftResponseDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<GiftResponseDTO>>> GetReceivedGifts()
    {
        User user = await CheckAccessFeatures([]);
        IEnumerable<Gift> gifts = await giftService.GetReceivedGiftsAsync(user.Id);
        IEnumerable<GiftResponseDTO> dtos = gifts.Select(g => new GiftResponseDTO
        {
            Id = g.Id,
            Title = g.Title,
            TextContent = g.TextContent,
            GiftTime = g.GiftTime,
            AcceptanceDate = g.AcceptanceDate,
            ReceiverName = g.Receiver?.UserName ?? string.Empty,
            GiftStyleName = g.GiftStyle?.Name ?? string.Empty,
            SubscriptionAmount = g.SubscriptionPayment?.Amount ?? 0,
            SubscriptionPackName = g.SubscriptionPayment?.SubscriptionPack?.Name ?? string.Empty
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<GiftResponseDTO>>>(dtos, ["Received gifts retrieved successfully."]);
    }

    /// <summary>
    /// Retrieves all gifts sent by the authenticated user.
    /// </summary>
    /// <param name="senderId">The ID of the sender (must match authenticated user).</param>
    /// <returns>List of gift response DTOs.</returns>
    /// <response code="200">Sent gifts retrieved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet("sent")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<GiftResponseDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<GiftResponseDTO>>> GetSentGifts(int senderId)
    {
        User user = await CheckAccessFeatures([]);
        IEnumerable<Gift> gifts = await giftService.GetSentGiftsAsync(user.Id);
        IEnumerable<GiftResponseDTO> dtos = gifts.Select(g => new GiftResponseDTO
        {
            Id = g.Id,
            Title = g.Title,
            TextContent = g.TextContent,
            GiftTime = g.GiftTime,
            AcceptanceDate = g.AcceptanceDate,
            ReceiverName = g.Receiver?.UserName ?? string.Empty,
            GiftStyleName = g.GiftStyle?.Name ?? string.Empty,
            SubscriptionAmount = g.SubscriptionPayment?.Amount ?? 0,
            SubscriptionPackName = g.SubscriptionPayment?.SubscriptionPack?.Name ?? string.Empty
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<GiftResponseDTO>>>(dtos, ["Sent gifts retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves detailed information about a specific gift.
    /// </summary>
    /// <param name="id">The ID of the gift to retrieve.</param>
    /// <returns>Gift response DTO with full details.</returns>
    /// <response code="200">Gift retrieved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Gift not found.</response>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<GiftResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GiftResponseDTO>> GetGift(int id)
    {
        Gift gift = await giftService.GetByIdValidatedAsync(id);
        GiftResponseDTO dto = new()
        {
            Id = gift.Id,
            Title = gift.Title,
            TextContent = gift.TextContent,
            GiftTime = gift.GiftTime,
            AcceptanceDate = gift.AcceptanceDate,
            ReceiverName = gift.Receiver?.UserName ?? string.Empty,
            GiftStyleName = gift.GiftStyle?.Name ?? string.Empty,
            SubscriptionAmount = gift.SubscriptionPayment?.Amount ?? 0,
            SubscriptionPackName = gift.SubscriptionPayment?.SubscriptionPack?.Name ?? string.Empty
        };
        throw ResponseFactory.Create<OkResponse<GiftResponseDTO>>(dto, ["Gift retrieved successfully"]);
    }


    /// <summary>
    /// Cancels a sent gift before it has been accepted.
    /// </summary>
    /// <param name="id">The ID of the gift to cancel.</param>
    /// <returns>Success response upon cancellation.</returns>
    /// <response code="200">Gift cancelled successfully.</response>
    /// <response code="401">User not authenticated or not the gift sender.</response>
    /// <response code="400">Gift has already been accepted.</response>
    [HttpDelete("{id}/cancel")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelGift(int id)
    {
        User user = await CheckAccessFeatures([]);
        await giftService.CancelGiftAsync(id, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Gift cancelled"]);
    }

    #region Gift Style Endpoints

    /// <summary>
    /// Retrieves all available gift styles.
    /// </summary>
    /// <returns>List of gift style DTOs.</returns>
    /// <response code="200">Gift styles retrieved successfully.</response>
    [HttpGet("styles")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<GiftStyleDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GiftStyleDTO>>> GetAllStyles()
    {
        IEnumerable<GiftStyle> styles = await giftStyleService.GetAllAsync();
        IEnumerable<GiftStyleDTO> dtos = styles.Select(s => new GiftStyleDTO
        {
            Id = s.Id,
            Name = s.Name
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<GiftStyleDTO>>>(dtos,
            ["Gift styles retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves a specific gift style by its ID.
    /// </summary>
    /// <param name="id">The ID of the gift style to retrieve.</param>
    /// <returns>Gift style DTO.</returns>
    /// <response code="200">Gift style retrieved successfully.</response>
    /// <response code="404">Gift style not found.</response>
    [HttpGet("styles/{id}")]
    [ProducesResponseType(typeof(OkResponse<GiftStyleDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GiftStyleDTO>> GetStyle(int id)
    {
        GiftStyle style = await giftStyleService.GetByIdValidatedAsync(id);
        GiftStyleDTO dto = new()
        {
            Id = style.Id,
            Name = style.Name
        };
        throw ResponseFactory.Create<OkResponse<GiftStyleDTO>>(dto, ["Gift style retrieved successfully"]);
    }

    #endregion
}