using Infrastructure;

namespace Application.Abstractions.Interfaces.Repository;

public interface IGenericRepository<T> where T : BaseModel
{
    Task<T?> GetByIdAsync(int? id);
    Task<IQueryable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
    Task RemoveRangeAsync(List<T> entities);
    Task SaveChangesAsync();
}