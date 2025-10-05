using Entities.Models;

namespace Application.Abstractions.Interfaces.Service;

public interface IGenericService<T> where T : BaseModel
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
}