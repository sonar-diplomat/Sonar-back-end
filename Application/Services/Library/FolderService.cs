using Application.Abstractions.Interfaces.Repository.Library;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Library;

namespace Application.Services.Library;

public class FolderService(IFolderRepository repository) : GenericService<Folder>(repository), IFolderService
{
}