using Entities.Models;

namespace Application.Abstractions.Interfaces.Repository;

public interface IGenericRepository<T> where T : BaseModel
{
    Task<T?> GetByIdAsync(int? id);
    Task<IQueryable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<Task> UpdateAsync(T entity);
    Task<Task> RemoveAsync(T entity);
    Task<Task> RemoveRangeAsync(List<T> entities);
    Task SaveChangesAsync();
}