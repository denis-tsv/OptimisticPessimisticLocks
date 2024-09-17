using Microsoft.EntityFrameworkCore;

namespace ExclusiveLockApi.Services;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> ForUpdate<TEntity>(this IQueryable<TEntity> source) 
        => source.TagWith(ForUpdateInterceptor.HintForUpdate);
}