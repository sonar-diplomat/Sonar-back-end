using Application.Interfaces;
using Infrastructure;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository;

public class GenericRepository : IGenericRepository
{
    private readonly SonarContext _context;

    public GenericRepository(SonarContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync<T>(int? id) where T : BaseModel
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public Task<IQueryable<T>> GetAllAsync<T>() where T : BaseModel
    {
        return Task.FromResult(_context.Set<T>().AsQueryable());
    }

    public async Task<T> AddAsync<T>(T entity) where T : BaseModel
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync<T>(T entity) where T : BaseModel
    {
        _context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task RemoveAsync<T>(T entity) where T : BaseModel
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync<T>(List<T> entities) where T : BaseModel
    {
        _context.Set<T>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}