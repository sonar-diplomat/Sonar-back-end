using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Models.UserCore;
using Entities.Models.UserExperience;
using Entities.TemplateResponses;
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
        User user = await GetUserByJwt();
        if (user.Id != giftDto.BuyerId)
            AppExceptionFactory.Create<BadRequestException>();
        Gift gift = await giftService.SendGiftAsync(giftDto);
        return CreatedAtAction(nameof(GetGift), new { id = gift.Id }, gift);
    }

    [HttpPost("{id}/accept")]
    [Authorize]
    public async Task<ActionResult<SubscriptionPayment>> AcceptGift(int id)
    {
        User user = await GetUserByJwt();
        SubscriptionPayment payment = await giftService.AcceptGiftAsync(id, user.Id);
        return Ok(new BaseResponse<SubscriptionPayment>(payment, "Gift accepted and subscription activated."));
    }

    [HttpGet("received")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Gift>>> GetReceivedGifts()
    {
        User user = await GetUserByJwt();
        IEnumerable<Gift> gifts = await giftService.GetReceivedGiftsAsync(user.Id);
        return Ok(new BaseResponse<IEnumerable<Gift>>(gifts, "Received gifts retrieved successfully."));
    }

    [HttpGet("sent")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Gift>>> GetSentGifts(int senderId)
    {
        User user = await GetUserByJwt();
        IEnumerable<Gift> gifts = await giftService.GetSentGiftsAsync(user.Id);
        return Ok(new BaseResponse<IEnumerable<Gift>>(gifts, "Sent gifts retrieved successfully"));
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Gift>> GetGift(int id)
    {
        Gift gift = await giftService.GetByIdValidatedAsync(id);
        return Ok(new BaseResponse<Gift>(gift, "Gift retrieved successfully"));
    }


    [HttpDelete("{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelGift(int id)
    {
        User user = await GetUserByJwt();
        await giftService.CancelGiftAsync(id, user.Id);
        return Ok(new BaseResponse<bool>("Gift cancelled"));
    }

    #region Gift Style Endpoints

    [HttpGet("styles")]
    public async Task<ActionResult<IEnumerable<GiftStyle>>> GetAllStyles()
    {
        IEnumerable<GiftStyle> styles = await giftStyleService.GetAllAsync();
        return Ok(new BaseResponse<IEnumerable<GiftStyle>>(styles, "Gift styles retrieved successfully"));
    }

    [HttpGet("styles/{id}")]
    public async Task<ActionResult<GiftStyle>> GetStyle(int id)
    {
        GiftStyle style = await giftStyleService.GetByIdValidatedAsync(id);
        return Ok(new BaseResponse<GiftStyle>(style, "Gift style retrieved successfully"));
    }

    #endregion
}
