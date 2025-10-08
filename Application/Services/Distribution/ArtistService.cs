using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Distribution;

namespace Application.Services.Distribution;

public class ArtistService(IArtistRepository repository) : GenericService<Artist>(repository), IArtistService
{
}
