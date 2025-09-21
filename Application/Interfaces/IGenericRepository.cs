namespace Application.Interfaces;

public interface IGenericRepository
{
    Task<T?> GetByIdAsync<T>(int? id) where T : class;
    Task<IQueryable<T>> GetAllAsync<T>() where T : class;
    Task<T> AddAsync<T>(T entity) where T : class;
    Task UpdateAsync<T>(T entity) where T : class;
    Task RemoveAsync<T>(T entity) where T : class;
    Task RemoveRangeAsync<T>(List<T> entities) where T : class;
    Task SaveChangesAsync();
}