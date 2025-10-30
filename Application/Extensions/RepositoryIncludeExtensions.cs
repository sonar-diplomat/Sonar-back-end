using System.Linq.Expressions;
using Application.Abstractions.Interfaces.Repository;
using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Response;
using Entities.Models;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Extensions;

public static class RepositoryIncludeExtensions
{
    // Include от репозитория
    public static IIncludableQueryable<T, TProperty> Include<T, TProperty>(
        this IGenericRepository<T> repository,
        Expression<Func<T, TProperty>> navigationPropertyPath)
        where T : BaseModel
    {
        return EntityFrameworkQueryableExtensions.Include(repository.Query(), navigationPropertyPath);
    }

    // Include от репозитория
    public static IIncludableQueryable<User, TProperty> Include<TProperty>(
        this IUserRepository repository,
        Expression<Func<User, TProperty>> navigationPropertyPath)
    {
        return EntityFrameworkQueryableExtensions.Include(repository.Query(), navigationPropertyPath);
    }

    // Include от IQueryable (для цепочки вызовов)
    public static IIncludableQueryable<T, TProperty> Include<T, TProperty>(
        this IQueryable<T> source,
        Expression<Func<T, TProperty>> navigationPropertyPath)
        where T : BaseModel
    {
        return EntityFrameworkQueryableExtensions.Include(source, navigationPropertyPath);
    }

    public static IIncludableQueryable<T, TNextProperty> ThenInclude<T, TPreviousProperty, TNextProperty>(
        this IIncludableQueryable<T, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
        where T : BaseModel
    {
        return EntityFrameworkQueryableExtensions.ThenInclude(source, navigationPropertyPath);
    }

    public static async Task<T?> GetByIdAsync<T>(this IQueryable<T> query, int id) where T : BaseModel
    {
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }
    
    public static async Task<T> GetByIdValidatedAsync<T>(this IQueryable<T> query, int id) where T : BaseModel
    {
        T? entity = await query.FirstOrDefaultAsync(e => e.Id == id);
        return entity ?? throw ResponseFactory.Create<NotFoundResponse>([$"{typeof(T).Name} not found"]);
    }
    
    public static async Task<User?> GetByIdAsync(this IQueryable<User> query, int id)
    {
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }
    
    public static async Task<User> GetByIdValidatedAsync(this IQueryable<User> query, int id)
    {
        User? user = await query.FirstOrDefaultAsync(e => e.Id == id);
        return user ?? throw ResponseFactory.Create<NotFoundResponse>([$"{nameof(User)} not found"]);
    }
}