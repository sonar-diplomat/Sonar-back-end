using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Report;
using Entities.Models;
using Entities.Models.Report;

namespace Application.Services.Report
{
    public class ReportReasonTypeService : IReportReasonTypeService
    {
        private readonly IReportReasonTypeRepository _repository;

        public ReportReasonTypeService(IReportReasonTypeRepository repository)
        {
            _repository = repository;
        }

        public Task<ReportReasonType> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<ReportReasonType>> GetAllAsync() => throw new NotImplementedException();
        public Task<ReportReasonType> CreateAsync(ReportReasonType entity) => throw new NotImplementedException();
        public Task<ReportReasonType> UpdateAsync(ReportReasonType entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

