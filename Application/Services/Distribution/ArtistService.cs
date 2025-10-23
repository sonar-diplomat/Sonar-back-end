using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Music;

namespace Application.Services.Distribution;

public class ArtistService(IArtistRepository repository)
    : GenericService<Artist>(repository), IArtistService
{
}