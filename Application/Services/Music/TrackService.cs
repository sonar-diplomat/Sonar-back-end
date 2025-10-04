using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;

namespace Application.Services.Music
{
    public class TrackService(ITrackRepository repository) : ITrackService
    {

    }
}
