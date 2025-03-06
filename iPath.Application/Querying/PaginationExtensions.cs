using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Querying;

public static class PaginationExtensions
{
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PaginatedListQuery request)
    {
        if (request.PageSize.HasValue)
            query = query.Skip(request.Page * request.PageSize.Value).Take(request.PageSize.Value);

        return query;
    }


    public static PaginatedListResult<T> GetPaginatedListResult<T>(this IQueryable<T> query, PaginatedListQuery request) 
    {
        // get total count
        var count = query.Count();

        // aplly sort
        query = query.ApplySort(request.SortDefinitions);

        // apply pagination
        if( request.PageSize.HasValue)
            query = query.Skip(request.Page * request.PageSize.Value).Take(request.PageSize.Value);

        // pack the result
        return new PaginatedListResult<T>(Data: query.ToList(), TotalItemsCount: count);
    }

    public static async Task<PaginatedListResult<T>> GetPaginatedListResultAsync<T>(this IQueryable<T> query, PaginatedListQuery request)
    {
        // get total count
        var count = await query.CountAsync();

        // aplly sort
        query = query.ApplySort(request.SortDefinitions);

        // apply pagination
        if (request.PageSize.HasValue && request.PageSize.Value > 0)
            query = query.Skip(request.Page).Take(request.PageSize.Value);

        // pack the result
        return new PaginatedListResult<T>(Data: await query.ToListAsync(), TotalItemsCount: count);
    }
}
