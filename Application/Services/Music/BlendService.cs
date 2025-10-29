using Application.Abstractions.Interfaces.Repository;
using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Music;

namespace Application.Services.Music;

public class BlendService(
    IBlendRepository repository, 
    IGenericRepository<Blend> genericRepository) : CollectionService<Blend>(genericRepository), IBlendService
{
}
