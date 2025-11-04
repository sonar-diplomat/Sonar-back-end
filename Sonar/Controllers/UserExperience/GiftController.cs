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
    // TODO: write XML comments and returnType attributes
    [HttpPost("send")]
    [Authorize]
    public async Task<ActionResult<Gift>> SendGift([FromBody] SendGiftDTO giftDto)
    {
        User user = await CheckAccessFeatures([]);
        if (user.Id != giftDto.BuyerId)
            ResponseFactory.Create<BadRequestResponse>();
        Gift gift = await giftService.SendGiftAsync(giftDto);
        return CreatedAtAction(nameof(GetGift), new { id = gift.Id }, gift);
    }

    // TODO: write XML comments and returnType attributes
    [HttpPost("{id}/accept")]
    [Authorize]
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
                PublicIdentifier = payment.Buyer.PublicIdentifier,
                Biography = payment.Buyer.Biography,
                RegistrationDate = payment.Buyer.RegistrationDate,
                AvatarUrl = payment.Buyer.AvatarImage?.Url ?? string.Empty
            },
            SubscriptionPackId = payment.SubscriptionPackId,
            SubscriptionPackName = payment.SubscriptionPack?.Name ?? string.Empty
        };
        throw ResponseFactory.Create<OkResponse<SubscriptionPaymentDTO>>(dto,
            ["Gift accepted and subscription activated."]);
    }

    // TODO: write XML comments and returnType attributes
    [HttpGet("received")]
    [Authorize]
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

    // TODO: write XML comments and returnType attributes
    [HttpGet("sent")]
    [Authorize]
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

    // TODO: write XML comments and returnType attributes
    [HttpGet("{id}")]
    [Authorize]
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


    // TODO: write XML comments and returnType attributes
    [HttpDelete("{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelGift(int id)
    {
        User user = await CheckAccessFeatures([]);
        await giftService.CancelGiftAsync(id, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Gift cancelled"]);
    }

    #region Gift Style Endpoints

    // TODO: write XML comments and returnType attributes
    [HttpGet("styles")]
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

    // TODO: write XML comments and returnType attributes
    [HttpGet("styles/{id}")]
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