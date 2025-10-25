using Application.Abstractions.Interfaces.Repository;
using Entities.Models;
using Infrastructure.Data;
using System.Linq.Expressions;

namespace Sonar.Infrastructure.Repository;

public class GenericRepository<T>(SonarContext context) : IGenericRepository<T>
    where T : BaseModel
{
    protected readonly SonarContext context = context;

    public IQueryable<T> Query()
    {
        return context.Set<T>().AsQueryable();
    }
    public virtual async Task<IQueryable<T>> Include(Expression<Func<T, object>> prop)
    {
        throw new NotImplementedException();
    }

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
        await context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task RemoveAsync(T entity)
    {
        context.Set<T>().Remove(entity);

        await context.SaveChangesAsync();
    }

    public virtual async Task RemoveRangeAsync(List<T> entities)
    {
        context.Set<T>().RemoveRange(entities);

        await context.SaveChangesAsync();
    }

    public virtual async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
