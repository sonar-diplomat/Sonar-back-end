using Application.Abstractions.Interfaces.Repository.Library;
using Application.Abstractions.Interfaces.Services;

namespace Application.Services.Library;

public class LibraryService(ILibraryRepository repository) : ILibraryService
{
    public Task<Entities.Models.Library.Library> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Entities.Models.Library.Library>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Entities.Models.Library.Library> CreateAsync(Entities.Models.Library.Library entity)
    {
        throw new NotImplementedException();
    }

    public Task<Entities.Models.Library.Library> UpdateAsync(Entities.Models.Library.Library entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}