using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Response;
using Entities.Models.UserCore;
using Entities.Models.UserExperience;
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
        throw ResponseFactory.Create<OkResponse<IEnumerable<SubscriptionPack>>>(packs,
            ["Subscription packs retrieved successfully"]);
    }

    [HttpGet("packs/{id}")]
    public async Task<ActionResult<SubscriptionPack>> GetPack(int id)
    {
        SubscriptionPack pack = await subscriptionPackService.GetByIdValidatedAsync(id);
        throw ResponseFactory.Create<OkResponse<SubscriptionPack>>(pack, ["Subscription pack retrieved successfully"]);
    }

    #endregion

    #region Subscription Payment Endpoints

    [HttpPost("purchase")]
    public async Task<ActionResult<SubscriptionPayment>> PurchaseSubscription([FromBody] PurchaseSubscriptionDTO dto)
    {
        await CheckAccessFeatures([]);
        SubscriptionPayment payment = await subscriptionPaymentService.PurchaseSubscriptionAsync(dto);
        throw ResponseFactory.Create<CreatedResponse<SubscriptionPayment>>(payment,
            ["Subscription purchased successfully"]);
    }


    [HttpGet("payments")]
    public async Task<ActionResult<IEnumerable<SubscriptionPayment>>> GetAllPayments()
    {
        IEnumerable<SubscriptionPayment> payments = await subscriptionPaymentService.GetAllAsync();
        throw ResponseFactory.Create<OkResponse<IEnumerable<SubscriptionPayment>>>(payments,
            ["Subscription payments retrieved successfully"]);
    }

    [HttpGet("payments/{id}")]
    public async Task<ActionResult<SubscriptionPayment>> GetPayment(int id)
    {
        SubscriptionPayment payment = await subscriptionPaymentService.GetByIdValidatedAsync(id);
        throw ResponseFactory.Create<OkResponse<SubscriptionPayment>>(payment,
            ["Subscription payment retrieved successfully"]);
    }

    #endregion

    #region Subscription Feature Endpoints

    [HttpGet("features")]
    public async Task<ActionResult<IEnumerable<SubscriptionFeature>>> GetAllFeatures()
    {
        IEnumerable<SubscriptionFeature> features = await subscriptionFeatureService.GetAllAsync();
        throw ResponseFactory.Create<OkResponse<IEnumerable<SubscriptionFeature>>>(features,
            ["Subscription features retrieved successfully"]);
    }

    [HttpGet("features/{id}")]
    public async Task<ActionResult<SubscriptionFeature>> GetFeature(int id)
    {
        SubscriptionFeature feature = await subscriptionFeatureService.GetByIdValidatedAsync(id);
        throw ResponseFactory.Create<OkResponse<SubscriptionFeature>>(feature,
            ["Subscription feature retrieved successfully"]);
    }

    #endregion
}