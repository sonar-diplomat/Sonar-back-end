using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Music;

namespace Application.Services.Music;

public class TrackService(ITrackRepository repository) : GenericService<Track>(repository), ITrackService
{

}

