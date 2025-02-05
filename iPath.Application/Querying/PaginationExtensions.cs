using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPath.Application.Querying;
public static class PaginationExtensions
{
    public static PaginatedListResult<T> GetPaginatedListResult<T>(this IQueryable<T> query, PaginatedListQuery request) 
    {
        // get total count
        var count = query.Count();

        // aplly sort
        query = query.ApplySort(request.SortDefinitions);

        // apply pagination
        if( request.Count.HasValue)
            query = query.Skip(request.StartIndex * request.Count.Value).Take(request.Count.Value);

        // pack the result
        return new PaginatedListResult<T>(query.ToList(), count);
    }

    public static async Task<PaginatedListResult<T>> GetPaginatedListResultAsync<T>(this IQueryable<T> query, PaginatedListQuery request)
    {
        // get total count
        var count = await query.CountAsync();

        // aplly sort
        query = query.ApplySort(request.SortDefinitions);

        // apply pagination
        if (request.Count.HasValue && request.Count.Value > 0)
            query = query.Skip(request.StartIndex * request.Count.Value).Take(request.Count.Value);

        // pack the result
        return new PaginatedListResult<T>(await query.ToListAsync(), count);
    }
}
