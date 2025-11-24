using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.User;
using Application.DTOs.UserExperience;
using Application.Response;
using Entities.Enums;
using Entities.Models.UserCore;
using Entities.Models.UserExperience;
using Microsoft.AspNetCore.Authorization;
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

    /// <summary>
    /// Retrieves all available subscription packs with their features and pricing.
    /// </summary>
    /// <returns>List of subscription pack DTOs including associated features.</returns>
    /// <response code="200">Subscription packs retrieved successfully.</response>
    [HttpGet("packs")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<SubscriptionPackDTO>>), StatusCodes.Status200OK)]
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

    /// <summary>
    /// Retrieves a specific subscription pack by ID with its features.
    /// </summary>
    /// <param name="id">The ID of the subscription pack to retrieve.</param>
    /// <returns>Subscription pack DTO with features.</returns>
    /// <response code="200">Subscription pack retrieved successfully.</response>
    /// <response code="404">Subscription pack not found.</response>
    [HttpGet("packs/{id}")]
    [ProducesResponseType(typeof(OkResponse<SubscriptionPackDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Purchases a subscription pack for the authenticated user.
    /// </summary>
    /// <param name="dto">Purchase subscription DTO containing pack ID and payment details.</param>
    /// <returns>Subscription payment DTO with purchase details.</returns>
    /// <response code="201">Subscription purchased successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="400">Invalid subscription pack or insufficient funds.</response>
    [HttpPost("purchase")]
    [Authorize]
    [ProducesResponseType(typeof(CreatedResponse<SubscriptionPaymentDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
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
        throw ResponseFactory.Create<CreatedResponse<SubscriptionPaymentDTO>>(responseDto,
            ["Subscription purchased successfully"]);
    }


    /// <summary>
    /// Retrieves all subscription payments in the system (admin only).
    /// </summary>
    /// <returns>List of subscription payment DTOs.</returns>
    /// <response code="200">Subscription payments retrieved successfully.</response>
    /// <response code="401">User not authorized (requires 'IamAGod' access feature).</response>
    /// <remarks>
    /// This endpoint requires the 'IamAGod' access feature for administrative access.
    /// </remarks>
    [HttpGet("payments")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<SubscriptionPaymentDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
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
                AvatarImageId = payment.Buyer.AvatarImageId
            },
            SubscriptionPackId = payment.SubscriptionPackId,
            SubscriptionPackName = payment.SubscriptionPack?.Name ?? string.Empty
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<SubscriptionPaymentDTO>>>(dtos,
            ["Subscription payments retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves a specific subscription payment by ID.
    /// </summary>
    /// <param name="id">The ID of the subscription payment to retrieve.</param>
    /// <returns>Subscription payment DTO with purchase details.</returns>
    /// <response code="200">Subscription payment retrieved successfully.</response>
    /// <response code="404">Subscription payment not found.</response>
    [HttpGet("payments/{id}")]
    [ProducesResponseType(typeof(OkResponse<SubscriptionPaymentDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
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
                AvatarImageId = payment.Buyer.AvatarImageId
            },
            SubscriptionPackId = payment.SubscriptionPackId,
            SubscriptionPackName = payment.SubscriptionPack?.Name ?? string.Empty
        };
        throw ResponseFactory.Create<OkResponse<SubscriptionPaymentDTO>>(dto,
            ["Subscription payment retrieved successfully"]);
    }

    #endregion

    #region Subscription Feature Endpoints

    /// <summary>
    /// Retrieves all available subscription features.
    /// </summary>
    /// <returns>List of subscription feature DTOs with pricing and descriptions.</returns>
    /// <response code="200">Subscription features retrieved successfully.</response>
    [HttpGet("features")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<SubscriptionFeatureDTO>>), StatusCodes.Status200OK)]
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

    /// <summary>
    /// Retrieves a specific subscription feature by ID.
    /// </summary>
    /// <param name="id">The ID of the subscription feature to retrieve.</param>
    /// <returns>Subscription feature DTO with pricing and description.</returns>
    /// <response code="200">Subscription feature retrieved successfully.</response>
    /// <response code="404">Subscription feature not found.</response>
    [HttpGet("features/{id}")]
    [ProducesResponseType(typeof(OkResponse<SubscriptionFeatureDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
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