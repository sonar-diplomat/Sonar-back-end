using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class SubscriptionPaymentService : ISubscriptionPaymentService
    {
        private readonly ISubscriptionPaymentRepository _repository;

        public SubscriptionPaymentService(ISubscriptionPaymentRepository repository)
        {
            _repository = repository;
        }

        public Task<SubscriptionPayment> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<SubscriptionPayment>> GetAllAsync() => throw new NotImplementedException();
        public Task<SubscriptionPayment> CreateAsync(SubscriptionPayment entity) => throw new NotImplementedException();
        public Task<SubscriptionPayment> UpdateAsync(SubscriptionPayment entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

