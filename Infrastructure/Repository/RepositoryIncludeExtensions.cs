using Application.Abstractions.Interfaces.Repository;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
public static class RepositoryIncludeExtensions
{
    public static IIncludableQueryable<T, TProperty> Include<T, TProperty>(
        this IGenericRepository<T> repository,
        Expression<Func<T, TProperty>> navigationPropertyPath)
        where T : BaseModel
    {
        return repository.Query().Include(navigationPropertyPath);
    }

    public static IIncludableQueryable<T, TProperty> Include_<T, TProperty>(
        this IQueryable<T> source,
        Expression<Func<T, TProperty>> navigationPropertyPath)
        where T : BaseModel
    {
        return source.Include(navigationPropertyPath);
    }

    public static IIncludableQueryable<T, TNextProperty> ThenInclude_<T, TPreviousProperty, TNextProperty>(
        this IIncludableQueryable<T, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
        where T : BaseModel
    {
        return source.ThenInclude(navigationPropertyPath);
    }

    public static async Task<T?> GetByIdAsync<T>(
        this IQueryable<T> query,
        int id)
        where T : BaseModel
    {
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }
}