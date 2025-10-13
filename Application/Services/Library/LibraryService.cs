using Application.Abstractions.Interfaces.Repository.Library;
using Application.Abstractions.Interfaces.Services;
using LibraryModel = Entities.Models.Library.Library;

namespace Application.Services.Library;

public class LibraryService(ILibraryRepository repository) : GenericService<LibraryModel>(repository), ILibraryService
{
}
