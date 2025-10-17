using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models.Distribution;
using Infrastructure.Data;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Distribution;

public class DistributorAccountRepository(SonarContext dbContext)
    : GenericRepository<DistributorAccount>(dbContext), IDistributorAccountRepository;