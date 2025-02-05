using iPath.Application.Querying;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace iPath.Application.Features;




public record GroupListDTO(int Id, string Name, int? CommunityId, int? NodeCount, int? NewObjCount, int? NewCommentCount);


public class GetGroupListDtoQuery : PaginatedListQuery, IRequest<PaginatedListResult<GroupListDTO>>
{
    public int? UserId { get; set; }
    public int? CommunityId { get; set; }
}


public class GetGroupListDtoQueryHandler(IPathDbContext ctx)
    : IRequestHandler<GetGroupListDtoQuery, PaginatedListResult<GroupListDTO>>
{
    public async Task<PaginatedListResult<GroupListDTO>> Handle(GetGroupListDtoQuery request, CancellationToken cancellationToken)
    {
        var q = ctx.Groups
            .Include(g => g.Community)
            .AsQueryable();

        // filter by user
        if( request.UserId.HasValue)
        {
            // personal list
            q = q.Where(g => g.Members.Any(m => m.UserId == request.UserId));
        }
        else
        {
            // list of public groups
            q = q.Where(g => g.Visibility == eGroupVisibility.Public);
        }
           
        // filter by community
        if( request.CommunityId.HasValue)
        {
            q = q.Where(g => g.CommunityId == request.CommunityId.Value);
        }

        // get the total group count
        var total = await q.AsNoTracking().CountAsync();

        // sort by name and apply pagination
        q = q.OrderBy(g => g.Name);
        if (request.Count.HasValue)
            q = q.Skip(request.StartIndex * request.Count.Value).Take(request.Count.Value);

        // get paged data with object count, etx
        var dtos = await q.Include(g => g.Nodes)
            .AsNoTracking()
            .Select(g => new GroupListDTO(g.Id, g.Name, g.CommunityId, g.Nodes.Count(), 0, 0))
            .ToListAsync();

        return new PaginatedListResult<GroupListDTO>()
        {
            TotalItemsCount = total,
            Items = dtos
        };
    }
}

