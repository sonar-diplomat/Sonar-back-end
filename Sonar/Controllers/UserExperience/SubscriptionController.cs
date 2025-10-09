using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Models.UserExperience;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.UserExperience;

[Route("api/[controller]")]
[ApiController]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionFeatureService subscriptionFeatureService;
    private readonly ISubscriptionPackService subscriptionPackService;
    private readonly ISubscriptionPaymentService subscriptionPaymentService;

    public SubscriptionController(ISubscriptionPackService subscriptionPackService,
        ISubscriptionPaymentService subscriptionPaymentService,
        ISubscriptionFeatureService subscriptionFeatureService)
    {
        this.subscriptionPackService = subscriptionPackService;
        this.subscriptionPaymentService = subscriptionPaymentService;
        this.subscriptionFeatureService = subscriptionFeatureService;
    }

    #region Subscription Pack Endpoints

    /// <summary>
    ///     Get all subscription packs
    /// </summary>
    /// <returns>List of all subscription packs</returns>
    [HttpGet("packs")]
    public async Task<ActionResult<IEnumerable<SubscriptionPack>>> GetAllPacks()
    {
        try
        {
            IEnumerable<SubscriptionPack> packs = await subscriptionPackService.GetAllAsync();
            return Ok(packs);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Get a specific subscription pack by ID
    /// </summary>
    /// <param name="id">Subscription pack ID</param>
    /// <returns>Subscription pack details</returns>
    [HttpGet("packs/{id}")]
    public async Task<ActionResult<SubscriptionPack>> GetPack(int id)
    {
        try
        {
            SubscriptionPack pack = await subscriptionPackService.GetByIdAsync(id);

            if (pack == null) throw new NotImplementedException();

            return Ok(pack);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Create a new subscription pack
    /// </summary>
    /// <param name="pack">Subscription pack details</param>
    /// <returns>Created subscription pack</returns>
    [HttpPost("packs")]
    public async Task<ActionResult<SubscriptionPack>> CreatePack([FromBody] SubscriptionPack pack)
    {
        try
        {
            SubscriptionPack createdPack = await subscriptionPackService.CreateAsync(pack);
            return CreatedAtAction(nameof(GetPack), new { id = createdPack.Id }, createdPack);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Update an existing subscription pack
    /// </summary>
    /// <param name="id">Subscription pack ID</param>
    /// <param name="pack">Updated subscription pack details</param>
    /// <returns>Updated subscription pack</returns>
    [HttpPut("packs/{id}")]
    public async Task<ActionResult<SubscriptionPack>> UpdatePack(int id, [FromBody] SubscriptionPack pack)
    {
        try
        {
            if (id != pack.Id) throw new NotImplementedException();

            SubscriptionPack updatedPack = await subscriptionPackService.UpdateAsync(pack);
            return Ok(updatedPack);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Delete a subscription pack
    /// </summary>
    /// <param name="id">Subscription pack ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("packs/{id}")]
    public async Task<IActionResult> DeletePack(int id)
    {
        try
        {
            await subscriptionPackService.DeleteAsync(id);
            return NoContent();
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Subscription Payment Endpoints

    /// <summary>
    ///     Purchase a subscription for yourself
    /// </summary>
    /// <param name="dto">Purchase details</param>
    /// <returns>Created subscription payment</returns>
    [HttpPost("purchase")]
    public async Task<ActionResult<SubscriptionPayment>> PurchaseSubscription([FromBody] PurchaseSubscriptionDTO dto)
    {
        try
        {
            SubscriptionPayment payment = await subscriptionPaymentService.PurchaseSubscriptionAsync(dto);
            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Get all subscription payments
    /// </summary>
    /// <returns>List of all subscription payments</returns>
    [HttpGet("payments")]
    public async Task<ActionResult<IEnumerable<SubscriptionPayment>>> GetAllPayments()
    {
        try
        {
            IEnumerable<SubscriptionPayment> payments = await subscriptionPaymentService.GetAllAsync();
            return Ok(payments);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Get a specific subscription payment by ID
    /// </summary>
    /// <param name="id">Payment ID</param>
    /// <returns>Payment details</returns>
    [HttpGet("payments/{id}")]
    public async Task<ActionResult<SubscriptionPayment>> GetPayment(int id)
    {
        try
        {
            SubscriptionPayment payment = await subscriptionPaymentService.GetByIdAsync(id);

            if (payment == null) throw new NotImplementedException();

            return Ok(payment);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Create a new subscription payment (purchase subscription)
    /// </summary>
    /// <param name="payment">Payment details</param>
    /// <returns>Created payment</returns>
    [HttpPost("payments")]
    public async Task<ActionResult<SubscriptionPayment>> CreatePayment([FromBody] SubscriptionPayment payment)
    {
        try
        {
            SubscriptionPayment createdPayment = await subscriptionPaymentService.CreateAsync(payment);
            return CreatedAtAction(nameof(GetPayment), new { id = createdPayment.Id }, createdPayment);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Delete/cancel a subscription payment
    /// </summary>
    /// <param name="id">Payment ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("payments/{id}")]
    public async Task<IActionResult> DeletePayment(int id)
    {
        try
        {
            await subscriptionPaymentService.DeleteAsync(id);
            return NoContent();
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Subscription Feature Endpoints

    /// <summary>
    ///     Get all subscription features
    /// </summary>
    /// <returns>List of all subscription features</returns>
    [HttpGet("features")]
    public async Task<ActionResult<IEnumerable<SubscriptionFeature>>> GetAllFeatures()
    {
        try
        {
            IEnumerable<SubscriptionFeature> features = await subscriptionFeatureService.GetAllAsync();
            return Ok(features);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Get a specific subscription feature by ID
    /// </summary>
    /// <param name="id">Feature ID</param>
    /// <returns>Feature details</returns>
    [HttpGet("features/{id}")]
    public async Task<ActionResult<SubscriptionFeature>> GetFeature(int id)
    {
        try
        {
            SubscriptionFeature feature = await subscriptionFeatureService.GetByIdAsync(id);

            if (feature == null) throw new NotImplementedException();

            return Ok(feature);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Create a new subscription feature
    /// </summary>
    /// <param name="feature">Feature details</param>
    /// <returns>Created feature</returns>
    [HttpPost("features")]
    public async Task<ActionResult<SubscriptionFeature>> CreateFeature([FromBody] SubscriptionFeature feature)
    {
        try
        {
            SubscriptionFeature createdFeature = await subscriptionFeatureService.CreateAsync(feature);
            return CreatedAtAction(nameof(GetFeature), new { id = createdFeature.Id }, createdFeature);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Update an existing subscription feature
    /// </summary>
    /// <param name="id">Feature ID</param>
    /// <param name="feature">Updated feature details</param>
    /// <returns>Updated feature</returns>
    [HttpPut("features/{id}")]
    public async Task<ActionResult<SubscriptionFeature>> UpdateFeature(int id, [FromBody] SubscriptionFeature feature)
    {
        try
        {
            if (id != feature.Id) throw new NotImplementedException();

            SubscriptionFeature updatedFeature = await subscriptionFeatureService.UpdateAsync(feature);
            return Ok(updatedFeature);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Delete a subscription feature
    /// </summary>
    /// <param name="id">Feature ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("features/{id}")]
    public async Task<IActionResult> DeleteFeature(int id)
    {
        try
        {
            await subscriptionFeatureService.DeleteAsync(id);
            return NoContent();
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}