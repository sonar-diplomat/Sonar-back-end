using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models.Distribution;
using Infrastructure.Data;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Distribution;

public class ArtistRegistrationRequestRepository(SonarContext dbContext)
    : GenericRepository<ArtistRegistrationRequest>(dbContext), IArtistRegistrationRequestRepository;