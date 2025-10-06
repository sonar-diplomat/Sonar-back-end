using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience;

public class SubscriptionPaymentService(ISubscriptionPaymentRepository repository)
    : GenericService<SubscriptionPayment>(repository), ISubscriptionPaymentService
{
}