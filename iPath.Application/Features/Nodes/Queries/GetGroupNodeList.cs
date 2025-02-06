using iPath.Application.Querying;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class GetNodeListQuery : PaginatedListQuery, IRequest<PaginatedListResult<Node>>
{
    public int? GroupId { get; set; }
    public int? OwnerId { get; set; }
}


public class GetNodeListQueryHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<GetNodeListQuery, PaginatedListResult<Node>>
{
    public async Task<PaginatedListResult<Node>> Handle(GetNodeListQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        var q = ctx.Nodes.AsNoTracking()
            .Include(g => g.Owner)
            .OrderByDescending(g => g.CreateOn)
            .AsQueryable();

        if( request.GroupId.HasValue)
        {
            q = q.Where(g => g.GroupId == request.GroupId);
        }

        if(request.OwnerId.HasValue)
        {
            q = q.Where(g => g.OwnerId == request.OwnerId);
        }   

        if ( request.Filter != null)
        {
           foreach (var f in request.Filter.Filters)
            {
                if (!string.IsNullOrEmpty(f.Field) && f.Value != null)
                {
                    switch (f.Field.ToLowerInvariant())
                    {
                        case "title":
                            q = q.Where(g => g.Title.Contains(f.Value.ToString()));
                            break;
                    }
                }
            }
        }

        return await q.GetPaginatedListResultAsync(request);
    }
}

