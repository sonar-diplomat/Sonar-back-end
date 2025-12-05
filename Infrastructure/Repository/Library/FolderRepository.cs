using Application.Abstractions.Interfaces.Repository.Library;
using Entities.Models.Library;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.Library;

public class FolderRepository(SonarContext dbContext) : GenericRepository<Folder>(dbContext), IFolderRepository
{
    public async Task<int?> GetRootFolderIdByLibraryIdAsync(int libraryId)
    {
        return await context.Set<Entities.Models.Library.Library>()
            .Where(l => l.Id == libraryId)
            .Select(l => l.RootFolderId)
            .FirstOrDefaultAsync();
    }
}
