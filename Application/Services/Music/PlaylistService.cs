using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;

namespace Application.Services.Music
{
    public class PlaylistService(IPlaylistRepository repository) : IPlaylistService
    {

    }
}

