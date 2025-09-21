using Infrastructure;

namespace Application.Interfaces;

public interface IGenericRepository
{
    Task<T?> GetByIdAsync<T>(int? id) where T : BaseModel;
    Task<IQueryable<T>> GetAllAsync<T>() where T : BaseModel;
    Task<T> AddAsync<T>(T entity) where T : BaseModel;
    Task UpdateAsync<T>(T entity) where T : BaseModel;
    Task RemoveAsync<T>(T entity) where T : BaseModel;
    Task RemoveRangeAsync<T>(List<T> entities) where T : BaseModel;
    Task SaveChangesAsync();
}