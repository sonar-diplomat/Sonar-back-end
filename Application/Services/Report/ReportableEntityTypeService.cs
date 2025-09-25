using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Report;
using Entities.Models;
using Entities.Models.Report;

namespace Application.Services.Report
{
    public class ReportableEntityTypeService : IReportableEntityTypeService
    {
        private readonly IReportableEntityTypeRepository _repository;

        public ReportableEntityTypeService(IReportableEntityTypeRepository repository)
        {
            _repository = repository;
        }

        public Task<ReportableEntityType> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<ReportableEntityType>> GetAllAsync() => throw new NotImplementedException();
        public Task<ReportableEntityType> CreateAsync(ReportableEntityType entity) => throw new NotImplementedException();
        public Task<ReportableEntityType> UpdateAsync(ReportableEntityType entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

