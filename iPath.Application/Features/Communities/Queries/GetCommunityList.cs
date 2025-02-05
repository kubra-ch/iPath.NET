using iPath.Application.Querying;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class GetCommunityListQuery : PaginatedListQuery, IRequest<PaginatedListResult<Community>>
{
}


public class GetCommunityListQueryHandler(IPathDbContext ctx)
    : IRequestHandler<GetCommunityListQuery, PaginatedListResult<Community>>
{
    public async Task<PaginatedListResult<Community>> Handle(GetCommunityListQuery request, CancellationToken cancellationToken)
    {
        var q = ctx.Communities.AsNoTracking().AsQueryable();
        if( request.Filter != null)
        {
           foreach (var f in request.Filter.Filters)
            {
                if (!string.IsNullOrEmpty(f.Field) && f.Value != null)
                {
                    switch (f.Field.ToLowerInvariant())
                    {
                        case "name":
                            q = q.Where(u => u.Name.Contains(f.Value.ToString()));
                            break;
                    }
                }
            }
        }

        return await q.GetPaginatedListResultAsync(request);
    }
}

