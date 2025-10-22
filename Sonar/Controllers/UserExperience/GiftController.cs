using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
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

    [HttpPost("{id}/accept")]
    [Authorize]
    public async Task<ActionResult<SubscriptionPayment>> AcceptGift(int id)
    {
        User user = await CheckAccessFeatures([]);
        SubscriptionPayment payment = await giftService.AcceptGiftAsync(id, user.Id);
        throw ResponseFactory.Create<OkResponse<SubscriptionPayment>>(payment, ["Gift accepted and subscription activated."]);
    }

    [HttpGet("received")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Gift>>> GetReceivedGifts()
    {
        User user = await CheckAccessFeatures([]);
        IEnumerable<Gift> gifts = await giftService.GetReceivedGiftsAsync(user.Id);
        throw ResponseFactory.Create<OkResponse<IEnumerable<Gift>>>(gifts, ["Received gifts retrieved successfully."]);
    }

    [HttpGet("sent")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Gift>>> GetSentGifts(int senderId)
    {
        User user = await CheckAccessFeatures([]);
        IEnumerable<Gift> gifts = await giftService.GetSentGiftsAsync(user.Id);
        throw ResponseFactory.Create<OkResponse<IEnumerable<Gift>>>(gifts, ["Sent gifts retrieved successfully"]);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Gift>> GetGift(int id)
    {
        Gift gift = await giftService.GetByIdValidatedAsync(id);
        throw ResponseFactory.Create<OkResponse<Gift>>(gift, ["Gift retrieved successfully"]);
    }


    [HttpDelete("{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelGift(int id)
    {
        User user = await CheckAccessFeatures([]);
        await giftService.CancelGiftAsync(id, user.Id);
        throw ResponseFactory.Create<OkResponse>(["Gift cancelled"]);
    }

    #region Gift Style Endpoints

    [HttpGet("styles")]
    public async Task<ActionResult<IEnumerable<GiftStyle>>> GetAllStyles()
    {
        IEnumerable<GiftStyle> styles = await giftStyleService.GetAllAsync();
        throw ResponseFactory.Create<OkResponse<IEnumerable<GiftStyle>>>(styles, ["Gift styles retrieved successfully"]);
    }

    [HttpGet("styles/{id}")]
    public async Task<ActionResult<GiftStyle>> GetStyle(int id)
    {
        GiftStyle style = await giftStyleService.GetByIdValidatedAsync(id);
        throw ResponseFactory.Create<OkResponse<GiftStyle>>(style, ["Gift style retrieved successfully"]);
    }

    #endregion
}
