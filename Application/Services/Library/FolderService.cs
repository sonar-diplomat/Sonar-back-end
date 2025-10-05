using Application.Abstractions.Interfaces.Repository;
using Application.Abstractions.Interfaces.Repository.Library;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Library;

namespace Application.Services.Library;

public class FolderService(IFolderRepository repository, IGenericRepository<Folder> genericRepository)
    : GenericService<Folder>(genericRepository), IFolderService
{
}