using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience;

public class SubscriptionPaymentService(ISubscriptionPaymentRepository repository) : ISubscriptionPaymentService
{
    public Task<SubscriptionPayment> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SubscriptionPayment>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<SubscriptionPayment> CreateAsync(SubscriptionPayment entity)
    {
        throw new NotImplementedException();
    }

    public Task<SubscriptionPayment> UpdateAsync(SubscriptionPayment entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}