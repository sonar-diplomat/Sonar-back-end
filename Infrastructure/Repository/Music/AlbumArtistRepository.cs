using Application.Abstractions.Interfaces.Repository.Music;
using Entities.Models.Music;
using Infrastructure.Data;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Music;

public class AlbumArtistRepository(SonarContext dbContext)
    : GenericRepository<AlbumArtist>(dbContext), IAlbumArtistRepository
{
}