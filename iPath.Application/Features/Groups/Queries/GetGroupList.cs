using iPath.Application.Querying;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class GetGroupListQuery : PaginatedListQuery, IRequest<GetGroupListResponse>
{
    public int? CommunityId { get; set; }
}


public record GetGroupListResponse(bool Success, string? Message = default!, PaginatedListResult<Group> Data = null!)
    : BaseResponseT<PaginatedListResult<Group>>(Success, Message, Data);



public class GetGroupListQueryHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<GetGroupListQuery, GetGroupListResponse>
{
    public async Task<GetGroupListResponse> Handle(GetGroupListQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
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

        var data = await q.GetPaginatedListResultAsync(request);
        return new GetGroupListResponse(true, Data: data);
    }
}

