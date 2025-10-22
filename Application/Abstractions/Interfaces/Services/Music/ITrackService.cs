using Application.DTOs;
using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services;

public interface ITrackService : IGenericService<Track>
{
    Task<MusicStreamResultDTO?> GetMusicStreamAsync(int songId, string? rangeHeader);
    Task<MusicStreamResultDTO?> GetDemoMusicStreamAsync(int songId);
}
