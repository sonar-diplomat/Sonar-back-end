using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Music;

namespace Application.Services.Music;

public class BlendService(IBlendRepository repository, ILibraryService libraryService, IFolderService folderService)
    : CollectionService<Blend>(repository, libraryService, folderService), IBlendService
{
}