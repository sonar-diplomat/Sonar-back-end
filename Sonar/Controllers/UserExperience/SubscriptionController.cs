using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Entities.Models.UserCore;
using Entities.Models.UserExperience;
using Entities.TemplateResponses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.UserExperience;

[Route("api/[controller]")]
[ApiController]
public class SubscriptionController(
    ISubscriptionPackService subscriptionPackService,
    ISubscriptionPaymentService subscriptionPaymentService,
    ISubscriptionFeatureService subscriptionFeatureService,
    UserManager<User> userManager)
    : BaseController(userManager)
{
    #region Subscription Pack Endpoints

    [HttpGet("packs")]
    public async Task<ActionResult<IEnumerable<SubscriptionPack>>> GetAllPacks()
    {
        IEnumerable<SubscriptionPack> packs = await subscriptionPackService.GetAllAsync();
        return Ok(new BaseResponse<IEnumerable<SubscriptionPack>>(packs, "Subscription packs retrieved successfully"));
    }

    [HttpGet("packs/{id}")]
    public async Task<ActionResult<SubscriptionPack>> GetPack(int id)
    {
        SubscriptionPack pack = await subscriptionPackService.GetByIdValidatedAsync(id);

        return Ok(new BaseResponse<SubscriptionPack>(pack, "Subscription pack retrieved successfully"));
    }

    #endregion

    #region Subscription Payment Endpoints

    [HttpPost("purchase")]
    public async Task<ActionResult<SubscriptionPayment>> PurchaseSubscription([FromBody] PurchaseSubscriptionDTO dto)
    {
        SubscriptionPayment payment = await subscriptionPaymentService.PurchaseSubscriptionAsync(dto);
        return CreatedAtAction(nameof(GetPayment),
            new { id = payment.Id }, new BaseResponse<SubscriptionPayment>(payment, "Subscription payment retrieved successfully"));
    }


    [HttpGet("payments")]
    public async Task<ActionResult<IEnumerable<SubscriptionPayment>>> GetAllPayments()
    {
        IEnumerable<SubscriptionPayment> payments = await subscriptionPaymentService.GetAllAsync();
        return Ok(new BaseResponse<IEnumerable<SubscriptionPayment>>(payments, "Subscription payments retrieved successfully"));
    }

    [HttpGet("payments/{id}")]
    public async Task<ActionResult<SubscriptionPayment>> GetPayment(int id)
    {
        SubscriptionPayment payment = await subscriptionPaymentService.GetByIdValidatedAsync(id);
        return Ok(payment);
    }

    #endregion

    #region Subscription Feature Endpoints

    [HttpGet("features")]
    public async Task<ActionResult<IEnumerable<SubscriptionFeature>>> GetAllFeatures()
    {
        IEnumerable<SubscriptionFeature> features = await subscriptionFeatureService.GetAllAsync();
        return Ok(new BaseResponse<IEnumerable<SubscriptionFeature>>(features, "Subscription features retrieved successfully"));
    }

    [HttpGet("features/{id}")]
    public async Task<ActionResult<SubscriptionFeature>> GetFeature(int id)
    {
        SubscriptionFeature feature = await subscriptionFeatureService.GetByIdValidatedAsync(id);
        return Ok(new BaseResponse<SubscriptionFeature>(feature, "Subscription feature retrieved successfully"));
    }

    #endregion
}
