using iPath.Application.Querying;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class GetGroupListQuery : PaginatedListQuery, IRequest<PaginatedListResult<Group>>
{
    public int? CommunityId { get; set; }
}


public class GetGroupListQueryHandler(IPathDbContext ctx)
    : IRequestHandler<GetGroupListQuery, PaginatedListResult<Group>>
{
    public async Task<PaginatedListResult<Group>> Handle(GetGroupListQuery request, CancellationToken cancellationToken)
    {
        var q = ctx.Groups.AsNoTracking()
            .Include(g => g.Community)
            .Include(g => g.Owner)
            .AsQueryable();

        if(request.CommunityId.HasValue)
        {
            q = q.Where(g => g.CommunityId == request.CommunityId);
        }

        if ( request.Filter != null)
        {
           foreach (var f in request.Filter.Filters)
            {
                if (!string.IsNullOrEmpty(f.Field) && f.Value != null)
                {
                    switch (f.Field.ToLowerInvariant())
                    {
                        case "name":
                            q = q.Where(g => g.Name.Contains(f.Value.ToString()));
                            break;
                        case "purpose":
                            q = q.Where(g => g.Purpose.Contains(f.Value.ToString()));
                            break;
                    }
                }
            }
        }

        return await q.GetPaginatedListResultAsync(request);
    }
}

