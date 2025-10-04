using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Music;

namespace Application.Services.Music;

public class PlaylistService(IPlaylistRepository repository) : GenericService<Playlist>(repository), IPlaylistService
{

}


