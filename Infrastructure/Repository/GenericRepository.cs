using Application.Abstractions.Interfaces.Repository;
using Infrastructure;
using Infrastructure.Data;


namespace Sonar.Infrastructure.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
{
    private readonly SonarContext _context;

    public GenericRepository(SonarContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(int? id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public Task<IQueryable<T>> GetAllAsync()
    {
        return Task.FromResult(_context.Set<T>().AsQueryable());
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(List<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}