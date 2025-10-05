using LibraryModel = Entities.Models.Library.Library;

namespace Application.Abstractions.Interfaces.Repository.Library;

public interface ILibraryRepository : IGenericRepository<LibraryModel>
{
}