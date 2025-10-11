using Entities.Models;

namespace Application.Abstractions.Interfaces.Services;

public interface IGenericService<T> where T : BaseModel
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task DeleteAsync(T entity);
}
