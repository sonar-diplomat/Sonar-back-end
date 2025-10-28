using Application.Abstractions.Interfaces.Repository;
using Application.Abstractions.Interfaces.Services;
using Application.Response;
using Entities.Models;

namespace Application.Services;

public abstract class GenericService<T>(IGenericRepository<T> repository) : IGenericService<T> where T : BaseModel
{
    public async Task<T> CreateAsync(T entity)
    {
        return await repository.AddAsync(entity);
    }

    public virtual async Task DeleteAsync(int id)
    {
        T entity = await GetByIdValidatedAsync(id);
        await repository.RemoveAsync(entity);
    }

    public async Task DeleteAsync(T entity)
    {
        await repository.RemoveAsync(entity);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await repository.GetAllAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await repository.GetByIdAsync(id);
    }

    public virtual async Task<T> GetByIdValidatedAsync(int id)
    {
        T? entity = await repository.GetByIdAsync(id);
        return entity ?? throw ResponseFactory.Create<NotFoundResponse>([$"{typeof(T).Name} not found"]);
    }

    public async Task<T> UpdateAsync(T entity)
    {
        return await repository.UpdateAsync(entity);
    }
}