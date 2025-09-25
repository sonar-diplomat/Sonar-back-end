using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Report;
using Entities.Models;

namespace Application.Services.Report
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;

        public ReportService(IReportRepository repository)
        {
            _repository = repository;
        }

        public Task<Entities.Models.Report.Report> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Entities.Models.Report.Report>> GetAllAsync() => throw new NotImplementedException();
        public Task<Entities.Models.Report.Report> CreateAsync(Entities.Models.Report.Report entity) => throw new NotImplementedException();
        public Task<Entities.Models.Report.Report> UpdateAsync(Entities.Models.Report.Report entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

