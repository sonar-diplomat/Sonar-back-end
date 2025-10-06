using Application.Abstractions.Interfaces.Repository;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository;

public class GenericRepository<T>(SonarContext context) : IGenericRepository<T>
    where T : BaseModel
{
    protected readonly SonarContext context = context;

    public virtual async Task<T?> GetByIdAsync(int? id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public virtual async Task<IQueryable<T>> GetAllAsync()
    {
        return await Task.FromResult(context.Set<T>().AsQueryable());
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await context.Set<T>().AddAsync(entity);
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        return context.Set<T>().Update(entity).Entity;
    }

    public virtual async Task RemoveAsync(T entity)
    {
        context.Set<T>().Remove(entity);
    }

    public virtual async Task RemoveRangeAsync(List<T> entities)
    {
        context.Set<T>().RemoveRange(entities);
    }

    public virtual async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}