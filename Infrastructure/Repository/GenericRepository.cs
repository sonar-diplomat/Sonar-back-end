using Application.Abstractions.Interfaces.Repository;
using Entities.Models;
using Infrastructure.Data;


namespace Sonar.Infrastructure.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
{
    protected readonly SonarContext context;

    public GenericRepository(SonarContext context)
    {
        this.context = context;
    }

    public async Task<T?> GetByIdAsync(int? id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<IQueryable<T>> GetAllAsync()
    {
        return await Task.FromResult(context.Set<T>().AsQueryable());
    }

    public async Task<T> AddAsync(T entity)
    {
        await context.Set<T>().AddAsync(entity);
        return entity;
    }

    public async Task<Task> UpdateAsync(T entity)
    {
        context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public async Task<Task> RemoveAsync(T entity)
    {
        context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<Task> RemoveRangeAsync(List<T> entities)
    {
        context.Set<T>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}