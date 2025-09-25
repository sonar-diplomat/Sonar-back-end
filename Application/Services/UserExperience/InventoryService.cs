using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserExperience;
using Entities.Models;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repository;

        public InventoryService(IInventoryRepository repository)
        {
            _repository = repository;
        }

        public Task<Inventory> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Inventory>> GetAllAsync() => throw new NotImplementedException();
        public Task<Inventory> CreateAsync(Inventory entity) => throw new NotImplementedException();
        public Task<Inventory> UpdateAsync(Inventory entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

