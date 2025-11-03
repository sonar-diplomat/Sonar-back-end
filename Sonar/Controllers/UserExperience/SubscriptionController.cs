using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.User;
using Application.DTOs.UserExperience;
using Application.Response;
using Entities.Enums;
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
    public async Task<ActionResult<IEnumerable<SubscriptionPackDTO>>> GetAllPacks()
    {
        IEnumerable<SubscriptionPack> packs = (await subscriptionPackService.GetAllAsync()).ToList();
        IEnumerable<SubscriptionPackDTO> dtos = packs.Select(p => new SubscriptionPackDTO
        {
            Id = p.Id,
            Name = p.Name,
            DiscountMultiplier = p.DiscountMultiplier,
            Description = p.Description,
            Price = p.Price,
            Features = p.SubscriptionFeatures?.Select(f => new SubscriptionFeatureDTO
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price
            }).ToList() ?? new List<SubscriptionFeatureDTO>()
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<SubscriptionPackDTO>>>(dtos,
            ["Subscription packs retrieved successfully"]);
    }

    [HttpGet("packs/{id}")]
    public async Task<ActionResult<SubscriptionPackDTO>> GetPack(int id)
    {
        SubscriptionPack pack = await subscriptionPackService.GetByIdValidatedAsync(id);
        SubscriptionPackDTO dto = new()
        {
            Id = pack.Id,
            Name = pack.Name,
            DiscountMultiplier = pack.DiscountMultiplier,
            Description = pack.Description,
            Price = pack.Price,
            Features = pack.SubscriptionFeatures?.Select(f => new SubscriptionFeatureDTO
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price
            }).ToList() ?? new List<SubscriptionFeatureDTO>()
        };
        throw ResponseFactory.Create<OkResponse<SubscriptionPackDTO>>(dto, ["Subscription pack retrieved successfully"]);
    }

    #endregion

    #region Subscription Payment Endpoints

    [HttpPost("purchase")]
    public async Task<ActionResult<SubscriptionPaymentDTO>> PurchaseSubscription([FromBody] PurchaseSubscriptionDTO dto)
    {
        User user = await CheckAccessFeatures([]);
        SubscriptionPayment payment = await subscriptionPaymentService.PurchaseSubscriptionAsync(user.Id, dto);
        SubscriptionPaymentDTO responseDto = new()
        {
            Id = payment.Id,
            Amount = payment.Amount,
            CreatedAt = payment.CreatedAt,
            Buyer = new UserResponseDTO
            {
                Id = payment.Buyer.Id,
                UserName = payment.Buyer.UserName ?? string.Empty,
                PublicIdentifier = payment.Buyer.PublicIdentifier,
                Biography = payment.Buyer.Biography,
                RegistrationDate = payment.Buyer.RegistrationDate,
                AvatarUrl = payment.Buyer.AvatarImage?.Url ?? string.Empty
            },
            SubscriptionPackId = payment.SubscriptionPackId,
            SubscriptionPackName = payment.SubscriptionPack?.Name ?? string.Empty
        };
        throw ResponseFactory.Create<CreatedResponse<SubscriptionPaymentDTO>>(responseDto,
            ["Subscription purchased successfully"]);
    }


    [HttpGet("payments")]
    public async Task<ActionResult<IEnumerable<SubscriptionPaymentDTO>>> GetAllPayments()
    {
        await CheckAccessFeatures([AccessFeatureStruct.IamAGod]);
        IEnumerable<SubscriptionPayment> payments = (await subscriptionPaymentService.GetAllAsync()).ToList();
        IEnumerable<SubscriptionPaymentDTO> dtos = payments.Select(payment => new SubscriptionPaymentDTO
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
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<SubscriptionPaymentDTO>>>(dtos,
            ["Subscription payments retrieved successfully"]);
    }

    [HttpGet("payments/{id}")]
    public async Task<ActionResult<SubscriptionPaymentDTO>> GetPayment(int id)
    {
        SubscriptionPayment payment = await subscriptionPaymentService.GetByIdValidatedAsync(id);
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
            ["Subscription payment retrieved successfully"]);
    }

    #endregion

    #region Subscription Feature Endpoints

    [HttpGet("features")]
    public async Task<ActionResult<IEnumerable<SubscriptionFeatureDTO>>> GetAllFeatures()
    {
        IEnumerable<SubscriptionFeature> features = (await subscriptionFeatureService.GetAllAsync()).ToList();
        IEnumerable<SubscriptionFeatureDTO> dtos = features.Select(f => new SubscriptionFeatureDTO
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            Price = f.Price
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<SubscriptionFeatureDTO>>>(dtos,
            ["Subscription features retrieved successfully"]);
    }

    [HttpGet("features/{id}")]
    public async Task<ActionResult<SubscriptionFeatureDTO>> GetFeature(int id)
    {
        SubscriptionFeature feature = await subscriptionFeatureService.GetByIdValidatedAsync(id);
        SubscriptionFeatureDTO dto = new()
        {
            Id = feature.Id,
            Name = feature.Name,
            Description = feature.Description,
            Price = feature.Price
        };
        throw ResponseFactory.Create<OkResponse<SubscriptionFeatureDTO>>(dto,
            ["Subscription feature retrieved successfully"]);
    }

    #endregion
}