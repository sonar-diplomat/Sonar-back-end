using Application.Abstractions.Interfaces.Exception;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Models.UserExperience;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.UserExperience
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService giftService;
        private readonly IAppExceptionFactory<AppException> appExceptionFactory;

        public GiftController(IGiftService giftService, IAppExceptionFactory<AppException> appExceptionFactory)
        {
            this.giftService = giftService;
            this.appExceptionFactory = appExceptionFactory;
        }

        /// <summary>
        /// Send a gift subscription to another user
        /// </summary>
        /// <param name="giftDto">Gift details including subscription pack and receiver</param>
        /// <returns>Created gift</returns>
        [HttpPost("send")]
        public async Task<ActionResult<Gift>> SendGift([FromBody] SendGiftDTO giftDto)
        {
            try
            {
                Gift gift = await giftService.SendGiftAsync(giftDto);
                return CreatedAtAction(nameof(GetGift), new { id = gift.Id }, gift);
            }
            catch (Exception)
            {
                throw appExceptionFactory.CreateBadRequest(); //appExceptionFactory.CreateInternalServerError();
            }
        }

        /// <summary>
        /// Accept a gift and activate the subscription
        /// </summary>
        /// <param name="id">Gift ID</param>
        /// <param name="receiverId">ID of the user accepting the gift</param>
        /// <returns>Activated subscription payment</returns>
        [HttpPost("{id}/accept")]
        public async Task<ActionResult<SubscriptionPayment>> AcceptGift(int id, [FromBody] int receiverId)
        {
            try
            {
                SubscriptionPayment payment = await giftService.AcceptGiftAsync(id, receiverId);
                return Ok(payment);
            }
            catch (Exception)
            {
                throw appExceptionFactory.CreateBadRequest(); //appExceptionFactory.CreateInternalServerError();
            }
        }

        /// <summary>
        /// Get all gifts received by a user
        /// </summary>
        /// <param name="receiverId">ID of the receiver</param>
        /// <returns>List of gifts received</returns>
        [HttpGet("received/{receiverId}")]
        public async Task<ActionResult<IEnumerable<Gift>>> GetReceivedGifts(int receiverId)
        {
            try
            {
                IEnumerable<Gift> gifts = await giftService.GetReceivedGiftsAsync(receiverId);
                return Ok(gifts);
            }
            catch (Exception)
            {
                throw appExceptionFactory.CreateBadRequest(); //appExceptionFactory.CreateInternalServerError();
            }
        }

        /// <summary>
        /// Get all gifts sent by a user (including planned)
        /// </summary>
        /// <param name="senderId">ID of the sender</param>
        /// <returns>List of gifts sent</returns>
        [HttpGet("sent/{senderId}")]
        public async Task<ActionResult<IEnumerable<Gift>>> GetSentGifts(int senderId)
        {
            try
            {
                IEnumerable<Gift> gifts = await giftService.GetSentGiftsAsync(senderId);
                return Ok(gifts);
            }
            catch (Exception)
            {
                throw appExceptionFactory.CreateBadRequest(); //appExceptionFactory.CreateInternalServerError();
            }
        }

        /// <summary>
        /// Get a specific gift by ID
        /// </summary>
        /// <param name="id">Gift ID</param>
        /// <returns>Gift details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Gift>> GetGift(int id)
        {
            try
            {
                Gift gift = await giftService.GetGiftByIdAsync(id);

                if (gift == null)
                {
                    throw appExceptionFactory.CreateNotFound();
                }

                return Ok(gift);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception)
            {
                throw appExceptionFactory.CreateBadRequest(); //appExceptionFactory.CreateInternalServerError();
            }
        }

        /// <summary>
        /// Cancel a planned gift (only before it's accepted)
        /// </summary>
        /// <param name="id">Gift ID</param>
        /// <param name="senderId">ID of the sender</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> CancelGift(int id, [FromBody] int senderId)
        {
            try
            {
                bool result = await giftService.CancelGiftAsync(id, senderId);

                if (!result)
                {
                    throw appExceptionFactory.CreateBadRequest();
                }

                return NoContent();
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception)
            {
                throw appExceptionFactory.CreateBadRequest(); //appExceptionFactory.CreateInternalServerError();
            }
        }
    }
}
