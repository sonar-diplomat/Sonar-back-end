using Application.Abstractions.Interfaces.Repository.Music;
using Entities.Models.Music;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Music;

public class ArtistRepository(SonarContext dbContext) : GenericRepository<Artist>(dbContext), IArtistRepository
{
    private readonly SonarContext dbContext = dbContext;

    public Task<Artist?> GetByNameAsync(string name)
    {
        return dbContext.Artists.FirstOrDefaultAsync(a => a.ArtistName == name);
    }

    public async Task<IEnumerable<Artist>> SearchByNameAsync(string searchTerm)
    {
        return await dbContext.Artists
            .Where(a => EF.Functions.Like(a.ArtistName, $"%{searchTerm}%"))
            .ToListAsync();
    }
}